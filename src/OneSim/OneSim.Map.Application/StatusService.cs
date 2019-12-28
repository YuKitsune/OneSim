namespace OneSim.Map.Application
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Threading.Tasks;

	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Logging;

	using OneSim.Map.Application.Abstractions;
	using OneSim.Map.Domain.Entities;

	/// <summary>
	/// 	The class responsible for maintaining a database of currently online pilots and ATC for a particular
	/// 	Online Flight Simulation Network.
	/// </summary>
	public class StatusService
	{
		/// <summary>
		/// 	The <see cref="IStatusDataProvider"/>.
		/// </summary>
		private readonly IStatusDataProvider _statusDataProvider;

		/// <summary>
		/// 	The <see cref="IStatusDataParser"/>.
		/// </summary>
		private readonly IStatusDataParser _statusDataParser;

		/// <summary>
		/// 	The <see cref="IStatusDbContext"/>.
		/// </summary>
		private readonly IStatusDbContext _statusDbContext;

		/// <summary>
		/// 	The <see cref="IHistoricalDbContext"/>.
		/// </summary>
		private readonly IHistoricalDbContext _historicalDbContext;

		/// <summary>
		/// 	The <see cref="Logger{TCategoryName}"/>.
		/// </summary>
		private readonly ILogger<StatusService> _logger;

		/// <summary>
		/// 	Initializes a new instance of the <see cref="StatusService"/> class.
		/// </summary>
		/// <param name="statusDataProvider">
		///		The <see cref="IStatusDataProvider"/>.
		/// </param>
		/// <param name="statusDataParser">
		///		The <see cref="IStatusDataParser"/>.
		/// </param>
		/// <param name="statusDbContext">
		///		The <see cref="IStatusDbContext"/>.
		/// </param>
		/// <param name="historicalDbContext">
		///		The <see cref="IHistoricalDbContext"/>.
		/// </param>
		/// <param name="logger">
		///		The <see cref="ILogger{TCategoryName}"/>.
		/// </param>
		public StatusService(
			IStatusDataProvider statusDataProvider,
			IStatusDataParser statusDataParser,
			IStatusDbContext statusDbContext,
			IHistoricalDbContext historicalDbContext,
			ILogger<StatusService> logger)
		{
			_statusDataProvider = statusDataProvider ?? throw new ArgumentNullException(nameof(statusDataProvider), "The Status File Provider cannot be null.");
			_statusDataParser = statusDataParser ?? throw new ArgumentNullException(nameof(statusDataParser), "The Status File Parser cannot be null.");
			_statusDbContext = statusDbContext ?? throw new ArgumentNullException(nameof(statusDbContext), "The Status DbContext cannot be null.");
			_historicalDbContext = historicalDbContext ?? throw new ArgumentNullException(nameof(historicalDbContext), "The Historical DbContext cannot be null.");
			_logger = logger ?? throw new ArgumentNullException(nameof(logger), "The Logger cannot be null.");
		}

		/// <summary>
		/// 	Updates the status data in the <see cref="IStatusDbContext"/> and creates a new entry in the
		/// 	<see cref="IHistoricalDbContext"/>.
		/// </summary>
		/// <returns>
		///		The <see cref="Task"/>.
		/// </returns>
		public async Task UpdateStatusDataAsync()
		{
			using (_logger.BeginScope("Updating Status data"))
			{
				// Start preparing the archive entry
				StatusDataArchiveEntry archiveEntry = new StatusDataArchiveEntry();

				// Get the status data
				_logger.LogInformation("Downloading Status data.");
				StatusDownloadResult downloadResult = await _statusDataProvider.GetStatusDataAsync();
				_logger.LogInformation($"Download complete. Total download time: {downloadResult.DownloadTime.TotalSeconds:##,###.00}s.");

				// Store the Status Data for archiving purposes
				archiveEntry.StatusFile = downloadResult.RawContent;
				archiveEntry.DateReceived = downloadResult.DateReceived;
				archiveEntry.DownloadTime = downloadResult.DownloadTime;
				_historicalDbContext.StatusData.Add(archiveEntry);
				await _historicalDbContext.SaveChangesAsync();
				_logger.LogInformation("Status data archived.");

				// Parse the data
				_logger.LogInformation("Parsing Status data.");
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				StatusParseResult parseResult = _statusDataParser.Parse(downloadResult.RawContent);
				stopwatch.Stop();
				_logger.LogInformation($"Parse complete. Total parse time: {stopwatch.Elapsed.TotalSeconds:##,###.00}s.");

				// Update the data
				using (_logger.BeginScope("Updating database"))
				{
					await UpdatePilotsAsync(parseResult.Pilots);
					await UpdateControllersAsync(parseResult.Controllers);
					await UpdatePreFileNoticesAsync(parseResult.FlightNotifications);
					await UpdateServersAsync(parseResult.Servers);

					_logger.LogInformation("Database update complete.");
				}
			}
		}

		/// <summary>
		/// 	Updates the <see cref="Pilot"/>s in the <see cref="IStatusDbContext"/>.
		/// </summary>
		/// <param name="newPilots">
		///		The <see cref="ICollection{T}"/> of new <see cref="Pilot"/>s.
		/// </param>
		/// <returns>
		///		The <see cref="Task"/>.
		/// </returns>
		private async Task UpdatePilotsAsync(ICollection<Pilot> newPilots)
		{
			_logger.LogInformation("Updating Pilots.");

			// Get all the pilots from the last batch
			List<Pilot> oldPilots = await _statusDbContext.Pilots
														  .Include(p => p.FlightPlan)
														  .ToListAsync();

			// Get all the pilots still connected
			List<Pilot> pilotsStillOnline = oldPilots.Where(p => newPilots
																.Select(np => new { np.Callsign, np.NetworkId })
																.Contains(new { p.Callsign, p.NetworkId }))
													 .ToList();

			// Remove all of the pilots, flight plans (only for pilots with plans) and history trails
			_statusDbContext.Points.RemoveRange(_statusDbContext.Points);
			_statusDbContext.Pilots.RemoveRange(_statusDbContext.Pilots);
			_statusDbContext.FlightPlans.RemoveRange(_statusDbContext.Pilots.Where(p => p.FlightPlan != null).Select(p => p.FlightPlan));
			await _statusDbContext.SaveChangesAsync();

			// Update the histories for all the new pilots
			foreach (Pilot pilot in newPilots)
			{
				// Find the current pilot from the old batch
				Pilot oldPilot = pilotsStillOnline.FirstOrDefault(p => p.Callsign == pilot.Callsign &&
																	   p.NetworkId == pilot.NetworkId);

				// If the pilot is in the old batch, then copy their history across
				if (oldPilot != null) pilot.History = oldPilot.History;

				// Add to the history
				pilot.History.Add(new Point3d
								  {
									  Latitude = pilot.Latitude,
									  Longitude = pilot.Longitude,
									  Altitude = pilot.Altitude,
									  DateTime = DateTime.UtcNow
								  });

				// Add the pilot to the database
				_statusDbContext.Pilots.Add(pilot);
			}

			// Save our changes
			await _statusDbContext.SaveChangesAsync();
		}

		/// <summary>
		/// 	Updates the <see cref="AirTrafficController"/>s in the <see cref="IStatusDbContext"/>.
		/// </summary>
		/// <param name="newControllers">
		///		The <see cref="ICollection{T}"/> of new <see cref="AirTrafficController"/>s.
		/// </param>
		/// <returns>
		///		The <see cref="Task"/>.
		/// </returns>
		private async Task UpdateControllersAsync(IEnumerable<AirTrafficController> newControllers)
		{
			_logger.LogInformation("Updating Controllers.");

			// Remove all of the controllers
			_statusDbContext.Controllers.RemoveRange(_statusDbContext.Controllers);
			await _statusDbContext.SaveChangesAsync();

			// Add all the new controllers
			await _statusDbContext.Controllers.AddRangeAsync(newControllers);
			await _statusDbContext.SaveChangesAsync();
		}

		/// <summary>
		/// 	Updates the <see cref="FlightNotification"/>s in the <see cref="IStatusDbContext"/>.
		/// </summary>
		/// <param name="newFlightNotifications">
		///		The <see cref="ICollection{T}"/> of new <see cref="FlightNotification"/>s.
		/// </param>
		/// <returns>
		///		The <see cref="Task"/>.
		/// </returns>
		private async Task UpdatePreFileNoticesAsync(IEnumerable<FlightNotification> newFlightNotifications)
		{
			_logger.LogInformation($"Updating Flight Notifications.");

			// Get all the notices from the last batch
			List<FlightNotification> oldNotifications = await _statusDbContext.FlightNotifications
																			  .Include(n => n.FlightPlan)
																			  .ToListAsync();

			// Remove all of the notices and flight plans
			_statusDbContext.FlightPlans.RemoveRange(oldNotifications.Select(n => n.FlightPlan));
			_statusDbContext.FlightNotifications.RemoveRange(_statusDbContext.FlightNotifications);
			await _statusDbContext.SaveChangesAsync();

			// Add all the new notices
			await _statusDbContext.FlightNotifications.AddRangeAsync(newFlightNotifications);
			await _statusDbContext.SaveChangesAsync();
		}

		/// <summary>
		/// 	Updates the <see cref="Server"/>s in the <see cref="IStatusDbContext"/>.
		/// </summary>
		/// <param name="newServers">
		///		The <see cref="ICollection{T}"/> of new <see cref="Server"/>s.
		/// </param>
		/// <returns>
		///		The <see cref="Task"/>.
		/// </returns>
		private async Task UpdateServersAsync(IEnumerable<Server> newServers)
		{
			_logger.LogInformation("Updating Servers.");

			// Remove all of the servers
			_statusDbContext.Servers.RemoveRange(_statusDbContext.Servers);
			await _statusDbContext.SaveChangesAsync();

			// Add all the new servers
			await _statusDbContext.Servers.AddRangeAsync(newServers);
			await _statusDbContext.SaveChangesAsync();
		}
	}
}
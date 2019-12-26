namespace OneSim.Map.Application
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Logging;

	using OneSim.Map.Application.Abstractions;
	using OneSim.Map.Domain.Entities;

	/// <summary>
	/// 	The Status Service.
	/// 	Responsible for parsing the Status files containing information about online ATC and pilots, and maintaining
	/// 	a database of live, and historical data.
	/// </summary>
	public class StatusService
	{
		/// <summary>
		/// 	The <see cref="IStatusFileProvider"/>.
		/// </summary>
		private readonly IStatusFileProvider _statusFileProvider;

		/// <summary>
		/// 	The <see cref="IStatusDbContext"/>.
		/// </summary>
		private readonly IStatusDbContext _statusDbContext;

		/// <summary>
		/// 	The <see cref="IHistoricalDbContext"/>.
		/// </summary>
		private readonly IHistoricalDbContext _historicalDbContext;

		/// <summary>
		/// 	The <see cref="IStatusFileParser"/>.
		/// </summary>
		private readonly IStatusFileParser _statusFileParser;

		/// <summary>
		/// 	The <see cref="Logger{TCategoryName}"/>.
		/// </summary>
		private readonly ILogger<StatusService> _logger;

		/// <summary>
		/// 	Initializes a new instance of the <see cref="StatusService"/> class.
		/// </summary>
		/// <param name="statusFileProvider">
		///		The <see cref="IStatusFileProvider"/>.
		/// </param>
		/// <param name="statusFileParser">
		///		The <see cref="IStatusFileParser"/>.
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
			IStatusFileProvider statusFileProvider,
			IStatusFileParser statusFileParser,
			IStatusDbContext statusDbContext,
			IHistoricalDbContext historicalDbContext,
			ILogger<StatusService> logger)
		{
			if (statusFileProvider == null) throw new ArgumentNullException(nameof(statusFileProvider), "The Status File Provider cannot be null.");
			if (statusFileParser == null) throw new ArgumentNullException(nameof(statusFileParser), "The Status File Parser cannot be null.");
			if (statusDbContext == null) throw new ArgumentNullException(nameof(statusDbContext), "The Status DbContext cannot be null.");
			if (historicalDbContext == null) throw new ArgumentNullException(nameof(historicalDbContext), "The Historical DbContext cannot be null.");
			if (logger == null) throw new ArgumentNullException(nameof(logger), "The Logger cannot be null.");

			_statusFileProvider = statusFileProvider;
			_statusFileParser = statusFileParser;
			_statusDbContext = statusDbContext;
			_historicalDbContext = historicalDbContext;
			_logger = logger;
		}

		/// <summary>
		/// 	Updates the status data.
		/// </summary>
		/// <returns>
		///		The <see cref="Task"/>.
		/// </returns>
		public async Task UpdateStatusDataAsync()
		{
			using (_logger.BeginScope("Updating Status data"))
			{
				// Start preparing the archive entry
				StatusFileArchiveEntry archiveEntry = new StatusFileArchiveEntry();

				// Get the status data
				_logger.LogInformation("Downloading Status data.");
				StatusFileDownloadResult downloadResult = await _statusFileProvider.GetStatusFileAsync();
				_logger.LogInformation($"Download complete. Time elapsed: {downloadResult.DownloadTime.TotalSeconds}.{downloadResult.DownloadTime.Milliseconds} seconds");

				// Store the Status Data for archiving purposes
				archiveEntry.StatusFile = downloadResult.RawStatusFile;
				archiveEntry.DateReceived = downloadResult.DateReceived;
				archiveEntry.DownloadTime = downloadResult.DownloadTime;
				_historicalDbContext.StatusFiles.Add(archiveEntry);
				await _historicalDbContext.SaveChangesAsync();

				// Parse the data
				_logger.LogInformation("Parsing Status data.");
				StatusFileParseResult parseResult = _statusFileParser.Parse(downloadResult.RawStatusFile);

				// Update the data
				_logger.LogInformation("Parse complete. Updating database.");
				await UpdatePilotsAsync(parseResult.Pilots);
				await UpdateControllersAsync(parseResult.Controllers);
				await UpdatePreFileNoticesAsync(parseResult.PreFileNotices);
				await UpdateServersAsync(parseResult.Servers);

				_logger.LogInformation("Database updated.");
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
			_logger.LogInformation($"Updating Pilots.");

			// Get all the pilots from the last batch
			List<Pilot> oldPilots = await _statusDbContext.Pilots.ToListAsync();

			// Get all the pilots still connected
			List<Pilot> pilotsStillOnline = oldPilots.Where(p => newPilots
																.Select(np => new { np.Callsign, np.NetworkId })
																.Contains(new { p.Callsign, p.NetworkId }))
													 .ToList();

			// Remove all of the pilots, flight plans and history trails
			_statusDbContext.Points.RemoveRange(oldPilots.SelectMany(op => op.History));
			_statusDbContext.FlightPlans.RemoveRange(oldPilots.Select(p => p.FlightPlan));
			_statusDbContext.Pilots.RemoveRange(oldPilots);

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
			_logger.LogInformation($"Pilots Updated.");
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

			// Get all the Controllers from the last batch
			List<AirTrafficController> oldControllers = await _statusDbContext.Controllers.ToListAsync();

			// Remove all of the Controllers
			_statusDbContext.Controllers.RemoveRange(oldControllers);

			// Add all the new controllers
			await _statusDbContext.Controllers.AddRangeAsync(newControllers);
			await _statusDbContext.SaveChangesAsync();
			_logger.LogInformation($"Controllers Updated.");
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
			List<FlightNotification> oldNotifications = await _statusDbContext.FlightNotifications.ToListAsync();

			// Remove all of the notices and flight plans
			_statusDbContext.FlightPlans.RemoveRange(oldNotifications.Select(n => n.FlightPlan));
			_statusDbContext.FlightNotifications.RemoveRange(oldNotifications);

			// Add all the new notices
			await _statusDbContext.FlightNotifications.AddRangeAsync(newFlightNotifications);
			await _statusDbContext.SaveChangesAsync();

			_logger.LogInformation($"Flight Notifications Updated.");
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
			_logger.LogInformation($"Updating Servers.");

			// Get all the servers from the last batch
			List<Server> oldServers = await _statusDbContext.Servers.ToListAsync();

			// Remove all of the notices
			_statusDbContext.Servers.RemoveRange(oldServers);

			// Add all the new servers
			await _statusDbContext.Servers.AddRangeAsync(newServers);
			await _statusDbContext.SaveChangesAsync();

			_logger.LogInformation($"Servers Updated.");
		}
	}
}
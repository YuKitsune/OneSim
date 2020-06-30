// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnlineTrafficService.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Application
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    using OneSim.Traffic.Application.Abstractions;
    using OneSim.Traffic.Domain.Entities;

    // Todo: Update summary to account for data sources which are not OSFNs.

    /// <summary>
    ///     The service responsible for maintaining a database of who's currently connected to a specific Online Flight
    ///      Simulation Network.
    /// </summary>
    public class OnlineTrafficService
    {
        /// <summary>
        ///     The <see cref="ITrafficDataProvider"/>.
        /// </summary>
        private readonly ITrafficDataProvider _statusFileProvider;

        /// <summary>
        ///     The <see cref="ITrafficDataParser"/>.
        /// </summary>
        private readonly ITrafficDataParser _statusFileParser;

        /// <summary>
        ///     The <see cref="ITrafficDbContext"/>.
        /// </summary>
        private readonly ITrafficDbContext _trafficDbContext;

        /// <summary>
        ///     The <see cref="IHistoricalDbContext"/>.
        /// </summary>
        private readonly IHistoricalDbContext _historicalDbContext;

        /// <summary>
        ///     The <see cref="ITrafficNotifier"/>.
        /// </summary>
        private readonly ITrafficNotifier _notifier;

        /// <summary>
        ///     The <see cref="Logger{TCategoryName}"/>.
        /// </summary>
        private readonly ILogger<OnlineTrafficService> _logger;

        /// <summary>
        ///     Initializes a new instance of the <see cref="OnlineTrafficService"/> class.
        /// </summary>
        /// <param name="statusFileProvider">
        ///     The <see cref="ITrafficDataProvider"/>.
        /// </param>
        /// <param name="statusFileParser">
        ///     The <see cref="ITrafficDataParser"/>.
        /// </param>
        /// <param name="trafficDbContext">
        ///     The <see cref="ITrafficDbContext"/>.
        /// </param>
        /// <param name="historicalDbContext">
        ///     The <see cref="IHistoricalDbContext"/>.
        /// </param>
        /// <param name="notifier">
        ///     The <see cref="ITrafficNotifier"/>.
        /// </param>
        /// <param name="logger">
        ///     The <see cref="ILogger{TCategoryName}"/>.
        /// </param>
        public OnlineTrafficService(
            ITrafficDataProvider statusFileProvider,
            ITrafficDataParser statusFileParser,
            ITrafficDbContext trafficDbContext,
            IHistoricalDbContext historicalDbContext,
            ITrafficNotifier notifier,
            ILogger<OnlineTrafficService> logger)
        {
            _statusFileProvider = statusFileProvider ?? throw new ArgumentNullException(nameof(statusFileProvider), "The Traffic Data Provider cannot be null.");
            _statusFileParser = statusFileParser ?? throw new ArgumentNullException(nameof(statusFileParser), "The Traffic Data Parser cannot be null.");
            _trafficDbContext = trafficDbContext ?? throw new ArgumentNullException(nameof(trafficDbContext), "The Traffic DbContext cannot be null.");
            _historicalDbContext = historicalDbContext ?? throw new ArgumentNullException(nameof(historicalDbContext), "The Historical DbContext cannot be null.");
            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier), "The Traffic Notifier cannot be null.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "The Logger cannot be null.");
        }

        /// <summary>
        ///     Updates the traffic data.
        /// </summary>
        /// <returns>
        ///        The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        public async Task UpdateTrafficDataAsync()
        {
            using (_logger.BeginScope("Updating traffic data"))
            {
                // Start preparing the archive entry
                TrafficDataArchiveEntry archiveEntry = new TrafficDataArchiveEntry();

                // Get the status data
                _logger.LogInformation("Downloading traffic data.");
                TrafficDataFetchResult fetchResult = await _statusFileProvider.GetTrafficDataAsync();
                _logger.LogInformation(
                    $"Download complete. Total download time: {fetchResult.FetchTime.TotalSeconds:##,###.00}s.");

                // Store the traffic data for archiving purposes
                archiveEntry.TrafficData = fetchResult.TrafficData;
                archiveEntry.DateReceived = fetchResult.DateReceived;
                archiveEntry.FetchTime = fetchResult.FetchTime;
                _historicalDbContext.TrafficData.Add(archiveEntry);
                await _historicalDbContext.SaveChangesAsync();
                _logger.LogInformation("Traffic data archived.");

                // Parse the data
                _logger.LogInformation("Parsing traffic data.");
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                TrafficDataParseResult parseResult = _statusFileParser.Parse(fetchResult.TrafficData);
                stopwatch.Stop();
                _logger.LogInformation(
                    $"Parse complete. Total parse time: {stopwatch.Elapsed.TotalSeconds:##,###.00}s.");

                // Update the data
                using (_logger.BeginScope("Updating database"))
                {
                    // Bug: Possible race condition. A request may come through part way through updating these entries.
                    await UpdatePilotsAsync(parseResult.Pilots);
                    await UpdateControllersAsync(parseResult.Controllers);
                    await UpdatePreFileNoticesAsync(parseResult.FlightNotifications);
                    await UpdateServersAsync(parseResult.Servers);

                    _logger.LogInformation("Database update complete.");
                }

                // Notify the subscribers to new traffic info
                await _notifier.NewTrafficDataAvailableAsync();
            }
        }

        /// <summary>
        ///     Updates the <see cref="Pilot"/>s in the <see cref="ITrafficDbContext"/>.
        /// </summary>
        /// <param name="newPilots">
        ///        The <see cref="ICollection{T}"/> of new <see cref="Pilot"/>s.
        /// </param>
        /// <returns>
        ///        The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        private async Task UpdatePilotsAsync(ICollection<Pilot> newPilots)
        {
            _logger.LogInformation("Updating Pilots.");

            // Get all the pilots from the last batch
            List<Pilot> oldPilots = await _trafficDbContext.Pilots
                                                           .Include(p => p.FlightPlan)
                                                           .ToListAsync();

            // Get all the pilots still connected
            List<Pilot> pilotsStillOnline = oldPilots.Where(
                                                          p => newPilots
                                                              .Select(np => new { np.Callsign, np.NetworkId })
                                                              .Contains(new { p.Callsign, p.NetworkId }))
                                                     .ToList();

            // Remove all of the pilots, flight plans (only for pilots with plans) and history trails
            _trafficDbContext.Points.RemoveRange(_trafficDbContext.Points);
            _trafficDbContext.Pilots.RemoveRange(_trafficDbContext.Pilots);
            _trafficDbContext.FlightPlans.RemoveRange(_trafficDbContext.Pilots.Where(p => p.FlightPlan != null).Select(p => p.FlightPlan));
            await _trafficDbContext.SaveChangesAsync();

            // Update the histories for all the new pilots
            foreach (Pilot pilot in newPilots)
            {
                // Find the current pilot from the old batch
                Pilot oldPilot = pilotsStillOnline.FirstOrDefault(
                    p => p.Callsign == pilot.Callsign &&
                         p.NetworkId == pilot.NetworkId);

                // If the pilot is in the old batch, then copy their history across
                if (oldPilot != null) pilot.History = oldPilot.History;

                // Add to the history
                pilot.History.Add(
                    new Point3d
                    {
                        Latitude = pilot.Latitude,
                        Longitude = pilot.Longitude,
                        Altitude = pilot.Altitude,
                        DateTime = DateTime.UtcNow
                    });

                // Add the pilot to the database
                _trafficDbContext.Pilots.Add(pilot);
            }

            // Save our changes
            await _trafficDbContext.SaveChangesAsync();
        }

        /// <summary>
        ///     Updates the <see cref="AirTrafficController"/>s in the <see cref="ITrafficDbContext"/>.
        /// </summary>
        /// <param name="newControllers">
        ///        The <see cref="ICollection{T}"/> of new <see cref="AirTrafficController"/>s.
        /// </param>
        /// <returns>
        ///        The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        private async Task UpdateControllersAsync(IEnumerable<AirTrafficController> newControllers)
        {
            _logger.LogInformation("Updating Controllers.");

            // Remove all of the controllers
            _trafficDbContext.Controllers.RemoveRange(_trafficDbContext.Controllers);
            await _trafficDbContext.SaveChangesAsync();

            // Add all the new controllers
            await _trafficDbContext.Controllers.AddRangeAsync(newControllers);
            await _trafficDbContext.SaveChangesAsync();
        }

        /// <summary>
        ///     Updates the <see cref="FlightNotification"/>s in the <see cref="ITrafficDbContext"/>.
        /// </summary>
        /// <param name="newFlightNotifications">
        ///        The <see cref="ICollection{T}"/> of new <see cref="FlightNotification"/>s.
        /// </param>
        /// <returns>
        ///        The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        private async Task UpdatePreFileNoticesAsync(IEnumerable<FlightNotification> newFlightNotifications)
        {
            _logger.LogInformation("Updating Flight Notifications.");

            // Get all the notices from the last batch
            List<FlightNotification> oldNotifications = await _trafficDbContext.FlightNotifications
                                                                               .Include(n => n.FlightPlan)
                                                                               .ToListAsync();

            // Remove all of the notices and flight plans
            _trafficDbContext.FlightPlans.RemoveRange(oldNotifications.Select(n => n.FlightPlan));
            _trafficDbContext.FlightNotifications.RemoveRange(_trafficDbContext.FlightNotifications);
            await _trafficDbContext.SaveChangesAsync();

            // Add all the new notices
            await _trafficDbContext.FlightNotifications.AddRangeAsync(newFlightNotifications);
            await _trafficDbContext.SaveChangesAsync();
        }

        /// <summary>
        ///     Updates the <see cref="Server"/>s in the <see cref="ITrafficDbContext"/>.
        /// </summary>
        /// <param name="newServers">
        ///        The <see cref="ICollection{T}"/> of new <see cref="Server"/>s.
        /// </param>
        /// <returns>
        ///        The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        private async Task UpdateServersAsync(IEnumerable<Server> newServers)
        {
            _logger.LogInformation("Updating Servers.");

            // Remove all of the servers
            _trafficDbContext.Servers.RemoveRange(_trafficDbContext.Servers);
            await _trafficDbContext.SaveChangesAsync();

            // Add all the new servers
            await _trafficDbContext.Servers.AddRangeAsync(newServers);
            await _trafficDbContext.SaveChangesAsync();
        }
    }
}

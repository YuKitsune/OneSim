// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrafficDataController.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Api.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using OneSim.Traffic.Api.Data;
    using OneSim.Traffic.Domain.Entities;
    using OneSim.Traffic.Persistence;

    /// <summary>
    ///     The <see cref="Controller"/> serving the data current traffic data.
    /// </summary>
    public class TrafficDataController : Controller
    {
        /// <summary>
        ///     The <see cref="TrafficDbContext"/>.
        /// </summary>
        private readonly TrafficDbContext _dbContext;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TrafficDataController"/> class.
        /// </summary>
        /// <param name="dbContext">
        ///        The <see cref="TrafficDbContext"/>.
        /// </param>
        public TrafficDataController(TrafficDbContext dbContext) => _dbContext = dbContext;

        /// <summary>
        ///     Gets all of the current traffic data in the form of a <see cref="CurrentTraffic"/> object.
        /// </summary>
        /// <returns>
        ///        The <see cref="CurrentTraffic"/> as a <see cref="JsonResult"/>.
        /// </returns>
        public async Task<JsonResult> All()
        {
            // Get all the data
            CurrentTraffic traffic = new CurrentTraffic
                                     {
                                         Pilots = await _dbContext.Pilots.ToListAsync(),
                                         Controllers = await _dbContext.Controllers.ToListAsync(),
                                         FlightNotifications = await _dbContext.FlightNotifications.Include(n => n.FlightPlan).ToListAsync(),
                                         Servers = await _dbContext.Servers.ToListAsync()
                                     };

            return Json(traffic);
        }

        /// <summary>
        ///     Gets all of the currently online <see cref="Pilot"/>s.
        /// </summary>
        /// <returns>
        ///        The <see cref="Pilot"/>s as a <see cref="JsonResult"/>.
        /// </returns>
        public async Task<JsonResult> Pilots()
        {
            // Get all the pilots
            List<Pilot> pilots = await _dbContext.Pilots
                                                 .Include(p => p.FlightPlan)
                                                 .Include(p => p.History)
                                                 .ToListAsync();

            // Return as JSON
            return Json(pilots);
        }

        /// <summary>
        ///     Gets a specific <see cref="Pilot"/> given a <see cref="BaseClient.Callsign"/>.
        /// </summary>
        /// <param name="callsign">
        ///        The <see cref="BaseClient.Callsign"/>.
        /// </param>
        /// <returns>
        ///        The <see cref="Pilot"/> as a <see cref="JsonResult"/>.
        /// </returns>
        public async Task<JsonResult> Pilot(string callsign)
        {
            // Get the pilot with the matching callsign
            Pilot pilots = await _dbContext.Pilots
                                           .Include(p => p.FlightPlan)
                                           .Include(p => p.History)
                                           .FirstOrDefaultAsync(p => p.Callsign == callsign);

            // Return as JSON
            return Json(pilots);
        }

        /// <summary>
        ///     Gets all of the currently online <see cref="AirTrafficController"/>s.
        /// </summary>
        /// <returns>
        ///        The <see cref="AirTrafficController"/>s as a <see cref="JsonResult"/>.
        /// </returns>
        public async Task<JsonResult> Controllers()
        {
            // Get all the controllers
            List<AirTrafficController> controllers = await _dbContext.Controllers.ToListAsync();

            // Return as JSON
            return Json(controllers);
        }

        /// <summary>
        ///     Gets a specific <see cref="AirTrafficController"/> given a <see cref="BaseClient.Callsign"/>.
        /// </summary>
        /// <param name="callsign">
        ///        The <see cref="BaseClient.Callsign"/>.
        /// </param>
        /// <returns>
        ///        The <see cref="AirTrafficController"/> as a <see cref="JsonResult"/>.
        /// </returns>
        public async Task<JsonResult> Controller(string callsign)
        {
            // Get the controller with the matching callsign
            AirTrafficController controller = await _dbContext.Controllers
                                                              .FirstOrDefaultAsync(c => c.Callsign == callsign);

            // Return as JSON
            return Json(controller);
        }

        /// <summary>
        ///     Gets all of the currently filed <see cref="FlightNotification"/>s.
        /// </summary>
        /// <returns>
        ///        The <see cref="FlightNotification"/>s as a <see cref="JsonResult"/>.
        /// </returns>
        public async Task<JsonResult> FlightNotifications()
        {
            // Get all the flight notifications
            List<FlightNotification> flightNotifications = await _dbContext.FlightNotifications
                                                                           .Include(n => n.FlightPlan)
                                                                           .ToListAsync();

            // Return as JSON
            return Json(flightNotifications);
        }

        /// <summary>
        ///     Gets all of the currently online <see cref="Server"/>s.
        /// </summary>
        /// <returns>
        ///        The <see cref="Server"/>s as a <see cref="JsonResult"/>.
        /// </returns>
        public async Task<JsonResult> Servers()
        {
            // Get all the servers
            List<Server> servers = await _dbContext.Servers.ToListAsync();

            // Return as JSON
            return Json(servers);
        }
    }
}

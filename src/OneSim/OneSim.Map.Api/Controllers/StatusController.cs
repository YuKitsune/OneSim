namespace OneSim.Api.Map.Controllers
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Mvc;
	using Microsoft.EntityFrameworkCore;

	using OneSim.Api.Map.Data;
	using OneSim.Map.Domain.Entities;
	using OneSim.Map.Persistence;

	/// <summary>
	/// 	The Status <see cref="Controller"/>.
	/// </summary>
	public class StatusController : Controller
	{
		/// <summary>
		/// 	The <see cref="StatusDbContext"/>.
		/// </summary>
		private readonly StatusDbContext _dbContext;

		/// <summary>
		/// 	Initializes a new instance of the <see cref="StatusController"/> class.
		/// </summary>
		/// <param name="dbContext">
		///		The <see cref="StatusDbContext"/>.
		/// </param>
		public StatusController(StatusDbContext dbContext) => _dbContext = dbContext;

		/// <summary>
		/// 	Gets all of the current Status data in the form of a <see cref="CurrentStatus"/> object.
		/// </summary>
		/// <returns>
		///		The <see cref="CurrentStatus"/> as a <see cref="JsonResult"/>.
		/// </returns>
		public async Task<JsonResult> All()
		{
			// Get all the data
			CurrentStatus status = new CurrentStatus
								   {
									   Pilots = await _dbContext.Pilots.Include(p => p.FlightPlan).ToListAsync(),
									   Controllers = await _dbContext.Controllers.ToListAsync(),
									   FlightNotifications = await _dbContext.FlightNotifications.Include(n => n.FlightPlan).ToListAsync(),
									   Servers = await _dbContext.Servers.ToListAsync()
								   };

			return Json(status);
		}

		/// <summary>
		/// 	Gets all of the currently online <see cref="Pilot"/>s.
		/// </summary>
		/// <returns>
		///		The <see cref="Pilot"/>s as a <see cref="JsonResult"/>.
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
		/// 	Gets a specific <see cref="Pilot"/> given a <see cref="BaseClient.Callsign"/>.
		/// </summary>
		/// <param name="callsign">
		///		The <see cref="BaseClient.Callsign"/>.
		/// </param>
		/// <returns>
		///		The <see cref="Pilot"/> as a <see cref="JsonResult"/>.
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
		/// 	Gets all of the currently online <see cref="AirTrafficController"/>s.
		/// </summary>
		/// <returns>
		///		The <see cref="AirTrafficController"/>s as a <see cref="JsonResult"/>.
		/// </returns>
		public async Task<JsonResult> Controllers()
		{
			// Get all the controllers
			List<AirTrafficController> controllers = await _dbContext.Controllers.ToListAsync();

			// Return as JSON
			return Json(controllers);
		}

		/// <summary>
		/// 	Gets a specific <see cref="AirTrafficController"/> given a <see cref="BaseClient.Callsign"/>.
		/// </summary>
		/// <param name="callsign">
		///		The <see cref="BaseClient.Callsign"/>.
		/// </param>
		/// <returns>
		///		The <see cref="AirTrafficController"/> as a <see cref="JsonResult"/>.
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
		/// 	Gets all of the currently filed <see cref="FlightNotification"/>s.
		/// </summary>
		/// <returns>
		///		The <see cref="FlightNotification"/>s as a <see cref="JsonResult"/>.
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
		/// 	Gets all of the currently online <see cref="Server"/>s.
		/// </summary>
		/// <returns>
		///		The <see cref="Server"/>s as a <see cref="JsonResult"/>.
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
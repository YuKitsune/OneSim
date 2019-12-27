namespace OneSim.Api.Map.Data
{
	using System.Collections.Generic;

	using OneSim.Map.Domain.Entities;

	/// <summary>
	/// 	The current status data.
	/// </summary>
	public class CurrentStatus
	{
		/// <summary>
		/// 	Gets the <see cref="List{T}"/> of <see cref="Pilot"/>s.
		/// </summary>
		public List<Pilot> Pilots { get; set; }

		/// <summary>
		/// 	Gets the <see cref="List{T}"/> of <see cref="AirTrafficController"/>s.
		/// </summary>
		public List<AirTrafficController> Controllers { get; set; }

		/// <summary>
		/// 	Gets the <see cref="List{T}"/> of <see cref="FlightNotification"/>s.
		/// </summary>
		public List<FlightNotification> FlightNotifications { get; set; }

		/// <summary>
		/// 	Gets the <see cref="List{T}"/> of <see cref="Server"/>s.
		/// </summary>
		public List<Server> Servers { get; set; }
	}
}
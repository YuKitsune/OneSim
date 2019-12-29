namespace OneSim.Traffic.Map.Data
{
	using System.Collections.Generic;

	using OneSim.Traffic.Domain.Entities;

	/// <summary>
	/// 	The class for presenting all the different elements of traffic data.
	/// </summary>
	public class CurrentTraffic
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
namespace OneSim.Map.Application
{
	using System.Collections.Generic;

	using OneSim.Map.Domain.Entities;

	/// <summary>
	/// 	The results from parsing a set of status data.
	/// </summary>
	public class StatusParseResult
	{
		/// <summary>
		/// 	Gets the <see cref="List{T}"/> of <see cref="Pilot"/>s.
		/// </summary>
		public List<Pilot> Pilots { get; }

		/// <summary>
		/// 	Gets the <see cref="List{T}"/> of <see cref="AirTrafficController"/>s.
		/// </summary>
		public List<AirTrafficController> Controllers { get; }

		/// <summary>
		/// 	Gets the <see cref="List{T}"/> of <see cref="FlightNotification"/>s.
		/// </summary>
		public List<FlightNotification> FlightNotifications { get; }

		/// <summary>
		/// 	Gets the <see cref="List{T}"/> of <see cref="Server"/>s.
		/// </summary>
		public List<Server> Servers { get; }

		/// <summary>
		/// 	Gets the <see cref="List{T}"/> of <see cref="StatusDataParseError"/>s.
		/// </summary>
		public List<StatusDataParseError> Errors { get; }

		/// <summary>
		/// 	Initializes a new instance of the <see cref="StatusParseResult"/>.
		/// </summary>
		public StatusParseResult()
		{
			Pilots = new List<Pilot>();
			Controllers = new List<AirTrafficController>();
			FlightNotifications = new List<FlightNotification>();
			Servers = new List<Server>();
			Errors = new List<StatusDataParseError>();
		}
	}
}
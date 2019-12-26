namespace OneSim.Map.Application
{
	using System.Collections.Generic;

	using OneSim.Map.Domain.Entities;

	/// <summary>
	/// 	The status file parse result.
	/// </summary>
	public class StatusFileParseResult
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
		public List<FlightNotification> PreFileNotices { get; }

		/// <summary>
		/// 	Gets the <see cref="List{T}"/> of <see cref="Server"/>s.
		/// </summary>
		public List<Server> Servers { get; }

		/// <summary>
		/// 	Gets the <see cref="List{T}"/> of <see cref="StatusFileParseError"/>s.
		/// </summary>
		public List<StatusFileParseError> Errors { get; }

		/// <summary>
		/// 	Initializes a new instance of the <see cref="StatusFileParseResult"/>.
		/// </summary>
		public StatusFileParseResult()
		{
			Pilots = new List<Pilot>();
			Controllers = new List<AirTrafficController>();
			PreFileNotices = new List<FlightNotification>();
			Servers = new List<Server>();
			Errors = new List<StatusFileParseError>();
		}
	}
}
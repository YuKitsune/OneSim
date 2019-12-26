namespace OneSim.Map.Domain.Entities
{
	/// <summary>
	/// 	The Flight Notification. Also known as a "prefile", or "pre-filed flight plan".
	/// </summary>
	public class FlightNotification
	{
		/// <summary>
		/// 	Gets or sets the ID.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		///		Gets or sets the callsign.
		/// </summary>
		public string Callsign { get; set; }

		/// <summary>
		///     Gets or sets the clients specific network ID.
		/// </summary>
		public string NetworkId { get; set; }

		/// <summary>
		///		Gets or sets the clients name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///		Gets or sets the <see cref="FlightPlan"/>.
		/// </summary>
		public FlightPlan FlightPlan { get; set; }
	}
}
namespace OneSim.Traffic.Domain.Entities
{
	/// <summary>
	/// 	The Server.
	/// </summary>
	public class Server
	{
		/// <summary>
		///     Gets or sets the ID.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		///     Gets or sets the name of the server.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///     Gets or sets the network specific identifier.
		/// </summary>
		public string NetworkIdentifier { get; set; }

		/// <summary>
		///     Gets or sets the IP address.
		/// </summary>
		public string IpAddress { get; set; }

		/// <summary>
		///     Gets or sets the location.
		/// </summary>
		public string Location { get; set; }
	}
}
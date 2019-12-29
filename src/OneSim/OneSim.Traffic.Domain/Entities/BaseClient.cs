namespace OneSim.Traffic.Domain.Entities
{
	using System;

	/// <summary>
	/// 	The base online network client.
	/// </summary>
	public abstract class BaseClient
	{
		/// <summary>
		/// 	Gets or sets the ID.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		///     Gets or sets the specific online network ID.
		/// </summary>
		public string NetworkId { get; set; }

		/// <summary>
		///     Gets or sets the logon callsign.
		/// </summary>
		public string Callsign { get; set; }

		/// <summary>
		///     Gets or sets the real name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///     Gets or sets the <see cref="OneSim.Traffic.Domain.Entities.Server.NetworkIdentifier"/> of the server the client has connected to.
		/// </summary>
		public string Server { get; set; }

		/// <summary>
		/// 	Gets or sets the <see cref="AdministrativeRating"/>
		/// </summary>
		public AdministrativeRating AdministrativeRating { get; set; }

		/// <summary>
		///     Gets or sets the UTC <see cref="DateTime"/> at which the current <see cref="BaseClient"/> connected to
		/// 	the network.
		/// </summary>
		public DateTime LogonTime { get; set; }

		/// <summary>
		///     Gets the <see cref="TimeSpan"/> which the current <see cref="BaseClient"/> has been connected to the
		/// 	network for.
		/// </summary>
		public TimeSpan TimeOnline => DateTime.UtcNow - LogonTime;
	}
}
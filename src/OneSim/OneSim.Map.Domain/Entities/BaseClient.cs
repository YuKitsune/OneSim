namespace OneSim.Map.Domain.Entities
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
		public string Id { get; set; }

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
		///     Gets or sets the name of the server the client has connected to.
		/// 	(Entities.Server.<see cref="Entities.Server.NetworkIdentifier"/>).
		/// </summary>
		public string Server { get; set; }

		/// <summary>
		///     Gets or sets the protocol revision in use client software.
		/// </summary>
		public string ProtocolRevision { get; set; }

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
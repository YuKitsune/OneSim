namespace OneSim.Map.Domain.Entities
{
	using System;

	/// <summary>
	/// 	The Status File Archive Entry.
	/// </summary>
	public class StatusFileArchiveEntry
	{
		/// <summary>
		/// 	Gets or sets the ID.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// 	Gets or sets the <see cref="DateTime"/> at which the <see cref="StatusFile"/> was downloaded.
		/// </summary>
		public DateTime DateReceived { get; set; }

		/// <summary>
		/// 	Gets or sets the <see cref="TimeSpan"/> representing the amount of time it took to download the
		/// 	<see cref="StatusFile"/>.
		/// </summary>
		public TimeSpan DownloadTime { get; set; }

		/// <summary>
		/// 	Gets or sets the URL where the <see cref="StatusFile"/> was downloaded from.
		/// </summary>
		public string SourceUrl { get; set; }

		/// <summary>
		/// 	Gets or sets the <see cref="string"/> representing the raw status file.
		/// </summary>
		public string StatusFile { get; set; }
	}
}
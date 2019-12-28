namespace OneSim.Map.Application
{
	using System;

	/// <summary>
	/// 	The Online Flight Simulation Network status data download result.
	/// </summary>
	public class StatusDownloadResult
	{
		/// <summary>
		/// 	Gets the raw content of the status data.
		/// </summary>
		public string RawContent { get; }

		/// <summary>
		/// 	Gets the <see cref="DateReceived"/> at which the data was downloaded.
		/// </summary>
		public DateTime DateReceived { get; }

		/// <summary>
		/// 	Gets the <see cref="TimeSpan"/> taken to download the status data.
		/// </summary>
		public TimeSpan DownloadTime { get; }

		/// <summary>
		/// 	Gets the URL where the status data was downloaded from.
		/// </summary>
		public string SourceUrl { get; }

		/// <summary>
		/// 	Initializes a new instance of the <see cref="StatusDownloadResult"/> class.
		/// </summary>
		/// <param name="rawContent">
		///		The raw content of the status data.
		/// </param>
		/// <param name="sourceUrl">
		///		The URL where the <paramref name="rawContent"/> was downloaded from.
		/// </param>
		/// <param name="dateReceived">
		///		The <see cref="DateTime"/> at which the <paramref name="rawContent"/> was downloaded.
		/// </param>
		/// <param name="downloadTime">
		///		The <see cref="TimeSpan"/> taken to download the <paramref name="rawContent"/>.
		/// </param>
		public StatusDownloadResult(
			string rawContent,
			string sourceUrl,
			DateTime dateReceived,
			TimeSpan downloadTime)
		{
			if (string.IsNullOrEmpty(sourceUrl)) throw new ArgumentNullException(nameof(sourceUrl), "The Source URL cannot be null or empty.");
			if (dateReceived == default) throw new ArgumentNullException(nameof(dateReceived), "The Date Received cannot be the default DateTime value.");
			if (downloadTime == null ||
				downloadTime == default)
				throw new ArgumentNullException(nameof(downloadTime), "The Download Time cannot be null, or the default TimeSpan value.");

			RawContent = rawContent;
			SourceUrl = sourceUrl;
			DateReceived = dateReceived;
			DownloadTime = downloadTime;
		}
	}
}
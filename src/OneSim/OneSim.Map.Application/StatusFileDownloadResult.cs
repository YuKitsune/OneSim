namespace OneSim.Map.Application
{
	using System;

	/// <summary>
	/// 	The status file download result.
	/// </summary>
	public class StatusFileDownloadResult
	{
		/// <summary>
		/// 	Gets the raw status file content.
		/// </summary>
		public string RawStatusFile { get; }

		/// <summary>
		/// 	Gets the <see cref="DateReceived"/> at which the file was downloaded.
		/// </summary>
		public DateTime DateReceived { get; }

		/// <summary>
		/// 	Gets the <see cref="TimeSpan"/> taken to download the status file.
		/// </summary>
		public TimeSpan DownloadTime { get; }

		/// <summary>
		/// 	Gets the URL where the status file was downloaded from.
		/// </summary>
		public string SourceUrl { get; }

		/// <summary>
		/// 	Initializes a new instance of the <see cref="StatusFileDownloadResult"/> class.
		/// </summary>
		/// <param name="rawStatusFile">
		///		The raw status file content.
		/// </param>
		/// <param name="sourceUrl">
		///		The URL where the <paramref name="rawStatusFile"/> was downloaded from.
		/// </param>
		/// <param name="dateReceived">
		///		The <see cref="DateTime"/> at which the <paramref name="rawStatusFile"/> was downloaded.
		/// </param>
		/// <param name="downloadTime">
		///		The <see cref="TimeSpan"/> taken to download the <paramref name="rawStatusFile"/>.
		/// </param>
		public StatusFileDownloadResult(
			string rawStatusFile,
			string sourceUrl,
			DateTime dateReceived,
			TimeSpan downloadTime)
		{
			if (string.IsNullOrEmpty(sourceUrl)) throw new ArgumentNullException(nameof(sourceUrl), "The Source URL cannot be null or empty.");
			if (dateReceived == default) throw new ArgumentNullException(nameof(dateReceived), "The Date Received cannot be the default DateTime value.");
			if (downloadTime == null ||
				downloadTime == default)
				throw new ArgumentNullException(nameof(downloadTime), "The Download Time cannot be null, or the default TimeSpan value.");

			RawStatusFile = rawStatusFile;
			SourceUrl = sourceUrl;
			DateReceived = dateReceived;
			DownloadTime = downloadTime;
		}
	}
}
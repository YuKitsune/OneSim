namespace OneSim.Map.Infrastructure.Ivao
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Net;
	using System.Threading.Tasks;

	using Microsoft.Extensions.Configuration;

	using OneSim.Map.Application;
	using OneSim.Map.Application.Abstractions;
	using OneSim.Map.Domain.Attributes;
	using OneSim.Map.Domain.Entities;

	/// <summary>
	/// 	The IvAo <see cref="IStatusDataProvider"/>.
	/// </summary>
	[Network(NetworkType.Ivao)]
	public class IvaoStatusDataProvider : IStatusDataProvider
	{
		/// <summary>
		/// 	Gets or sets the last URL used to fetch the VATSIM Status data.
		/// </summary>
		/// <remarks>
		///		Need to keep track of this so we don't make the same request to the same server too often.
		/// </remarks>
		private static string LastUsedUrl { get; set; }

		/// <summary>
		/// 	The <see cref="StatusDataProviderSettings"/>.
		/// </summary>
		private readonly StatusDataProviderSettings _settings;

		/// <summary>
		/// 	The Status file URLs.
		/// </summary>
		private List<string> _statusUrls;

		/// <summary>
		/// 	The last <see cref="DateTime"/> at which the root status file was downloaded.
		/// </summary>
		private DateTime _lastRootDownloadTime;

		/// <summary>
		/// 	Initializes a new instance of the <see cref="IvaoStatusDataProvider"/> class.
		/// </summary>
		/// <param name="settings">
		///		The <see cref="StatusDataProviderSettings"/>.
		/// </param>
		public IvaoStatusDataProvider(StatusDataProviderSettings settings) =>
			_settings = settings ?? throw new ArgumentNullException(nameof(settings), "The settings cannot be null.");

		/// <summary>
		/// 	Initializes a new instance of the <see cref="IvaoStatusDataProvider"/> class.
		/// </summary>
		/// <param name="configuration">
		///		The <see cref="IConfiguration"/>.
		/// </param>
		public IvaoStatusDataProvider(IConfiguration configuration)
		{
			// Extract the settings from the configuration
			StatusDataProviderSettings settings =
				configuration.GetSection("StatusProviderSettings").Get<StatusDataProviderSettings>();
			_settings = settings ?? throw new ArgumentNullException(nameof(settings), "Couldn't find the \"StatusProviderSettings\" section in the configuration.");
		}

		/// <summary>
		/// 	Gets the status data.
		/// </summary>
		/// <returns>
		///		The <see cref="StatusDownloadResult"/>.
		/// </returns>
		public StatusDownloadResult GetStatusData() => GetStatusDataAsync().GetAwaiter().GetResult();

		/// <summary>
		/// 	Gets the status data as an asynchronous operation.
		/// </summary>
		/// <returns>
		///		The <see cref="StatusDownloadResult"/>.
		/// </returns>
		public async Task<StatusDownloadResult> GetStatusDataAsync()
		{
			// If there is no previously used URL, or we need to refresh the URLs then download new URLs
			if (string.IsNullOrEmpty(LastUsedUrl) ||
				DateTime.UtcNow >= _lastRootDownloadTime.AddMinutes(_settings.MinutesBeforeRootRefresh))
			{
				string rootStatusFile = await GetRootStatusFileAsync();
				_statusUrls = GetStatusUrls(rootStatusFile).ToList();

				// If we've run out of URLs, then throw an exception
				if (!_statusUrls.Any()) throw new Exception("Could not find any status URLs.");

				// Set the last URL used to the first one
				LastUsedUrl = _statusUrls[0];
			}

			// Get a random URL and convert to a URI
			string url = GetRandomUrl(_statusUrls, LastUsedUrl);
			Uri uri = new Uri(url);

			// Download the status file and time the duration
			using WebClient client = new WebClient();
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			string statusFile = await client.DownloadStringTaskAsync(uri);
			stopwatch.Stop();
			DateTime downloadTime = DateTime.UtcNow;

			// Update the last URL used
			LastUsedUrl = url;

			// Return the result
			return new StatusDownloadResult(statusFile, url, downloadTime, stopwatch.Elapsed);
		}

		/// <summary>
		/// 	Gets the root data file.
		/// </summary>
		/// <returns>
		///		The root data file content.
		/// </returns>
		private async Task<string> GetRootStatusFileAsync()
		{
			using WebClient client = new WebClient();
			Uri rootUri = new Uri(_settings.RootStatusUrl);
			string rootDataFile = await client.DownloadStringTaskAsync(rootUri);

			_lastRootDownloadTime = DateTime.UtcNow;

			return rootDataFile;
		}

		/// <summary>
		/// 	Gets the status URLs from the root status file.
		/// </summary>
		/// <param name="rootStatusFile">
		///		The root status file content.
		/// </param>
		/// <returns>
		///		The status file URLs.
		/// </returns>
		private static IEnumerable<string> GetStatusUrls(string rootStatusFile)
		{
			if (string.IsNullOrEmpty(rootStatusFile)) throw new ArgumentNullException(nameof(rootStatusFile), "The root status file content cannot be null or empty.");

			// Get each line
			string[] lines = rootStatusFile.Split(Environment.NewLine);

			List<string> urls = new List<string>();
			foreach (string line in lines)
			{
				// Ignore lines that don't contain the status file URL
				if (!line.StartsWith("url0", StringComparison.Ordinal)) continue;

				// Extract the URL
				string url = line.Replace("url0=", string.Empty);
				urls.Add(url);
			}

			return urls;
		}

		/// <summary>
		/// 	Gets a random URL from the given list of URLs.
		/// </summary>
		/// <param name="urls">
		///		The list of URLs to choose from.
		/// </param>
		/// <param name="excludeUrl">
		///		A url to be excluded from the selection.
		/// </param>
		/// <returns>
		///		A random URL from the given list of URLs.
		/// </returns>
		private static string GetRandomUrl(IEnumerable<string> urls, string excludeUrl = "")
		{
			// Convert the enumerable to a list
			List<string> urlList = urls.ToList();

			// Remove the excluded url if required
			if (!string.IsNullOrEmpty(excludeUrl))
			{
				urlList.Remove(excludeUrl);

				// Check we haven't lost all URLs
				if (!urlList.Any())
				{
					// Todo: Figure out what to properly do here. It's possible we could end up having just one URL
					// available
					// throw new Exception($"Asked to exclude \"{excludeUrl}\", but it's the only available URL in the list.");
					return excludeUrl;
				}
			}

			// Take a random index
			Random random = new Random();
			int index = random.Next(0, urlList.Count - 1);

			// Return the random url
			return urlList[index];
		}
	}
}
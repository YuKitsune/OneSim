namespace OneSim.Map.Infrastructure
{
	/// <summary>
	/// 	The Status Data Provider settings.
	/// </summary>
	public class StatusDataProviderSettings
	{
		/// <summary>
		/// 	Gets or sets the URL at which the other URLs to full data sets can be found.
		/// </summary>
		public string RootStatusUrl { get; set; }

		/// <summary>
		/// 	Gets or sets the amount of time to wait before refreshing the root status URLs.
		/// </summary>
		public int MinutesBeforeRootRefresh { get; set; }
	}
}
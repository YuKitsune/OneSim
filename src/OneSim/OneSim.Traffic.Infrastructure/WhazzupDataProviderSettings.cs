namespace OneSim.Traffic.Infrastructure
{
	/// <summary>
	/// 	The Whazzup Data Provider settings.
	/// </summary>
	public class WhazzupDataProviderSettings
	{
		/// <summary>
		/// 	Gets or sets the URL at which the status file can be found.
		/// 	The Status file contains URLs to mirrors where full data files can be found containing traffic data.
		/// </summary>
		public string StatusUrl { get; set; }

		/// <summary>
		/// 	Gets or sets the amount of minutes to wait before re-downloading the status file and refreshing the
		/// 	available traffic data URLs.
		/// </summary>
		public int MinutesBeforeStatusRefresh { get; set; }
	}
}
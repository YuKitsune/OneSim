namespace OneSim.Api.Map
{
	using Newtonsoft.Json;
	using Newtonsoft.Json.Converters;

	using OneSim.Map.Domain.Entities;

	/// <summary>
	/// 	The OneSim Map API Settings.
	/// </summary>
	public class MapApiSettings
	{
		/// <summary>
		/// 	Gets or sets the <see cref="NetworkType"/> which the current Map API instance is viewing.
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		public NetworkType TargetNetwork { get; set; }

		/// <summary>
		/// 	Gets or sets the amount of minutes before refreshing the status data.
		/// </summary>
		public int DataRefreshInterval { get; set; }
	}
}
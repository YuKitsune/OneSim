namespace OneSim.Map.Application.Abstractions
{
	using System.Threading.Tasks;

	/// <summary>
	/// 	The interface for providing (by any means) traffic data.
	/// </summary>
	public interface ITrafficDataProvider
	{
		/// <summary>
		/// 	Gets the online traffic data.
		/// </summary>
		/// <returns>
		///		The <see cref="TrafficDataFetchResult"/>.
		/// </returns>
		TrafficDataFetchResult GetTrafficData();

		/// <summary>
		/// 	Gets the online traffic data as an asynchronous operation.
		/// </summary>
		/// <returns>
		///		The <see cref="TrafficDataFetchResult"/>.
		/// </returns>
		Task<TrafficDataFetchResult> GetTrafficDataAsync();
	}
}
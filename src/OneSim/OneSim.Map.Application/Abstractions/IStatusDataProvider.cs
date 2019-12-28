namespace OneSim.Map.Application.Abstractions
{
	using System.Threading.Tasks;

	/// <summary>
	/// 	The Online Flight Simulation Network status data provider.
	/// </summary>
	public interface IStatusDataProvider
	{
		/// <summary>
		/// 	Gets the status data.
		/// </summary>
		/// <returns>
		///		The <see cref="StatusDownloadResult"/>.
		/// </returns>
		StatusDownloadResult GetStatusData();

		/// <summary>
		/// 	Gets the status data as an asynchronous operation.
		/// </summary>
		/// <returns>
		///		The <see cref="StatusDownloadResult"/>.
		/// </returns>
		Task<StatusDownloadResult> GetStatusDataAsync();
	}
}
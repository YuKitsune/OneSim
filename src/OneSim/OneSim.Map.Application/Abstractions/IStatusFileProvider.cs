namespace OneSim.Map.Application.Abstractions
{
	using System.Threading.Tasks;

	/// <summary>
	/// 	The status file provider.
	/// </summary>
	public interface IStatusFileProvider
	{
		/// <summary>
		/// 	Gets the status file.
		/// </summary>
		/// <returns>
		///		The <see cref="StatusFileDownloadResult"/>.
		/// </returns>
		StatusFileDownloadResult GetStatusFile();

		/// <summary>
		/// 	Gets the status file as an asynchronous operation.
		/// </summary>
		/// <returns>
		///		The <see cref="StatusFileDownloadResult"/>.
		/// </returns>
		Task<StatusFileDownloadResult> GetStatusFileAsync();
	}
}
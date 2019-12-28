namespace OneSim.Map.Application.Abstractions
{
	/// <summary>
	/// 	The Online Flight Simulation Network status data parser interface.
	/// </summary>
	public interface IStatusDataParser
	{
		/// <summary>
		/// 	Parses the given <see cref="string"/> as set of status data.
		/// </summary>
		/// <param name="rawStatusFile">
		///		The raw status data.
		/// </param>
		/// <returns>
		///		The <see cref="StatusParseResult"/>.
		/// </returns>
		StatusParseResult Parse(string rawStatusFile);
	}
}
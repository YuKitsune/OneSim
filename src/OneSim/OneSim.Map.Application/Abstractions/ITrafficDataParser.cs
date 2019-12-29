namespace OneSim.Map.Application.Abstractions
{
	/// <summary>
	/// 	The interface for parsing data containing online traffic data for a specific Online Flight Simulation Network.
	/// </summary>
	public interface ITrafficDataParser
	{
		/// <summary>
		/// 	Parses the given <see cref="string"/> as online traffic data.
		/// </summary>
		/// <param name="rawStatusFile">
		///		The online traffic data in the form of a <see cref="string"/>..
		/// </param>
		/// <returns>
		///		The <see cref="TrafficDataParseResult"/>.
		/// </returns>
		TrafficDataParseResult Parse(string rawStatusFile);
	}
}
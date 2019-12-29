namespace OneSim.Traffic.Application.Abstractions
{
	/// <summary>
	/// 	The interface for parsing data containing online traffic data.
	/// </summary>
	public interface ITrafficDataParser
	{
		/// <summary>
		/// 	Parses the given <see cref="string"/> as online traffic data.
		/// </summary>
		/// <param name="trafficData">
		///		The online traffic data in the form of a <see cref="string"/>..
		/// </param>
		/// <returns>
		///		The <see cref="TrafficDataParseResult"/>.
		/// </returns>
		TrafficDataParseResult Parse(string trafficData);
	}
}
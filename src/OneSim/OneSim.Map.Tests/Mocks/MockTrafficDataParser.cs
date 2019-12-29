namespace OneSim.Map.Tests.Mocks
{
	using Newtonsoft.Json;

	using OneSim.Map.Application;
	using OneSim.Map.Application.Abstractions;

	/// <summary>
	/// 	The mock <see cref="ITrafficDataParser"/>.
	/// </summary>
	public class MockTrafficDataParser : ITrafficDataParser
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
		public TrafficDataParseResult Parse(string trafficData) => JsonConvert.DeserializeObject<TrafficDataParseResult>(trafficData);
	}
}
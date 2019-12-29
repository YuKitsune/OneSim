namespace OneSim.Map.Tests.Mocks
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using Newtonsoft.Json;

	using OneSim.Map.Application;
	using OneSim.Map.Application.Abstractions;
	using OneSim.Map.Domain.Entities;

	/// <summary>
	/// 	The mock <see cref="ITrafficDataProvider"/>.
	/// </summary>
	public class MockTrafficDataProvider : ITrafficDataProvider
	{
		/// <summary>
		/// 	Gets the <see cref="List{T}"/> of <see cref="Pilot"/> to return.
		/// </summary>
		public List<Pilot> Pilots { get; }

		/// <summary>
		/// 	Gets the <see cref="List{T}"/> of <see cref="AirTrafficController"/>s to return
		/// </summary>
		public List<AirTrafficController> Controllers { get; }

		/// <summary>
		/// 	Gets the <see cref="List{T}"/> of <see cref="FlightNotification"/>s to return.
		/// </summary>
		public List<FlightNotification> FlightNotifications { get; }

		/// <summary>
		/// 	Gets the <see cref="List{T}"/> of <see cref="Server"/> to return.
		/// </summary>
		public List<Server> Servers { get; }

		/// <summary>
		/// 	Gets the online traffic data.
		/// </summary>
		/// <returns>
		///		The <see cref="TrafficDataFetchResult"/>.
		/// </returns>
		public TrafficDataFetchResult GetTrafficData() => GetTrafficDataAsync().GetAwaiter().GetResult();

		/// <summary>
		/// 	Gets the online traffic data as an asynchronous operation.
		/// </summary>
		/// <returns>
		///		The <see cref="TrafficDataFetchResult"/>.
		/// </returns>
		public async Task<TrafficDataFetchResult> GetTrafficDataAsync()
		{
			await Task.Yield();
			TrafficDataParseResult result = new TrafficDataParseResult();
			result.Pilots.AddRange(Pilots);
			result.Controllers.AddRange(Controllers);
			result.FlightNotifications.AddRange(FlightNotifications);
			result.Servers.AddRange(Servers);

			return new TrafficDataFetchResult(JsonConvert.SerializeObject(result),
											  "TEST",
											  DateTime.UtcNow,
											  TimeSpan.FromSeconds(1));
		}
	}
}
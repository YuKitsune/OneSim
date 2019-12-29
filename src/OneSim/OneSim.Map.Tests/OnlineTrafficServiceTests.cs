namespace OneSim.Map.Tests
{
	using System.Threading.Tasks;

	using NUnit.Framework;

	using OneSim.Map.Application;
	using OneSim.Map.Application.Abstractions;
	using OneSim.Map.Domain.Entities;
	using OneSim.Map.Tests.Mocks;

	/// <summary>
	/// 	The <see cref="OnlineTrafficService"/> Tests.
	/// </summary>
	[TestFixture]
	public class OnlineTrafficServiceTests
	{
		/// <summary>
		/// 	Ensures all the traffic data is correctly updated.
		/// </summary>
		/// <returns>
		///		The <see cref="Task"/>
		/// </returns>
		[Test]
		public async Task DataGetsUpdated()
		{
			// Arrange
			MockTrafficDataProvider provider = new MockTrafficDataProvider();
			ITrafficDataParser parser = new MockTrafficDataParser();
			ITrafficDbContext trafficDbContext = MockHelpers.GetTrafficDbContext();
			IHistoricalDbContext historicalDbContext = MockHelpers.GetHistoricalDbContext();

			provider.Pilots.Add(new Pilot
								{
									Callsign = "Stingy Joe",
									
								});
			
			// Act
		}

		/// <summary>
		/// 	Ensures all data linked to a pilot (flight plan and history) is purged prior to updating the pilot data.
		/// </summary>
		/// <returns>
		///		The <see cref="Task"/>
		/// </returns>
		[Test]
		public async Task OldPilotDataIsPurgedOnUpdate()
		{
			
		}

		/// <summary>
		/// 	Ensures the history of a pilots position is copied to the updated pilot and a new entry with the latest
		/// 	position is created.
		/// </summary>
		/// <returns>
		///		The <see cref="Task"/>
		/// </returns>
		[Test]
		public async Task PilotHistoryIsUpdated()
		{
			
		}

		/// <summary>
		/// 	Ensures all data linked to a Flight Notification (flight plan) is purged prior to updating the
		/// 	Flight Notification data.
		/// </summary>
		/// <returns>
		///		The <see cref="Task"/>
		/// </returns>
		[Test]
		public async Task OldNotificationDataIsPurgedOnUpdate()
		{
			
		}
	}
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnlineTrafficServiceTests.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Tests.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    using Moq;

    using NUnit.Framework;

    using OneSim.Traffic.Application;
    using OneSim.Traffic.Application.Abstractions;
    using OneSim.Traffic.Domain.Entities;
    using OneSim.Traffic.Domain.ValueObjects;
    using OneSim.Traffic.Tests.Mocks;

    /// <summary>
    ///     The <see cref="OnlineTrafficService"/> Tests.
    /// </summary>
    [TestFixture]
    public class OnlineTrafficServiceTests
    {
        /// <summary>
        ///     Gets a <see cref="Pilot"/> with a <see cref="FlightPlan"/>.
        /// </summary>
        private Pilot PilotWithFlightPlan => new Pilot
                                             {
                                                 Callsign = "STG101",
                                                 NetworkId = "1234567",
                                                 Name = "Stingy Joe",
                                                 Location =
                                                     new Point3D(
                                                         0,
                                                         0,
                                                         37000),
                                                 GroundSpeed = 200,
                                                 Heading = 0,
                                                 Squawk = new SquawkCode("1234"),
                                                 FlightPlan = new FlightPlan
                                                              {
                                                                  FlightRules = FlightPlanRules.InstrumentFlightRules,
                                                                  AircraftType = "B738",
                                                                  TrueAirSpeed = "200",
                                                                  Altitude = 37000,
                                                                  DepartureIcao = "YBBN",
                                                                  ArrivalIcao = "YSSY",
                                                                  ScheduledDepartureTime =
                                                                      new DateTime(
                                                                          2019,
                                                                          12,
                                                                          25,
                                                                          1,
                                                                          2,
                                                                          0,
                                                                          DateTimeKind.Utc),
                                                                  EstimatedEnrouteTime = TimeSpan.FromHours(1),
                                                                  Endurance = TimeSpan.FromHours(1.5),
                                                                  Route = "DCT",
                                                                  Remarks = "I don't care about remarks"
                                                              }
                                             };

        /// <summary>
        ///     Gets a <see cref="Pilot"/> without a <see cref="FlightPlan"/>.
        /// </summary>
        private Pilot PilotWithoutFlightPlan => new Pilot
                                                {
                                                    Callsign = "STG102",
                                                    NetworkId = "1234568",
                                                    Name = "Stingy Sally",
                                                    Location =
                                                        new Point3D(
                                                            1,
                                                            1,
                                                            36000),
                                                    GroundSpeed = 200,
                                                    Heading = 0,
                                                    Squawk = new SquawkCode("1235")
                                                };

        /// <summary>
        ///     Gets an <see cref="AirTrafficController"/>.
        /// </summary>
        private AirTrafficController Controller => new AirTrafficController
                                                   {
                                                       Callsign = "BN-DOS_CTR",
                                                       NetworkId = "1234569",
                                                       Name = "Stingy Joe",
                                                       Frequency = "123.450",
                                                       Rating = ControllerRating.Controller1,
                                                       FacilityType = ControllerFacilityType.Centre,
                                                       VisibilityRange = 600,
                                                       Atis = "Whatever dude"
                                                   };

        /// <summary>
        ///     Gets a <see cref="FlightNotification"/>.
        /// </summary>
        private FlightNotification Notification => new FlightNotification
                                                   {
                                                       Callsign = "STG103",
                                                       Name = "Stingy Dude",
                                                       NetworkId = "1234570",
                                                       FlightPlan = new FlightPlan
                                                                    {
                                                                        FlightRules =
                                                                            FlightPlanRules.InstrumentFlightRules,
                                                                        AircraftType = "B738",
                                                                        TrueAirSpeed = "200",
                                                                        Altitude = 38000,
                                                                        DepartureIcao = "YSSY",
                                                                        ArrivalIcao = "YBBN",
                                                                        ScheduledDepartureTime =
                                                                            new DateTime(
                                                                                2019,
                                                                                12,
                                                                                25,
                                                                                2,
                                                                                3,
                                                                                0,
                                                                                DateTimeKind.Utc),
                                                                        EstimatedEnrouteTime = TimeSpan.FromHours(1),
                                                                        Endurance = TimeSpan.FromHours(1.5),
                                                                        Route = "DCT",
                                                                        Remarks = "I also don't care about remarks"
                                                                    }
                                                   };

        /// <summary>
        ///     Gets a <see cref="Server"/>.
        /// </summary>
        private Server Server => new Server
                                 {
                                     NetworkIdentifier = "FAKE",
                                     Name = "Some fake server",
                                     IpAddress = "123.456.789.101",
                                     Location = "123 Fake St."
                                 };

        /// <summary>
        ///     Ensures all the traffic data is correctly updated.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task DataGetsUpdated()
        {
            // Todo: The assertion section of this gets messy, it's not production code, so it could stay that way
            //     Maybe clean it up at some stage.

            // Arrange
            MockTrafficDataProvider provider = new MockTrafficDataProvider();
            ITrafficDataParser parser = new MockTrafficDataParser();
            ITrafficDbContext trafficDbContext = MockHelpers.GetTrafficDbContext();
            IHistoricalDbContext historicalDbContext = MockHelpers.GetHistoricalDbContext();
            MockTrafficNotifier notifier = new MockTrafficNotifier();
            OnlineTrafficService service = new OnlineTrafficService(
                provider,
                parser,
                trafficDbContext,
                historicalDbContext,
                notifier,
                new Mock<ILogger<OnlineTrafficService>>().Object);

            // Seed the initial data
            provider.Pilots.Add(PilotWithFlightPlan);
            provider.Pilots.Add(PilotWithoutFlightPlan);
            provider.Controllers.Add(Controller);
            provider.FlightNotifications.Add(Notification);
            provider.Servers.Add(Server);

            // Act
            await service.UpdateTrafficDataAsync();

            // Assert
            Assert.AreEqual(2, trafficDbContext.Pilots.Count());
            Assert.AreEqual(1, trafficDbContext.FlightNotifications.Count());
            Assert.AreEqual(2, trafficDbContext.FlightPlans.Count());
            Assert.AreEqual(1, trafficDbContext.Controllers.Count());
            Assert.AreEqual(1, trafficDbContext.Servers.Count());
            Assert.AreEqual(1, notifier.NotificationCount);

            // Check our pilots have the correct history records
            Pilot pilotWithPlan = await trafficDbContext.Pilots
                                                        .Include(p => p.FlightPlan)
                                                        .Include(p => p.History)
                                                        .FirstOrDefaultAsync(
                                                             p => p.Callsign ==
                                                                  PilotWithFlightPlan.Callsign);

            Assert.IsTrue(pilotWithPlan.FlightPlanFiled);
            Assert.IsNotNull(pilotWithPlan.FlightPlan);
            Point3D pilotWithPlanPoint = pilotWithPlan.History.First();
            Assert.IsNotNull(pilotWithPlanPoint);
            Assert.AreEqual(pilotWithPlanPoint.Latitude, pilotWithPlan.Location.Latitude);
            Assert.AreEqual(pilotWithPlanPoint.Longitude, pilotWithPlan.Location.Longitude);
            Assert.AreEqual(pilotWithPlanPoint.Altitude, pilotWithPlan.Location.Altitude);

            // Check the pilot without a plan
            Pilot pilotWithoutPlan = await trafficDbContext.Pilots
                                                           .Include(p => p.FlightPlan)
                                                           .Include(p => p.History)
                                                           .FirstOrDefaultAsync(
                                                                p => p.Callsign ==
                                                                     PilotWithoutFlightPlan.Callsign);
            Assert.IsFalse(pilotWithoutPlan.FlightPlanFiled);
            Assert.IsNull(pilotWithoutPlan.FlightPlan);
            Point3D pilotWithoutPlanPoint = pilotWithoutPlan.History.First();
            Assert.IsNotNull(pilotWithoutPlanPoint);
            Assert.AreEqual(pilotWithoutPlanPoint.Latitude, pilotWithoutPlan.Location.Latitude);
            Assert.AreEqual(pilotWithoutPlanPoint.Longitude, pilotWithoutPlan.Location.Longitude);
            Assert.AreEqual(pilotWithoutPlanPoint.Altitude, pilotWithoutPlan.Location.Altitude);

            // Update pilot positions
            provider.Pilots.Clear();

            Pilot updatedPilotWithPlan = PilotWithFlightPlan;
            updatedPilotWithPlan.Location = new Point3D(1, 1, 35000);
            provider.Pilots.Add(updatedPilotWithPlan);

            Pilot updatedPilotWithoutPlan = PilotWithoutFlightPlan;
            updatedPilotWithoutPlan.Location = new Point3D(1, 1, 34000);
            provider.Pilots.Add(updatedPilotWithoutPlan);

            // Act
            service = new OnlineTrafficService(
                provider,
                parser,
                trafficDbContext,
                historicalDbContext,
                notifier,
                new Mock<ILogger<OnlineTrafficService>>().Object);
            await service.UpdateTrafficDataAsync();

            // Assert
            Assert.AreEqual(2, trafficDbContext.Pilots.Count());
            Assert.AreEqual(1, trafficDbContext.FlightNotifications.Count());
            Assert.AreEqual(2, trafficDbContext.FlightPlans.Count());
            Assert.AreEqual(1, trafficDbContext.Controllers.Count());
            Assert.AreEqual(1, trafficDbContext.Servers.Count());
            Assert.AreEqual(2, notifier.NotificationCount);

            // Check our pilots have the correct history records
            pilotWithPlan = await trafficDbContext.Pilots
                                                  .Include(p => p.FlightPlan)
                                                  .Include(p => p.History)
                                                  .FirstOrDefaultAsync(
                                                       p => p.Callsign ==
                                                            PilotWithFlightPlan.Callsign);

            Assert.IsTrue(pilotWithPlan.FlightPlanFiled);
            Assert.IsNotNull(pilotWithPlan.FlightPlan);
            pilotWithPlanPoint = pilotWithPlan.History.OrderBy(h => h.DateTime).ToList()[1];
            Assert.IsNotNull(pilotWithPlanPoint);
            Assert.AreEqual(pilotWithPlanPoint.Latitude, pilotWithPlan.Location.Latitude);
            Assert.AreEqual(pilotWithPlanPoint.Longitude, pilotWithPlan.Location.Longitude);
            Assert.AreEqual(pilotWithPlanPoint.Altitude, pilotWithPlan.Location.Altitude);

            // Check the pilot without a plan
            pilotWithoutPlan = await trafficDbContext.Pilots
                                                     .Include(p => p.FlightPlan)
                                                     .Include(p => p.History)
                                                     .FirstOrDefaultAsync(
                                                          p => p.Callsign ==
                                                               PilotWithoutFlightPlan.Callsign);
            Assert.IsFalse(pilotWithoutPlan.FlightPlanFiled);
            Assert.IsNull(pilotWithoutPlan.FlightPlan);
            pilotWithoutPlanPoint = pilotWithoutPlan.History.OrderBy(h => h.DateTime).ToList()[1];
            Assert.IsNotNull(pilotWithoutPlanPoint);
            Assert.AreEqual(pilotWithoutPlanPoint.Latitude, pilotWithoutPlan.Location.Latitude);
            Assert.AreEqual(pilotWithoutPlanPoint.Longitude, pilotWithoutPlan.Location.Longitude);
            Assert.AreEqual(pilotWithoutPlanPoint.Altitude, pilotWithoutPlan.Location.Altitude);
        }

        /// <summary>
        ///     Ensures all data linked to a pilot (flight plan and history) is purged prior to updating the pilot data.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task OldPilotDataIsPurgedOnUpdate()
        {
            // Arrange
            MockTrafficDataProvider provider = new MockTrafficDataProvider();
            ITrafficDataParser parser = new MockTrafficDataParser();
            ITrafficDbContext trafficDbContext = MockHelpers.GetTrafficDbContext();
            IHistoricalDbContext historicalDbContext = MockHelpers.GetHistoricalDbContext();
            MockTrafficNotifier notifier = new MockTrafficNotifier();
            OnlineTrafficService service = new OnlineTrafficService(
                provider,
                parser,
                trafficDbContext,
                historicalDbContext,
                notifier,
                new Mock<ILogger<OnlineTrafficService>>().Object);

            // Seed the initial data
            provider.Pilots.Add(PilotWithFlightPlan);
            provider.Pilots.Add(PilotWithoutFlightPlan);

            // Act
            await service.UpdateTrafficDataAsync();

            // Assert
            Assert.AreEqual(2, trafficDbContext.Pilots.Count());
            Assert.AreEqual(1, trafficDbContext.FlightPlans.Count());

            // Clear the provider data
            provider.Pilots.Clear();
            provider.Pilots.Clear();

            // Act
            service = new OnlineTrafficService(
                provider,
                parser,
                trafficDbContext,
                historicalDbContext,
                notifier,
                new Mock<ILogger<OnlineTrafficService>>().Object);
            await service.UpdateTrafficDataAsync();

            // Assert
            Assert.AreEqual(0, trafficDbContext.Pilots.Count());
            Assert.AreEqual(0, trafficDbContext.FlightPlans.Count());
        }

        /// <summary>
        ///     Ensures all data linked to a Flight Notification (flight plan) is purged prior to updating the
        ///     Flight Notification data.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task OldNotificationDataIsPurgedOnUpdate()
        {
            // Arrange
            MockTrafficDataProvider provider = new MockTrafficDataProvider();
            ITrafficDataParser parser = new MockTrafficDataParser();
            ITrafficDbContext trafficDbContext = MockHelpers.GetTrafficDbContext();
            IHistoricalDbContext historicalDbContext = MockHelpers.GetHistoricalDbContext();
            MockTrafficNotifier notifier = new MockTrafficNotifier();
            OnlineTrafficService service = new OnlineTrafficService(
                provider,
                parser,
                trafficDbContext,
                historicalDbContext,
                notifier,
                new Mock<ILogger<OnlineTrafficService>>().Object);

            // Seed the initial data
            provider.Pilots.Add(PilotWithFlightPlan);
            provider.FlightNotifications.Add(Notification);

            // Act
            await service.UpdateTrafficDataAsync();

            // Assert
            Assert.AreEqual(1, trafficDbContext.Pilots.Count());
            Assert.AreEqual(1, trafficDbContext.FlightNotifications.Count());
            Assert.AreEqual(2, trafficDbContext.FlightPlans.Count());

            // Remove the notification
            provider.FlightNotifications.Clear();

            // Act
            service = new OnlineTrafficService(
                provider,
                parser,
                trafficDbContext,
                historicalDbContext,
                notifier,
                new Mock<ILogger<OnlineTrafficService>>().Object);
            await service.UpdateTrafficDataAsync();

            // Assert
            Assert.AreEqual(1, trafficDbContext.Pilots.Count());
            Assert.AreEqual(0, trafficDbContext.FlightNotifications.Count());
            Assert.AreEqual(1, trafficDbContext.FlightPlans.Count());
        }
    }
}

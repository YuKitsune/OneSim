// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MockTrafficDataProvider.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Tests.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    using OneSim.Traffic.Application;
    using OneSim.Traffic.Application.Abstractions;
    using OneSim.Traffic.Domain.Entities;

    /// <summary>
    ///     The mock <see cref="ITrafficDataProvider"/>.
    /// </summary>
    public class MockTrafficDataProvider : ITrafficDataProvider
    {
        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="Pilot"/> to return.
        /// </summary>
        public List<Pilot> Pilots { get; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="AirTrafficController"/>s to return.
        /// </summary>
        public List<AirTrafficController> Controllers { get; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="FlightNotification"/>s to return.
        /// </summary>
        public List<FlightNotification> FlightNotifications { get; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="Server"/> to return.
        /// </summary>
        public List<Server> Servers { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MockTrafficDataProvider"/> class.
        /// </summary>
        public MockTrafficDataProvider()
        {
            Pilots = new List<Pilot>();
            Controllers = new List<AirTrafficController>();
            FlightNotifications = new List<FlightNotification>();
            Servers = new List<Server>();
        }

        /// <summary>
        ///     Gets the online traffic data.
        /// </summary>
        /// <returns>
        ///        The <see cref="TrafficDataFetchResult"/>.
        /// </returns>
        public TrafficDataFetchResult GetTrafficData() => GetTrafficDataAsync().GetAwaiter().GetResult();

        /// <summary>
        ///     Gets the online traffic data as an asynchronous operation.
        /// </summary>
        /// <returns>
        ///        The <see cref="TrafficDataFetchResult"/>.
        /// </returns>
        public async Task<TrafficDataFetchResult> GetTrafficDataAsync()
        {
            await Task.Yield();
            TrafficDataParseResult result = new TrafficDataParseResult();
            result.Pilots.AddRange(Pilots);
            result.Controllers.AddRange(Controllers);
            result.FlightNotifications.AddRange(FlightNotifications);
            result.Servers.AddRange(Servers);

            return new TrafficDataFetchResult(
                JsonConvert.SerializeObject(result),
                "TEST",
                DateTime.UtcNow,
                TimeSpan.FromSeconds(1));
        }
    }
}

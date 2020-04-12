// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrafficDataFetchResult.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Application
{
    using System;

    /// <summary>
    ///     The results from fetching a set of traffic data.
    /// </summary>
    public class TrafficDataFetchResult
    {
        /// <summary>
        ///     Gets the <see cref="string"/> representing the traffic data.
        /// </summary>
        public string TrafficData { get; }

        /// <summary>
        ///     Gets the <see cref="string"/> representing the source of the traffic data.
        /// </summary>
        public string Source { get; }

        /// <summary>
        ///     Gets the <see cref="DateReceived"/> at which the traffic data was downloaded.
        /// </summary>
        public DateTime DateReceived { get; }

        /// <summary>
        ///     Gets the <see cref="TimeSpan"/> taken to fetch the traffic data.
        /// </summary>
        public TimeSpan FetchTime { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TrafficDataFetchResult"/> class.
        /// </summary>
        /// <param name="trafficData">
        ///        The <see cref="string"/> representing the traffic data.
        /// </param>
        /// <param name="source">
        ///        The <see cref="string"/> representing the source of the <paramref name="trafficData"/>.
        /// </param>
        /// <param name="dateReceived">
        ///        The <see cref="DateTime"/> at which the <paramref name="trafficData"/> was received.
        /// </param>
        /// <param name="fetchTime">
        ///        The <see cref="TimeSpan"/> taken to fetch the <paramref name="trafficData"/>.
        /// </param>
        public TrafficDataFetchResult(
            string trafficData,
            string source,
            DateTime dateReceived,
            TimeSpan fetchTime)
        {
            if (string.IsNullOrEmpty(source)) throw new ArgumentNullException(nameof(source), "The Source cannot be null or empty.");
            if (dateReceived == default) throw new ArgumentNullException(nameof(dateReceived), "The Date Received cannot be the default DateTime value.");
            if (fetchTime == null ||
                fetchTime == default)
                throw new ArgumentNullException(nameof(fetchTime), "The Fetch Time cannot be null or the default TimeSpan value.");

            TrafficData = trafficData;
            Source = source;
            DateReceived = dateReceived;
            FetchTime = fetchTime;
        }
    }
}

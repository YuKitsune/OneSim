// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrafficDataArchiveEntry.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities
{
    using System;

    /// <summary>
    ///     The object representing an archived set of traffic data.
    /// </summary>
    public class TrafficDataArchiveEntry
    {
        /// <summary>
        ///     Gets or sets the ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DateReceived"/> at which the traffic data was downloaded.
        /// </summary>
        public DateTime DateReceived { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="TimeSpan"/> taken to fetch the traffic data.
        /// </summary>
        public TimeSpan FetchTime { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="string"/> representing the source of the traffic data.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="string"/> representing the raw status file.
        /// </summary>
        public string TrafficData { get; set; }
    }
}

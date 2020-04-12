// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrafficApiSettings.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Map
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using OneSim.Traffic.Domain.Entities;

    /// <summary>
    ///     The Traffic API Settings.
    /// </summary>
    public class TrafficApiSettings
    {
        /// <summary>
        ///     Gets or sets the <see cref="NetworkType"/> which the current Traffic API instance is using.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public NetworkType TargetNetwork { get; set; }

        /// <summary>
        ///     Gets or sets the amount of minutes before refreshing the traffic data.
        /// </summary>
        public int DataRefreshInterval { get; set; }
    }
}

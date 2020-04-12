// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrafficDataParseResult.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Application
{
    using System.Collections.Generic;

    using OneSim.Traffic.Domain.Entities;

    /// <summary>
    ///     Represents the resulting data from parsing traffic data.
    /// </summary>
    public class TrafficDataParseResult
    {
        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="Pilot"/>s.
        /// </summary>
        public List<Pilot> Pilots { get; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="AirTrafficController"/>s.
        /// </summary>
        public List<AirTrafficController> Controllers { get; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="FlightNotification"/>s.
        /// </summary>
        public List<FlightNotification> FlightNotifications { get; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="Server"/>s.
        /// </summary>
        public List<Server> Servers { get; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="TrafficDataParseError"/>s.
        /// </summary>
        public List<TrafficDataParseError> Errors { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TrafficDataParseResult"/> class.
        /// </summary>
        public TrafficDataParseResult()
        {
            Pilots = new List<Pilot>();
            Controllers = new List<AirTrafficController>();
            FlightNotifications = new List<FlightNotification>();
            Servers = new List<Server>();
            Errors = new List<TrafficDataParseError>();
        }
    }
}

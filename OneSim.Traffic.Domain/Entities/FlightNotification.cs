// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlightNotification.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities
{
    /// <summary>
    ///     The Flight Notification. Also known as a "prefile", or "pre-filed flight plan".
    /// </summary>
    public class FlightNotification
    {
        /// <summary>
        ///     Gets or sets the ID.
        /// </summary>
        /// <remarks>
        ///     This is the ID used for storing the current <see cref="FlightNotification"/> instance in a relational database.
        /// </remarks>
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the callsign.
        /// </summary>
        public string Callsign { get; set; }

        /// <summary>
        ///     Gets or sets the pilots OFSN ID.
        /// </summary>
        public string NetworkId { get; set; }

        /// <summary>
        ///     Gets or sets the pilots name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="FlightPlan"/>.
        /// </summary>
        public FlightPlan FlightPlan { get; set; }
    }
}

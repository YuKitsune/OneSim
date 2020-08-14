// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Airport.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities.Aeronautical
{
    using System.Collections.Generic;

    /// <summary>
    ///     The airport.
    /// </summary>
    public class Airport : Fix
    {
        /// <summary>
        ///     Gets or sets the IATA code of the current <see cref="Airport"/>.
        /// </summary>
        public string IataCode { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="AirspaceClass"/>.
        /// </summary>
        public AirspaceClass? Class { get; set; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="ControllerPosition"/>s available at the current
        ///     <see cref="Airport"/>.
        /// </summary>
        public List<ControllerPosition> ControllerPositions { get; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="Runway"/>.
        /// </summary>
        public List<Runway> Runways { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Airport"/> class.
        /// </summary>
        /// <param name="identifier">
        ///     The ICAO identifier.
        /// </param>
        /// <param name="location">
        ///     The <see cref="Coordinate"/> representing the location.
        /// </param>
        public Airport(string identifier, Coordinate location)
            : base(identifier, location)
        {
            ControllerPositions = new List<ControllerPosition>();
            Runways = new List<Runway>();
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControllerFacilityType.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities
{
    /// <summary>
    ///     The <see cref="AirTrafficController"/>s' Facility type.
    /// </summary>
    public enum ControllerFacilityType
    {
        /// <summary>
        ///     An observer.
        /// </summary>
        Observer = 0,

        /// <summary>
        ///     Flight Service Station.
        /// </summary>
        FlightServiceStation = 1,

        /// <summary>
        ///     Airways Clearance Delivery.
        /// </summary>
        Delivery = 2,

        /// <summary>
        ///     Ground.
        /// </summary>
        Ground = 3,

        /// <summary>
        ///     Tower.
        /// </summary>
        Tower = 4,

        /// <summary>
        ///     Approach.
        /// </summary>
        Approach = 5,

        /// <summary>
        ///     Centre.
        /// </summary>
        Centre = 6,

        /// <summary>
        ///     Departure.
        /// </summary>
        Departure = 7
    }
}

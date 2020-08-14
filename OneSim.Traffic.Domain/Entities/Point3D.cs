// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Point3D.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities
{
    using System;

    /// <summary>
    ///     A 3 dimensional (<see cref="Point2D"/> + Altitude) point in space.
    /// </summary>
    public class Point3D : Point2D
    {
        /// <summary>
        ///     Gets or sets the altitude in feet (ft).
        /// </summary>
        public int Altitude { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Point3D"/> class.
        /// </summary>
        /// <param name="latitude">
        ///     The latitude.
        /// </param>
        /// <param name="longitude">
        ///     The longitude.
        /// </param>
        /// <param name="altitude">
        ///     The altitude.
        /// </param>
        public Point3D(double latitude, double longitude, int altitude)
            : base(latitude, longitude)
        {
            if (altitude < 0) throw new ArgumentException("The {nameof(altitude)} cannot be less than 0.", nameof(altitude));

            Altitude = altitude;
        }
    }
}

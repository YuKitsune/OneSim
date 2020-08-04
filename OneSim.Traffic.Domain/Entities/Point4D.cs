// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Point4D.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities
{
    using System;

    /// <summary>
    ///     A <see cref="Point3D"/> with a <see cref="DateTime"/>.
    /// </summary>
    public class Point4D : Point3D
    {
        /// <summary>
        ///     Gets or sets the <see cref="DateTime"/> at which the owner of the current <see cref="Point4D"/> was
        ///     located at <see cref="Point4D"/>.
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Point4D"/> class.
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
        /// <param name="dateTime">
        ///     The <see cref="DateTime"/> at which the owner of the <see cref="Point4D"/> was located at the
        ///     <see cref="Point4D"/>.
        /// </param>
        public Point4D(double latitude, double longitude, int altitude, DateTime dateTime)
            : base(latitude, longitude, altitude) => DateTime = dateTime;
    }
}

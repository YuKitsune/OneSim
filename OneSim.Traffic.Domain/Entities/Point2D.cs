// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Point2D.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities
{
    using System;

    /// <summary>
    ///     A 2 dimensional (Latitude/Longitude) point in space.
    /// </summary>
    public class Point2D
    {
        /// <summary>
        ///     Gets or sets the ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the latitude.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        ///     Gets or sets the longitude.
        /// </summary>
        public double Longitude { get; set; }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Point2D.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities
{
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

        /// <summary>
        ///     Initializes a new instance of the <see cref="Point2D"/> class.
        /// </summary>
        /// <param name="longitude">
        ///     The longitude.
        /// </param>
        /// <param name="latitude">
        ///     The latitude.
        /// </param>
        public Point2D(double longitude = 0, double latitude = 0)
        {
            Longitude = longitude;
            Latitude = latitude;
        }
    }
}

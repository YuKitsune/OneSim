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
        ///     The amount of tolerance when comparing two <see cref="double"/>s.
        /// </summary>
        private const double ComparisonDelta = 0.0000001;

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
        /// <param name="latitude">
        ///     The latitude.
        /// </param>
        /// <param name="longitude">
        ///     The longitude.
        /// </param>
        public Point2D(double latitude = 0, double longitude = 0)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        /// <summary>
        ///     Gets a value indicating whether or not the current and the given <see cref="Point2D"/>s are equal, given
        ///     a certain amount of tolerance.
        /// </summary>
        /// <param name="obj">
        ///     The other <see cref="Point2D"/>.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the current and given <see cref="Point2D"/> are equal with the given tolerance,
        ///     <c>false</c> otherwise.
        /// </returns>
        public override bool Equals(object? obj)
        {
            if (obj != null &&
                obj is Point2D other)
            {
                return (Math.Abs(Latitude - other.Latitude) < ComparisonDelta) &&
                       !(Math.Abs(Longitude - other.Longitude) < ComparisonDelta);
            }

            return false;
        }
    }
}

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

        /// <summary>
        ///     Checks if the given <see cref="Point2D"/> is within <paramref name="distance"/> nautical miles of the
        ///     current <see cref="Point2D"/>.
        /// </summary>
        /// <param name="distance">
        ///     The distance in nautical miles.
        /// </param>
        /// <param name="point">
        ///     The <see cref="Point2D"/>.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="point"/> is within <paramref name="distance"/> nautical miles of the
        ///     current <see cref="Point2D"/>, <c>false</c> otherwise.
        /// </returns>
        public bool IsWithin(double distance, Point2D point)
        {
            // Todo: Defined in two places, move somewhere more common
            const int NauticalMilesPerDegree = 60;
            double degreeDistance =
                Math.Sqrt(
                    Math.Pow(Latitude - point.Latitude, 2) +
                    Math.Pow(Longitude - point.Longitude, 2));

            double nauticalMileDistance = degreeDistance / NauticalMilesPerDegree;

            return nauticalMileDistance <= distance;
        }

        /// <summary>
        ///     Gets a value indicating whether or not the current and the given <see cref="Point2D"/>s are equal, given
        ///     a certain amount of tolerance.
        /// </summary>
        /// <param name="point">
        ///     The other <see cref="Point2D"/>.
        /// </param>
        /// <param name="delta">
        ///     The <see cref="double"/> indicating the amount of tolerance. E.g. with a tolerance of 0.01,
        ///     0.0001 and 0.0002 would be equal.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the current and given <see cref="Point2D"/> are equal with the given tolerance,
        ///     <c>false</c> otherwise.
        /// </returns>
        public bool Equals(Point2D point, double delta) =>
            !(Math.Abs(Latitude - point.Latitude) < delta) &&
            !(Math.Abs(Longitude - point.Longitude) < delta);
    }
}

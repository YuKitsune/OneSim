// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Coordinate.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities.Ais
{
    using System;

    /// <summary>
    ///     The geographic coordinate.
    /// </summary>
    public class Coordinate
    {
        /// <summary>
        ///     Gets the <see cref="CoordinateComponent"/> representing the latitude.
        /// </summary>
        public CoordinateComponent Latitude { get; }

        /// <summary>
        ///     Gets the <see cref="CoordinateComponent"/> representing the longitude.
        /// </summary>
        public CoordinateComponent Longitude { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Coordinate"/> class.
        /// </summary>
        /// <param name="latitude">
        ///     The <see cref="CoordinateComponent"/> representing the latitude.
        /// </param>
        /// <param name="longitude">
        ///     The <see cref="CoordinateComponent"/> representing the longitude.
        /// </param>
        public Coordinate(CoordinateComponent latitude, CoordinateComponent longitude)
        {
            // Check for null
            if (latitude == null) throw new ArgumentNullException(nameof(latitude));
            if (longitude == null) throw new ArgumentNullException(nameof(longitude));

            // Check the axis is correct
            if (latitude.CardinalAxis != CardinalAxis.Latitude) throw new ArgumentException($"The {nameof(latitude)} must be on the North/South plane.", nameof(latitude));
            if (longitude.CardinalAxis != CardinalAxis.Longitude) throw new ArgumentException($"The {nameof(longitude)} does be on the East/West plane.", nameof(longitude));

            Latitude = latitude;
            Longitude = longitude;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Coordinate"/> class.
        /// </summary>
        /// <param name="latitude">
        ///     The <see cref="double"/> representing the latitude.
        /// </param>
        /// <param name="longitude">
        ///     The <see cref="double"/> representing the longitude.
        /// </param>
        public Coordinate(double latitude, double longitude)
        {
            Latitude = new CoordinateComponent(CardinalAxis.Latitude, latitude);
            Longitude = new CoordinateComponent(CardinalAxis.Longitude, longitude);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Coordinate"/> class.
        /// </summary>
        /// <param name="latitude">
        ///     The DMS formatted <see cref="string"/> representing the latitude.
        /// </param>
        /// <param name="longitude">
        ///     The DMS formatted <see cref="string"/> representing the longitude.
        /// </param>
        public Coordinate(string latitude, string longitude)
        {
            Latitude = CoordinateComponent.Parse(latitude);
            Longitude = CoordinateComponent.Parse(longitude);
        }

        /// <summary>
        ///     Gets the <see cref="Point2D"/> representing the current <see cref="Coordinate"/>.
        /// </summary>
        /// <returns>
        ///     The <see cref="Point2D"/> instance.
        /// </returns>
        public Point2D GetPoint() => new Point2D(Longitude.GetDecimal(), Latitude.GetDecimal());

        /// <summary>
        ///     Determines whether or not the <paramref name="obj"/> is equal to the current <see cref="Coordinate"/>
        ///     instance.
        /// </summary>
        /// <param name="obj">
        ///     The <see cref="object"/> to compare.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="obj"/> has the same value as the current <see cref="Coordinate"/>
        ///     instance, <c>false</c> otherwise.
        /// </returns>
        public override bool Equals(object? obj)
        {
            if (obj != null &&
                obj is Coordinate coordinate)
                return Latitude.Equals(coordinate.Latitude) && Longitude.Equals(coordinate.Longitude);

            // If we made it to here, then we don't have a match
            return false;
        }

        /// <summary>
        ///     Gets the <see cref="string"/> representation of the current <see cref="Coordinate"/>.
        /// </summary>
        /// <returns>
        ///     The <see cref="string"/>.
        /// </returns>
        public override string ToString() => $"{Latitude} {Longitude}";
    }
}

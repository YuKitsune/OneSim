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
            Latitude = latitude ?? throw new ArgumentNullException(nameof(latitude));
            Longitude = longitude ?? throw new ArgumentNullException(nameof(longitude));
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
    }
}

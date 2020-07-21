// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Runway.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities.Ais
{
    using System;

    /// <summary>
    ///     The runway.
    /// </summary>
    public class Runway : SectorSetSpecificEntity
    {
        /// <summary>
        ///     Gets or sets the ID of the current <see cref="Runway"/>.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the identifier.
        ///     E.g. 16L.
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        ///     Gets or sets the threshold position represented by a <see cref="Point2D"/>.
        /// </summary>
        public Point2D ThresholdLocation { get; set; }

        // Todo: Do we want the length?

        /// <summary>
        ///     Gets or sets the heading.
        /// </summary>
        public int Heading { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Runway"/> class.
        /// </summary>
        /// <param name="identifier">
        ///     The ICAO identifier.
        /// </param>
        /// <param name="thresholdLocation">
        ///     The <see cref="Point2D"/> representing the location.
        /// </param>
        /// <param name="heading">
        ///     The heading.
        /// </param>
        public Runway(string identifier, Point2D thresholdLocation, int heading)
        {
            if (string.IsNullOrEmpty(identifier)) throw new ArgumentNullException(nameof(identifier));
            if (thresholdLocation == null) throw new ArgumentNullException(nameof(thresholdLocation));
            Identifier = identifier;
            ThresholdLocation = thresholdLocation;
            Heading = heading;
        }
    }
}

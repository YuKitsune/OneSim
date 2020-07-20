// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Runway.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities.Ais
{
    /// <summary>
    ///     The runway.
    /// </summary>
    public class Runway : SectorSetSpecific
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
    }
}

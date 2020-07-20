// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Fix.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities.Ais
{
    /// <summary>
    ///     The navigation fix.
    /// </summary>
    public class Fix : SectorSetSpecific
    {
        /// <summary>
        ///     Gets or sets the ID of the current <see cref="Fix"/>.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the ICAO identifier for the current <see cref="Fix"/>.
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="Point2D"/> representing the current <see cref="Fix"/>es location.
        /// </summary>
        public Point2D Location { get; set; }
    }
}

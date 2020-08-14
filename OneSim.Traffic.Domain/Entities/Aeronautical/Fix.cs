// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Fix.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities.Aeronautical
{
    using System;
    using System.Diagnostics;

    /// <summary>
    ///     The navigation fix.
    /// </summary>
    [DebuggerDisplay("{Identifier}")]
    public class Fix : SectorSetSpecificEntity
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
        ///     Gets or sets the <see cref="Coordinate"/> representing the current <see cref="Fix"/>es location.
        /// </summary>
        public Coordinate Location { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Fix"/> class.
        /// </summary>
        /// <param name="identifier">
        ///     The ICAO identifier.
        /// </param>
        /// <param name="location">
        ///     The <see cref="Coordinate"/> representing the location.
        /// </param>
        public Fix(string identifier, Coordinate location)
        {
            if (string.IsNullOrEmpty(identifier)) throw new ArgumentNullException(nameof(identifier));
            if (location == null) throw new ArgumentNullException(nameof(location));
            Identifier = identifier;
            Location = location;
        }
    }
}

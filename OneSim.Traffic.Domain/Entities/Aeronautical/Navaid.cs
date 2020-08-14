// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Navaid.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities.Aeronautical
{
    /// <summary>
    ///     The radio-based navigation aid.
    /// </summary>
    public class Navaid : Fix
    {
        /// <summary>
        ///     Gets or sets the <see cref="NavaidType"/>.
        /// </summary>
        public NavaidType Type { get; set; }

        /// <summary>
        ///     Gets or sets the frequency of the current <see cref="Navaid"/> multiplied by 1000.
        ///     E.g. 120.500 = 120500.
        /// </summary>
        public int Frequency { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Navaid"/> class.
        /// </summary>
        /// <param name="identifier">
        ///     The ICAO identifier.
        /// </param>
        /// <param name="location">
        ///     The <see cref="Coordinate"/> representing the location.
        /// </param>
        /// <param name="frequency">
        ///     the frequency of the <see cref="Navaid"/> multiplied by 1000.
        ///     E.g. 120.500 = 120500.
        /// </param>
        /// <param name="type">
        ///     The <see cref="NavaidType"/>.
        /// </param>
        public Navaid(string identifier, Coordinate location, int frequency, NavaidType type)
            : base(identifier, location)
        {
            Frequency = frequency;
            Type = type;
        }
    }
}

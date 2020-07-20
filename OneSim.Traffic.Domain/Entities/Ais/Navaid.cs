// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Navaid.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities.Ais
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
    }
}

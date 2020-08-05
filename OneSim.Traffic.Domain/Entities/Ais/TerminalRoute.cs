// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TerminalRoute.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities.Ais
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="PreDefinedRoute"/> representing a terminal route (SID / STAR).
    /// </summary>
    public class TerminalRoute : PreDefinedRoute
    {
        /// <summary>
        ///     Gets or sets the <see cref="TerminalRouteType"/>.
        /// </summary>
        public TerminalRouteType Type { get; set; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="Runway"/>s which the current
        ///     <see cref="TerminalRoute"/> is valid for.
        /// </summary>
        public List<Runway> ValidRunways { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TerminalRoute"/> class.
        /// </summary>
        /// <param name="identifier">
        ///     The identifier of the <see cref="TerminalRoute"/>.
        /// </param>
        /// <param name="type">
        ///     The <see cref="TerminalRouteType"/>.
        /// </param>
        /// <param name="fixes">
        ///     The <see cref="List{T}"/> of <see cref="Fix"/>es which make up the <see cref="TerminalRoute"/>.
        /// </param>
        /// <param name="validRunways">
        ///     The <see cref="List{T}"/> of <see cref="Runway"/>s which the <see cref="TerminalRoute"/> is valid for.
        /// </param>
        public TerminalRoute(string identifier, TerminalRouteType type, List<Fix> fixes = null, List<Runway> validRunways = null)
            : base(fixes)
        {
            if (string.IsNullOrEmpty(identifier)) throw new ArgumentNullException(nameof(identifier));

            Identifier = identifier;
            Type = type;
            ValidRunways = validRunways ?? new List<Runway>();
        }
    }
}

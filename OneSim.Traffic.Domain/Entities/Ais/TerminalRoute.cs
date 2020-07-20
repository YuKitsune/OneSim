// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TerminalRoute.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities.Ais
{
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="PreDefinedRoute"/> representing a terminal route (SID / STAR).
    /// </summary>
    public class TerminalRoute : PreDefinedRoute
    {
        /// <summary>
        ///     Gets or sets the identifier of the current <see cref="Airway"/>.
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="TerminalRouteType"/>.
        /// </summary>
        public TerminalRouteType Type { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="List{T}"/> of <see cref="Runway"/>s which the current
        ///     <see cref="TerminalRoute"/> is valid for.
        /// </summary>
        public List<Runway> ValidRunways { get; set; }
    }
}

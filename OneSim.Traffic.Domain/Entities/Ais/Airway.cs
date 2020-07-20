// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Airway.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities.Ais
{
    /// <summary>
    ///     The <see cref="PreDefinedRoute"/> representing an airway.
    /// </summary>
    public class Airway : PreDefinedRoute
    {
        /// <summary>
        ///     Gets or sets the identifier of the current <see cref="Airway"/>.
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="AirwayDirection"/>.
        /// </summary>
        public AirwayDirection Direction { get; set; }
    }
}

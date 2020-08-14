// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Airway.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities.Aeronautical
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="PreDefinedRoute"/> representing an airway.
    /// </summary>
    public class Airway : PreDefinedRoute
    {
        /// <summary>
        ///     Gets or sets the <see cref="AirwayDirection"/>.
        /// </summary>
        public AirwayDirection Direction { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Airway"/> class.
        /// </summary>
        /// <param name="identifier">
        ///     The identifier of the <see cref="Airway"/>.
        /// </param>
        /// <param name="fixes">
        ///     The <see cref="List{T}"/> of <see cref="Fix"/>es which make up the <see cref="Airway"/>.
        /// </param>
        public Airway(string identifier, List<Fix> fixes = null)
            : base(fixes)
        {
            if (string.IsNullOrEmpty(identifier)) throw new ArgumentNullException(nameof(identifier));
            Identifier = identifier;
            Direction = AirwayDirection.OnmiDirectional;
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreDefinedRoute.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities.Ais
{
    using System.Collections.Generic;

    /// <summary>
    ///     The pre-defined route.
    /// </summary>
    public abstract class PreDefinedRoute : SectorSetSpecificEntity
    {
        /// <summary>
        ///     Gets or sets the ID of the current <see cref="PreDefinedRoute"/>.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the identifier for the current <see cref="PreDefinedRoute"/>.
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="List{T}"/> of <see cref="Fix"/>es which make up the
        ///     <see cref="PreDefinedRoute"/>.
        /// </summary>
        public List<Fix> Fixes { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PreDefinedRoute"/> class.
        /// </summary>
        /// <param name="fixes">
        ///     The <see cref="List{T}"/> of <see cref="Fix"/>es which make up the <see cref="PreDefinedRoute"/>.
        /// </param>
        protected PreDefinedRoute(List<Fix> fixes = null) => Fixes = fixes ?? new List<Fix>();
    }
}

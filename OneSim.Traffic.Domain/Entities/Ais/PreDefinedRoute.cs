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
    public abstract class PreDefinedRoute : SectorSetSpecific
    {
        /// <summary>
        ///     Gets or sets the ID of the current <see cref="PreDefinedRoute"/>.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="List{T}"/> of <see cref="Fix"/>es which make up the
        ///     <see cref="PreDefinedRoute"/>.
        /// </summary>
        public List<Fix> Fixes { get; set; }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectorSetSpecific.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities.Ais
{
    /// <summary>
    ///     The class representing an object which belongs to a specific <see cref="SectorSet"/>.
    /// </summary>
    public abstract class SectorSetSpecific
    {
        /// <summary>
        ///     The <see cref="SectorSet"/> which owns the current object.
        /// </summary>
        public SectorSet ParentSectorSet { get; set; }
    }
}

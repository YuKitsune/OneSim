// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectorSetSpecificEntity.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities.Ais
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     The class representing an entity which belongs to a specific <see cref="SectorSet"/>.
    /// </summary>
    public abstract class SectorSetSpecificEntity
    {
        /// <summary>
        ///     The <see cref="SectorSet"/> which owns the current object.
        /// </summary>
        [Required]
        public SectorSet ParentSectorSet { get; set; }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectorSetSpecificEntity.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities.Aeronautical
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     The class representing an entity which belongs to a specific <see cref="SectorSet"/>.
    /// </summary>
    public abstract class SectorSetSpecificEntity
    {
        /// <summary>
        ///     Gets or sets the <see cref="SectorSet"/> which owns the current object.
        /// </summary>
        [Required]
        public SectorSet ParentSectorSet { get; set; }
    }
}

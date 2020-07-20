// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Sector.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities.Ais
{
    using System.Collections.Generic;

    /// <summary>
    ///     The Air Traffic Control sector.
    /// </summary>
    public class Sector : SectorSetSpecific
    {
        /// <summary>
        ///     Gets or sets the ID of the current <see cref="Sector"/>.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the name of the current <see cref="Sector"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="List{T}"/> of <see cref="Point2D"/>2 representing the lateral border of the
        ///     current <see cref="Sector"/>.
        /// </summary>
        public List<Point2D> Border { get; set; }

        // Todo: Holes.

        /// <summary>
        ///     Gets or sets the lower level of the current <see cref="Sector"/> measured in feet (ft).
        /// </summary>
        public int LowerLevel { get; set; }

        /// <summary>
        ///     Gets or sets the upper level of the current <see cref="Sector"/> measured in feet (ft).
        /// </summary>
        public int UpperLevel { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="List{T}"/> of <see cref="ControllerPriority"/>s for the current
        ///     <see cref="Sector"/>.
        /// </summary>
        public List<ControllerPriority> Positions { get; set; }
    }
}

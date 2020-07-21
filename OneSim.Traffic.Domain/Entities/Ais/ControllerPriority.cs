// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControllerPriority.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities.Ais
{
    /// <summary>
    ///     The class representing what priority a particular <see cref="ControllerPosition"/> has when assuming a
    ///     specific <see cref="Sector"/>.
    /// </summary>
    public class ControllerPriority : SectorSetSpecificEntity
    {
        /// <summary>
        ///     Gets or sets the ID of the current <see cref="ControllerPriority"/>.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="int"/> representing the priority.
        ///     1 being highest priority, everything after decreasing in priority.
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="ControllerPosition"/>.
        /// </summary>
        public ControllerPosition Position { get; set; }
    }
}

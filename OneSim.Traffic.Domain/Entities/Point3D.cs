// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Point3D.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities
{
    /// <summary>
    ///     A 3 dimensional (<see cref="Point2D"/> + Altitude) point in space.
    /// </summary>
    public class Point3D : Point2D
    {
        /// <summary>
        ///     Gets or sets the altitude in feet (ft).
        /// </summary>
        public int Altitude { get; set; }
    }
}

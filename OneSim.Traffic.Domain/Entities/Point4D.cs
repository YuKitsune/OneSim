// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Point4D.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities
{
    using System;

    /// <summary>
    ///     A <see cref="Point3D"/> with a <see cref="DateTime"/>.
    /// </summary>
    public class Point4D : Point3D
    {
        /// <summary>
        ///     Gets or sets the <see cref="DateTime"/> at which the owner of the current <see cref="Point4D"/> was
        ///     located at <see cref="Point4D"/>.
        /// </summary>
        public DateTime DateTime { get; set; }
    }
}

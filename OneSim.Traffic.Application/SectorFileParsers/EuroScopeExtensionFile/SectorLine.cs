// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectorLine.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Application.SectorFileParsers.EuroScopeExtensionFile
{
    using System.Collections.Generic;

    using OneSim.Traffic.Domain.Entities;

    /// <summary>
    ///     The Sector Line.
    /// </summary>
    public class SectorLine
    {
        /// <summary>
        ///     Gets the ID of the current <see cref="SectorLine"/>.
        /// </summary>
        public string LineId { get; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="Point2D"/>s representing the current
        ///     <see cref="SectorLine"/>.
        /// </summary>
        public List<Point2D> Points { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SectorLine"/> class.
        /// </summary>
        /// <param name="lineId">
        ///     The ID of the <see cref="SectorLine"/>.
        /// </param>
        public SectorLine(string lineId)
        {
            LineId = lineId;
            Points = new List<Point2D>();
        }
    }
}

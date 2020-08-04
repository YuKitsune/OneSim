// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectorLine.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Application.SectorFileParsers.EuroScopeExtensionFile
{
    using System.Collections.Generic;

    using OneSim.Traffic.Domain.Entities;
    using OneSim.Traffic.Domain.Entities.Ais;

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
        ///     Gets the <see cref="List{T}"/> of <see cref="Coordinate"/>s representing the current
        ///     <see cref="SectorLine"/>.
        /// </summary>
        public List<Coordinate> Points { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SectorLine"/> class.
        /// </summary>
        /// <param name="lineId">
        ///     The ID of the <see cref="SectorLine"/>.
        /// </param>
        public SectorLine(string lineId)
        {
            LineId = lineId;
            Points = new List<Coordinate>();
        }
    }
}

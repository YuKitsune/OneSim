// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectorFileParseResult.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Application.SectorFileParsers.SectorFile
{
    using System.Collections.Generic;

    using OneSim.Traffic.Domain.Entities.Ais;

    /// <summary>
    ///     The Sector File parse result.
    /// </summary>
    public class SectorFileParseResult
    {
        /// <summary>
        ///     Gets the current <see cref="SectorSet"/>.
        /// </summary>
        public SectorSet SectorSet { get; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="Airport"/>s.
        /// </summary>
        public List<Airport> Airports { get; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="Fix"/>es.
        /// </summary>
        public List<Fix> Fixes { get; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="Navaid"/>s.
        /// </summary>
        public List<Navaid> Navaids { get; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="NamedSegment"/>s which were found in the low-level
        ///     <see cref="Airway"/> section.
        /// </summary>
        public List<NamedSegment> LowAirwaySegments { get; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of low-level <see cref="Airway"/>.
        /// </summary>
        public List<Airway> LowAirways { get; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="NamedSegment"/>s which were found in the
        ///     high-level <see cref="Airway"/> section.
        /// </summary>
        public List<NamedSegment> HighAirwaySegments { get; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of high-level <see cref="Airway"/>.
        /// </summary>
        public List<Airway> HighAirways { get; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="ParseError"/>s.
        /// </summary>
        public List<ParseError> ParseErrors { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SectorFileParseResult"/> class.
        /// </summary>
        public SectorFileParseResult()
        {
            Airports = new List<Airport>();
            Fixes = new List<Fix>();
            Navaids = new List<Navaid>();

            LowAirwaySegments = new List<NamedSegment>();
            LowAirways = new List<Airway>();
            HighAirwaySegments = new List<NamedSegment>();
            HighAirways = new List<Airway>();

            ParseErrors = new List<ParseError>();
        }
    }
}

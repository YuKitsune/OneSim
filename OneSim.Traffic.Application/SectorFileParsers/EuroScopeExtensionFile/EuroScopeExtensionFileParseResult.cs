// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EuroScopeExtensionFileParseResult.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Application.SectorFileParsers.EuroScopeExtensionFile
{
    using System;
    using System.Collections.Generic;

    using OneSim.Traffic.Application.SectorFileParsers.SectorFile;
    using OneSim.Traffic.Domain.Entities.Aeronautical;

    /// <summary>
    ///     The EuroScope Extension File parse result.
    /// </summary>
    public class EuroScopeExtensionFileParseResult
    {
        /// <summary>
        ///     Gets the current <see cref="SectorSet"/> generated from the EuroScope Extension files corresponding
        ///     Sector file.
        /// </summary>
        public SectorSet SectorSet { get; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="TerminalRoute"/>s.
        /// </summary>
        public List<TerminalRoute> TerminalRoutes { get; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="ControllerPosition"/>.
        /// </summary>
        public List<ControllerPosition> ControllerPositions { get; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="Sector"/>.
        /// </summary>
        public List<Sector> Sectors { get; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="ParseError"/>s.
        /// </summary>
        public List<ParseError> ParseErrors { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EuroScopeExtensionFileParseResult"/> class.
        /// </summary>
        /// <param name="set">
        ///     The <see cref="SectorSet"/> generated from the EuroScope Extension files corresponding Sector file.
        /// </param>
        public EuroScopeExtensionFileParseResult(SectorSet set)
        {
            SectorSet = set ?? throw new ArgumentNullException(nameof(set));
            TerminalRoutes = new List<TerminalRoute>();
            ControllerPositions = new List<ControllerPosition>();
            Sectors = new List<Sector>();
            ParseErrors = new List<ParseError>();
        }
    }
}

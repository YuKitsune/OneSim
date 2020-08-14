// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PositionFileParseResult.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Application.SectorFileParsers.PositionFile
{
    using System.Collections.Generic;

    using OneSim.Traffic.Domain.Entities.Aeronautical;

    /// <summary>
    ///     The Position File parse result.
    /// </summary>
    public class PositionFileParseResult
    {
        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="ControllerPosition"/>.
        /// </summary>
        public List<ControllerPosition> ControllerPositions { get; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="ParseError"/>s.
        /// </summary>
        public List<ParseError> ParseErrors { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PositionFileParseResult"/> class.
        /// </summary>
        public PositionFileParseResult()
        {
            ControllerPositions = new List<ControllerPosition>();
            ParseErrors = new List<ParseError>();
        }
    }
}

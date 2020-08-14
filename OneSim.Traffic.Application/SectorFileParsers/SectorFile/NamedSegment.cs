// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamedSegment.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Application.SectorFileParsers.SectorFile
{
    using OneSim.Traffic.Domain.Entities.Aeronautical;

    /// <summary>
    ///     A single line segment which has been named.
    /// </summary>
    public class NamedSegment
    {
        /// <summary>
        ///     Gets or sets the name of the current <see cref="NamedSegment"/>.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        ///     Gets or sets the starting <see cref="Coordinate"/>.
        /// </summary>
        public Coordinate Start { get; set; }

        /// <summary>
        ///     Gets or sets the end <see cref="Coordinate"/>.
        /// </summary>
        public Coordinate End { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NamedSegment"/> class.
        /// </summary>
        /// <param name="label">
        ///     The name of the <see cref="NamedSegment"/>.
        /// </param>
        /// <param name="start">
        ///     The starting <see cref="Coordinate"/>.
        /// </param>
        /// <param name="end">
        ///     The end <see cref="Coordinate"/>.
        /// </param>
        public NamedSegment(string label, Coordinate start, Coordinate end)
        {
            Label = label;
            Start = start;
            End = end;
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Segment.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Application.SectorFileParsers.SectorFile
{
    using OneSim.Traffic.Domain.Entities;

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
        ///     Gets or sets the starting <see cref="Point2D"/>.
        /// </summary>
        public Point2D Start { get; set; }

        /// <summary>
        ///     Gets or sets the end <see cref="Point2D"/>.
        /// </summary>
        public Point2D End { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NamedSegment"/> class.
        /// </summary>
        /// <param name="label">
        ///     The name of the <see cref="NamedSegment"/>.
        /// </param>
        /// <param name="start">
        ///     The starting <see cref="Point2D"/>.
        /// </param>
        /// <param name="end">
        ///     The end <see cref="Point2D"/>.
        /// </param>
        public NamedSegment(string label, Point2D start, Point2D end)
        {
            Label = label;
            Start = start;
            End = end;
        }
    }
}

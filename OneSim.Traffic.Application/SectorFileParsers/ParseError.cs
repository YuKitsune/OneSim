// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectorFileParser.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Application.SectorFileParsers
{
    using System;

    /// <summary>
    ///     The file parse error.
    /// </summary>
    public class ParseError 
    {
        /// <summary>
        ///     The number of the line where the current <see cref="ParseError"/> occurred.
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        ///     The content of the line where the current <see cref="ParseError"/> exists.
        /// </summary>
        public string LineContents { get; set; }

        /// <summary>
        ///     The message outlining the error.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     The message outlining the error.
        /// </summary>
        /// <param name="lineNumber">
        ///     The number of the line where the <see cref="ParseError"/> occurred.
        /// </param>
        /// <param name="contents">
        ///     The content of the line where the <see cref="ParseError"/> exists.
        /// </param>
        /// <param name="message">
        ///     The message outlining the error.
        /// </param>
        public ParseError(int lineNumber, string contents, string message)
        {
            if (lineNumber >= 0) throw new ArgumentException($"The {nameof(lineNumber)} must be greater than 0.", nameof(lineNumber));
            if (string.IsNullOrEmpty(contents)) throw new ArgumentNullException(nameof(contents));
            if (string.IsNullOrEmpty(message)) throw new ArgumentNullException(nameof(message));

            LineNumber = lineNumber;
            LineContents = contents;
            Message = message;
        }
    }
}
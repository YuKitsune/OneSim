// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PositionFileParser.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Application.SectorFileParsers.PositionFile
{
    using OneSim.Traffic.Domain.Entities.Ais;

    /// <summary>
    ///     The Position File (.pof) parser.
    /// </summary>
    public class PositionFileParser
    {
        /// <summary>
        ///     Gets or sets the <see cref="PositionFileParseResult"/>.
        /// </summary>
        private PositionFileParseResult Result { get; set; }

        /// <summary>
        ///     The current line number.
        /// </summary>
        private int _lineNumber = 0;

        /// <summary>
        ///     The current line content.
        /// </summary>
        private string _currentLine = string.Empty;

        /// <summary>
        ///     Adds a new <see cref="ParseError"/> to the error list.
        /// </summary>
        /// <param name="message">
        ///     The error message.
        /// </param>
        private void AddParseError(string message) =>
            Result.ParseErrors.Add(new ParseError(_lineNumber, _currentLine, message));

        /// <summary>
        ///     Parses the given <see cref="string"/> as a EuroScope Extension files content.
        /// </summary>
        /// <param name="fileContent">
        ///     The Position file content.
        /// </param>
        /// <returns>
        ///     The <see cref="PositionFileParseResult"/>.
        /// </returns>
        public PositionFileParseResult Parse(string fileContent)
        {
            string[] lines = fileContent.Split('\r', '\n');
            foreach (string line in lines)
            {
                _currentLine = line;
                _lineNumber++;

                // Skip empty lines.
                if (string.IsNullOrEmpty(_currentLine.Trim())) continue;

                // Skip lines that contain only a comment.
                if (_currentLine.Trim().Substring(0, 1) == ";" ||
                    _currentLine.Trim().Substring(0, 1) == "//")
                    continue;

                // Strip off trailing comments.
                FileParserUtils.StripComments(ref _currentLine);

                // Trim trailing whitespace from the line.
                _currentLine = _currentLine.TrimEnd(' ', '\t', '\r', '\n', '\f');

                // If the line ends up empty, skip it.
                if (string.IsNullOrEmpty(_currentLine)) continue;

                ParsePositionLine();
            }

            return Result;
        }

        /// <summary>
        ///     Parses the <see cref="_currentLine"/> as a <see cref="ControllerPosition"/>.
        /// </summary>
        private void ParsePositionLine()
        {
            // Split by ":"
            string[] sections = _currentLine.Split(":");

            if (sections.Length < 7)
            {
                AddParseError(
                    $"Unexpected number of sections in the Position definition. Expected 7 or more, found {sections.Length}.");
                return;
            }

            Result.ControllerPositions.Add(
                new ControllerPosition
                {
                    Name = sections[0],
                    RadioCallsign = sections[1],
                    Frequency = FileParserUtils.ParseFrequency(sections[2]),
                    SectorId = sections[3],
                    CallsignPrefix = sections[5],
                    CallsignMiddle = sections[4],
                    CallsignSuffix = sections[6]
                });
        }
    }
}

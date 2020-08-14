// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileParserUtils.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Application.SectorFileParsers
{
    using System;
    using System.Globalization;
    using System.Linq;

    using OneSim.Traffic.Application.SectorFileParsers.SectorFile;
    using OneSim.Traffic.Domain.Entities;
    using OneSim.Traffic.Domain.Entities.Aeronautical;

    /// <summary>
    ///     The file parser utilities.
    /// </summary>
    public static class FileParserUtils
    {
        /// <summary>
        ///     Strips any comments from the given <paramref name="line"/>.
        /// </summary>
        /// <param name="line">
        ///     The <see cref="string"/> to strip the comments from.
        /// </param>
        public static void StripComments(ref string line)
        {
            // Get rid of anything after ";" or "//"
            int pos = line.IndexOf(';');
            if (pos > -1) line = line.Substring(0, pos);
            pos = line.IndexOf("//", StringComparison.Ordinal);
            if (pos > -1) line = line.Substring(0, pos);
        }

        /// <summary>
        ///     Parses the given <see cref="string"/> as a radio frequency.
        /// </summary>
        /// <param name="s">
        ///     The frequency in the form of a <see cref="string"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="int"/> form of the frequency.
        /// </returns>
        public static int ParseFrequency(string s)
        {
            // Frequencies are stored in integer form.
            double.TryParse(s, out double freq);
            return (int)(freq * 1000);
        }

        /// <summary>
        ///     Determines whether or not the given <see cref="char"/> is whitespace.
        /// </summary>
        /// <param name="c">
        ///     The <see cref="char"/>.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="c"/> is whitespace, <c>false</c> otherwise.
        /// </returns>
        public static bool IsWhitespace(char c) => c == ' ' || c == '\t' || c == '\r' || c == '\n' || c == '\f';

        /// <summary>
        ///     Gets a <see cref="Fix"/> given a <see cref="Coordinate"/>.
        /// </summary>
        /// <param name="result">
        ///     The <see cref="SectorFileParseResult"/> to get the fixes from.
        /// </param>
        /// <param name="coordinate">
        ///     The <see cref="Coordinate"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="Fix"/> at the <paramref name="coordinate"/>.
        /// </returns>
        public static Fix GetFixFromCoordinate(SectorFileParseResult result, Coordinate coordinate)
        {
            // Check fixes
            Fix matchingFix = result.Fixes.FirstOrDefault(f => f.Location.Equals(coordinate));
            if (matchingFix != null) return matchingFix;

            // Check navaids
            Navaid matchingNavaid = result.Navaids.FirstOrDefault(n => n.Location.Equals(coordinate));
            if (matchingNavaid != null) return matchingNavaid;

            // Check airports
            Fix matchingAirport = result.Airports.FirstOrDefault(a => a.Location.Equals(coordinate));
            if (matchingAirport != null) return matchingAirport;

            // Made it this far and haven't found anything, we're fucked
            throw new Exception($"Unable to find any fixes at {coordinate}.");
        }

        /// <summary>
        ///     Gets a <see cref="Fix"/> given a <see cref="Fix.Identifier"/>.
        /// </summary>
        /// <param name="result">
        ///     The <see cref="SectorFileParseResult"/> to get the fixes from.
        /// </param>
        /// <param name="identifier">
        ///     The <see cref="Fix.Identifier"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="Fix"/> with the matching <paramref name="identifier"/>.
        /// </returns>
        public static Fix GetFixFromName(SectorFileParseResult result, string identifier)
        {
            // Check fixes
            Fix matchingFix = result.Fixes.FirstOrDefault(f => f.Identifier == identifier);
            if (matchingFix != null) return matchingFix;

            // Check navaids
            Navaid matchingNavaid = result.Navaids.FirstOrDefault(n => n.Identifier == identifier);
            if (matchingNavaid != null) return matchingNavaid;

            // Check airports
            Fix matchingAirport = result.Airports.FirstOrDefault(a => a.Identifier == identifier);
            if (matchingAirport != null) return matchingAirport;

            // Made it this far and haven't found anything, we're fucked
            throw new Exception($"Unable to find any fixes with the identifier {identifier}.");
        }
    }
}

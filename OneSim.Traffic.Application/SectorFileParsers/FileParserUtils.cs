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
    using OneSim.Traffic.Domain.Entities.Ais;

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
        ///     Converts a DMS coordinate to a <see cref="double"/>.
        /// </summary>
        /// <param name="s">
        ///     The DMS coordinate in the form of a <see cref="string"/>.
        /// </param>
        /// <returns>
        ///     The coordinate in the form of a <see cref="double"/>.
        /// </returns>
        public static double DmsToDecimal(string s)
        {
            // Make sure we got a value.
            if (!string.IsNullOrEmpty(s))
            {
                // Find out which side of the axis we're on, then strip out the N, S, E or W.
                bool neg = s.IndexOfAny(new[] { 'S', 's', 'W', 'w' }) > -1;
                s = s.Substring(1);

                // Get the whole degrees portion.
                char[] sep = { '.', ',' };
                int pt1 = s.IndexOfAny(sep);
                if (pt1 > -1)
                {
                    string deg = s.Substring(0, pt1);

                    // Get the minutes portion.
                    int pt2 = s.IndexOfAny(sep, pt1 + 1);
                    if (pt2 > -1)
                    {
                        string min = s.Substring(pt1 + 1, (pt2 - pt1) - 1);

                        // Get the whole seconds portion.
                        int pt3 = s.IndexOfAny(sep, pt2 + 1);
                        if (pt3 > -1)
                        {
                            string secWhole = s.Substring(pt2 + 1, (pt3 - pt2) - 1);

                            // Get the partial seconds portion.
                            if (pt3 < s.Length - 1)
                            {
                                string secDec = s.Substring(pt3 + 1);

                                // Reassemble the seconds value.
                                string sec = secWhole + "." + secDec;

                                // Parse into numeric values.
                                int.TryParse(deg, out int degrees);
                                int.TryParse(min, out int minutes);
                                double.TryParse(sec, out double seconds);

                                // Do the math.
                                double result = degrees;
                                result += minutes / 60.0;
                                result += seconds / 3600.0;

                                // Return the result, negated if necessary.
                                return Math.Round(neg ? (result * -1.0) : result, 7);
                            }
                        }
                    }
                }
            }

            // If we fall through to this line, then there was a problem with the value.
            throw new Exception($"Invalid formatting in lat/lon value: {s}");
        }

        /// <summary>
        ///     Gets a <see cref="Fix"/> given a <see cref="Point2D"/>.
        /// </summary>
        /// <param name="result">
        ///     The <see cref="SectorFileParseResult"/> to get the fixes from.
        /// </param>
        /// <param name="point">
        ///     The <see cref="Point2D"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="Fix"/> at the <paramref name="point"/>.
        /// </returns>
        public static Fix GetFixFromPoint(SectorFileParseResult result, Point2D point)
        {
            const int MaxDistance = 1;

            // Check fixes
            Fix matchingFix = result.Fixes.FirstOrDefault(f => f.Location.IsWithin(MaxDistance, point));
            if (matchingFix != null) return matchingFix;

            // Check navaids
            Navaid matchingNavaid = result.Navaids.FirstOrDefault(n => n.Location.IsWithin(MaxDistance, point));
            if (matchingNavaid != null) return matchingNavaid;

            // Check airports
            Fix matchingAirport = result.Airports.FirstOrDefault(a => a.Location.IsWithin(MaxDistance, point));
            if (matchingAirport != null) return matchingAirport;

            // Made it this far and haven't found anything, we're fucked
            throw new Exception($"Unable to find any fixes within {MaxDistance}nm of {point}.");
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

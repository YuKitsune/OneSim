// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectorFileParser.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Application.SectorFileParsers.SectorFile
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using OneSim.Traffic.Domain.Entities;
    using OneSim.Traffic.Domain.Entities.Ais;

    /// <summary>
    ///     The Sector File (.sct or .stc2) parser.
    /// </summary>
    /// <remarks>
    ///     The GEO, REGIONS and LABELS sections, as well as color definitions are ignored, as OneSim does not have a
    ///     use for them.
    /// </remarks>
    public class SectorFileParser
    {
        /// <summary>
        ///     Gets or sets the <see cref="SectorFileParseResult"/>.
        /// </summary>
        private SectorFileParseResult Result { get; set; }

        /// <summary>
        ///     The <see cref="Regex"/> for the Section header.
        /// </summary>
        private static readonly Regex SectionHeaderRegex = new Regex(@"^\[(.+)\]$", RegexOptions.Compiled);

        /// <summary>
        ///     The <see cref="Regex"/> for VORs and NDBs.
        /// </summary>
        private static readonly Regex VorNdbRegex = new Regex(@"^(\S+)\s+(\S+)\s+(\S+)\s+(\S+)", RegexOptions.Compiled);

        /// <summary>
        ///     The <see cref="Regex"/> for the Fixes.
        /// </summary>
        private static readonly Regex FixRegex = new Regex(@"^(\S+)\s+(\S+)\s+(\S+)", RegexOptions.Compiled);

        /// <summary>
        ///     The <see cref="Regex"/> for the Airports.
        /// </summary>
        private static readonly Regex AirportRegex = new Regex(@"^(\S+)\s+(\S+)\s+(\S+)\s+(\S+)(?:\s+([A-Z]))?", RegexOptions.Compiled);

        /// <summary>
        ///     The <see cref="Regex"/> for the Runways.
        /// </summary>
        private static readonly Regex RunwayRegex = new Regex(@"^(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)", RegexOptions.Compiled);

        /// <summary>
        ///     The <see cref="Regex"/> for the ARTCC boundaries.
        /// </summary>
        private static readonly Regex BoundarySegmentRegex = new Regex(@"^(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)", RegexOptions.Compiled);

        /// <summary>
        ///     The <see cref="CultureInfo"/>.
        /// </summary>
        private static readonly CultureInfo Culture = new CultureInfo("en-US");

        /// <summary>
        ///     The current <see cref="Match"/>.
        /// </summary>
        private Match _currentMatch;

        /// <summary>
        ///     The current line number.
        /// </summary>
        private int _lineNumber;

        /// <summary>
        ///     The current line content.
        /// </summary>
        private string _currentLine = string.Empty;

        /// <summary>
        ///     The current <see cref="FileSection"/>.
        /// </summary>
        private FileSection _currentSection = FileSection.None;

        /// <summary>
        ///     Adds a new <see cref="ParseError"/> to the error list.
        /// </summary>
        /// <param name="message">
        ///     The error message.
        /// </param>
        private void AddParseError(string message) => Result.ParseErrors.Add(new ParseError(_lineNumber, _currentLine, message));

        /// <summary>
        ///     Converts a DMS coordinate to a <see cref="double"/>.
        /// </summary>
        /// <param name="s">
        ///     The DMS coordinate in the form of a <see cref="string"/>.
        /// </param>
        /// <returns>
        ///     The coordinate in the form of a <see cref="double"/>.
        /// </returns>
        public double DmsToDecimal(string s)
        {
            try
            {
                return FileParserUtils.DmsToDecimal(s);
            }
            catch (Exception ex)
            {
                AddParseError(ex.Message);
                return 0.0;
            }
        }

        /// <summary>
        ///     Parses the given sector file content.
        /// </summary>
        /// <param name="stream">
        ///     The sector file content in the form of a <see cref="string"/>.
        /// </param>
        public void Parse(Stream stream)
        {
            Result = new SectorFileParseResult();
            using (StreamReader sr = new StreamReader(stream))
            {
                int infoSectionLine = 0;

                // Need 2 passes, 1 to get all the fixes, 2 to get segments, diagrams, etc (since they rely on fixes)
                for (int pass = 1; pass <= 2; pass++)
                {
                    _currentSection = FileSection.None;
                    _lineNumber = 0;

                    // Rewind the file for pass 2.
                    if (pass == 2) sr.BaseStream.Seek(0, SeekOrigin.Begin);

                    // Step through the file.
                    while ((_currentLine = sr.ReadLine()) != null)
                    {
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

                        // Look for [SECTION] headers.
                        if (_currentLine.Substring(0, 1) == "[")
                        {
                            _currentMatch = SectionHeaderRegex.Match(_currentLine);
                            if (_currentMatch.Success)
                            {
                                // Switch to the new section.
                                switch (_currentMatch.Groups[1].Value.ToUpper())
                                {
                                    case "INFO":
                                        _currentSection = FileSection.Info;
                                        infoSectionLine = 0;
                                        continue;

                                    case "VOR":
                                        _currentSection = FileSection.VOR;
                                        continue;

                                    case "NDB":
                                        _currentSection = FileSection.NDB;
                                        continue;

                                    case "AIRPORT":
                                        _currentSection = FileSection.Airport;
                                        continue;

                                    case "RUNWAY":
                                        _currentSection = FileSection.Runway;
                                        continue;

                                    case "FIXES":
                                        _currentSection = FileSection.Fixes;
                                        continue;

                                    case "LOW AIRWAY":
                                        _currentSection = FileSection.LowAirway;
                                        continue;

                                    case "HIGH AIRWAY":
                                        _currentSection = FileSection.HighAirway;
                                        continue;

                                    case "ARTCC":
                                    case "ARTCC HIGH":
                                    case "ARTCC LOW":
                                    case "SID":
                                    case "STAR":
                                    case "GEO":
                                    case "REGIONS":
                                    case "LABELS":
                                        _currentSection = FileSection.Unsupported;
                                        continue;

                                    default:
                                        if (pass == 1) AddParseError("Unknown section header encountered.");
                                        continue;
                                }
                            }
                        }

                        // If we're in an unsupported section, just skip
                        if (_currentSection == FileSection.Unsupported) continue;

                        // If we get here, and we're not in a [SECTION], then we've found an orphaned line.
                        if (_currentSection == FileSection.None)
                        {
                            if (pass == 1) AddParseError("Orphaned line.");
                            continue;
                        }

                        // If we get this far, we've got a data line. Call the appropriate parsing
                        // method based on the current pass and section.
                        switch (pass)
                        {
                            case 1:
                                switch (_currentSection)
                                {
                                    case FileSection.Info:
                                        ParseInfoLine(++infoSectionLine);
                                        break;

                                    case FileSection.VOR:
                                    case FileSection.NDB:
                                        ParseVorNdbLine();
                                        break;

                                    case FileSection.Airport:
                                        ParseAirportLine();
                                        break;

                                    case FileSection.Fixes:
                                        ParseFixLine();
                                        break;
                                }

                                break;

                            case 2:
                                switch (_currentSection)
                                {
                                    case FileSection.Runway:
                                        ParseRunwayLine();
                                        break;

                                    case FileSection.ARTCC:
                                    case FileSection.ArtccHigh:
                                    case FileSection.ArtccLow:
                                    case FileSection.HighAirway:
                                    case FileSection.LowAirway:
                                        ParseNamedSegmentLine();
                                        break;

                                    case FileSection.SID:
                                    case FileSection.STAR:
                                    case FileSection.Geo:
                                    case FileSection.Regions:
                                    case FileSection.Labels:
                                        // Not supported by OneSim
                                        break;
                                }

                                break;
                        }
                    }
                }

                // Convert segments to airways
                Result.LowAirways.AddRange(GetRoutesFromSegments(Result.LowAirwaySegments, identifier => new Airway(identifier)));
                Result.HighAirways.AddRange(GetRoutesFromSegments(Result.HighAirwaySegments, identifier => new Airway(identifier)));
            }
        }

        /// <summary>
        ///     Parses the current line as an info line.
        /// </summary>
        /// <param name="infoLine">
        ///     The line number relative to the info section.
        /// </param>
        private void ParseInfoLine(int infoLine)
        {
            switch (infoLine)
            {
                case 1:
                    Result.SectorSet.Name = _currentLine;
                    break;

                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    // Unsupported by OneSim
                    break;

                default:
                    AddParseError("Extra line in [INFO] section.");
                    break;
            }
        }

        /// <summary>
        ///     Parses the <see cref="_currentLine"/> as a <see cref="Navaid"/>.
        /// </summary>
        private void ParseVorNdbLine()
        {
            _currentMatch = VorNdbRegex.Match(_currentLine);
            if (_currentMatch.Success)
            {
                // Check which type of Navaid we're reading
                NavaidType type = _currentSection switch
                {
                    FileSection.VOR => NavaidType.VOR,
                    FileSection.NDB => NavaidType.NDB,
                    _ => throw new InvalidOperationException("Invalid file section.")
                };

                // Create the navaid
                string identifier = _currentMatch.Groups[1].Value.ToUpper();
                Navaid navaid = new Navaid(
                    identifier,
                    new Point2D(
                        DmsToDecimal(_currentMatch.Groups[4].Value),
                        DmsToDecimal(_currentMatch.Groups[3].Value)),
                    FileParserUtils.ParseFrequency(_currentMatch.Groups[2].Value),
                    type);

                Result.Navaids.Add(navaid);
            }
            else
            {
                AddParseError($"Unrecognized formatting in {_currentSection} section.");
            }
        }

        /// <summary>
        ///      Parses the <see cref="_currentLine"/> as an <see cref="Airport"/>.
        /// </summary>
        private void ParseAirportLine()
        {
            _currentMatch = AirportRegex.Match(_currentLine);
            if (_currentMatch.Success)
            {
                // Create the airport
                string identifier = _currentMatch.Groups[1].Value.ToUpper();
                Airport airport = new Airport(
                    identifier,
                    new Point2D(
                        DmsToDecimal(_currentMatch.Groups[4].Value),
                        DmsToDecimal(_currentMatch.Groups[3].Value)));

                // Todo: CTAF: ParseFrequency(mMatch.Groups[2].Value),

                // Set the AirspaceClass if we have one
                if (!string.IsNullOrEmpty(_currentMatch.Groups[5].Value)) airport.Class = (AirspaceClass)Enum.Parse(typeof(AirspaceClass), _currentMatch.Groups[5].Value);

                Result.Airports.Add(airport);
            }
            else
            {
                AddParseError($"Unrecognized formatting in {_currentSection} section.");
            }
        }

        /// <summary>
        ///     Parses the <see cref="_currentLine"/> as a <see cref="Fix"/>.
        /// </summary>
        private void ParseFixLine()
        {
            _currentMatch = FixRegex.Match(_currentLine);
            if (_currentMatch.Success)
            {
                // Create the fix
                string identifier = _currentMatch.Groups[1].Value.ToUpper();
                Fix fix = new Fix(
                    identifier,
                    new Point2D(
                        DmsToDecimal(_currentMatch.Groups[3].Value),
                        DmsToDecimal(_currentMatch.Groups[2].Value)));

                Result.Fixes.Add(fix);
            }
            else
            {
                AddParseError($"Unrecognized formatting in {_currentSection} section.");
            }
        }

        /// <summary>
        ///     Parses the <see cref="_currentLine"/> as a <see cref="Runway"/>.
        /// </summary>
        private void ParseRunwayLine()
        {
            _currentMatch = RunwayRegex.Match(_currentLine);
            if (_currentMatch.Success)
            {
                // Create the first runway
                int.TryParse(_currentMatch.Groups[3].Value, out int firstHeading);
                Runway firstRunway = new Runway(
                    _currentMatch.Groups[1].Value.ToUpper(),
                    new Point2D(
                        DmsToDecimal(_currentMatch.Groups[6].Value),
                        DmsToDecimal(_currentMatch.Groups[5].Value)),
                    firstHeading);

                // Create the second runway
                int.TryParse(_currentMatch.Groups[4].Value, out int secondHeading);
                Runway secondRunway = new Runway(
                    _currentMatch.Groups[2].Value.ToUpper(),
                    new Point2D(
                        DmsToDecimal(_currentMatch.Groups[8].Value),
                        DmsToDecimal(_currentMatch.Groups[7].Value)),
                    secondHeading);

                // Find the midpoint between the thresholds
                Point2D runwayMidpoint = new Point2D(
                    (firstRunway.ThresholdLocation.Longitude - secondRunway.ThresholdLocation.Longitude) / 2,
                    (firstRunway.ThresholdLocation.Latitude - secondRunway.ThresholdLocation.Latitude) / 2);

                // Find the closest airport so we can add it
                Airport closestAirport = null;
                double distance = 0;
                foreach (Airport airport in Result.Airports)
                {
                    double currentDistance =
                        Math.Sqrt(
                            Math.Pow(airport.Location.Latitude - runwayMidpoint.Latitude, 2) +
                            Math.Pow(airport.Location.Longitude - runwayMidpoint.Longitude, 2));

                    // If we don't have an airport set, just use the current one, otherwise override if we have a
                    // closer one
                    if (closestAirport == null ||
                        currentDistance < distance)
                    {
                        closestAirport = airport;
                    }
                }

                // Add the runways to the closest airport
                if (closestAirport != null)
                {
                    closestAirport.Runways.Add(firstRunway);
                    closestAirport.Runways.Add(secondRunway);
                }
                else
                {
                    AddParseError("Unable to find a suitable airport for runways.");
                }
            }
            else
            {
                AddParseError($"Unrecognized formatting in {_currentSection} section.");
            }
        }

        /// <summary>
        ///     Parses the <see cref="_currentLine"/> as a <see cref="NamedSegment"/>.
        /// </summary>
        private void ParseNamedSegmentLine()
        {
            _currentMatch = BoundarySegmentRegex.Match(_currentLine);
            if (_currentMatch.Success)
            {
                // Create the segment
                NamedSegment segment = new NamedSegment(
                    _currentMatch.Groups[1].Value,
                    new Point2D(
                        DmsToDecimal(_currentMatch.Groups[3].Value),
                        DmsToDecimal(_currentMatch.Groups[2].Value)),
                    new Point2D(
                        DmsToDecimal(_currentMatch.Groups[5].Value),
                        DmsToDecimal(_currentMatch.Groups[4].Value)));

                // Add to the appropriate list
                switch (_currentSection)
                {
                    // Todo: ARTCCs
                    // case FileSection.ARTCC:
                    //     if (ArtccSegmentFound != null) ArtccSegmentFound(this, new DataFoundEventArgs<LabeledSegment>(segment));
                    //     break;

                    // case FileSection.ArtccHigh:
                    //     if (ArtccHighSegmentFound != null) ArtccHighSegmentFound(this, new DataFoundEventArgs<LabeledSegment>(segment));
                    //     break;

                    // case FileSection.ArtccLow:
                    //     if (ArtccLowSegmentFound != null) ArtccLowSegmentFound(this, new DataFoundEventArgs<LabeledSegment>(segment));
                    //     break;

                    case FileSection.HighAirway:
                        Result.HighAirwaySegments.Add(segment);
                        break;

                    case FileSection.LowAirway:
                        Result.LowAirwaySegments.Add(segment);
                        break;
                }
            }
            else
            {
                AddParseError($"Unrecognized formatting in {_currentSection} section.");
            }
        }

        /// <summary>
        ///     Converts the given <see cref="List{T}"/> of <see cref="NamedSegment"/>s to a <see cref="List{T}"/> of
        ///     <typeparamref name="TRoute"/>s.
        /// </summary>
        /// <param name="segments">
        ///     The <see cref="List{T}"/> of <see cref="NamedSegment"/>s to convert.
        /// </param>
        /// <param name="routeFactory">
        ///     The <see cref="Func{TParam, TResult}"/> to invoke when creating new <typeparamref name="TRoute"/>s.
        ///     The parameter is the <see cref="PreDefinedRoute.Identifier"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="List{T}"/> of <see cref="TRoute"/>s.
        /// </returns>
        /// <typeparam name="TRoute">
        ///     The type of <see cref="PreDefinedRoute"/>.
        /// </typeparam>
        private List<TRoute> GetRoutesFromSegments<TRoute>(
            List<NamedSegment> segments,
            Func<string, TRoute> routeFactory)
            where TRoute : PreDefinedRoute
        {
            List<TRoute> routes = new List<TRoute>();

            // Get all of the identifiers that were found
            string[] identifiers = segments.Select(s => s.Label).Distinct().ToArray();

            foreach (string identifier in identifiers)
            {
                // Get all segments with the current identifier, and create a new route
                List<NamedSegment> currentSegments = segments.Where(s => s.Label == identifier).ToList();
                TRoute route = routeFactory(identifier);

                // Find our first segment (start point doesn't match any of the end points), and our last segment
                // (end point doesn't match any of the start points)
                NamedSegment firstSegment = currentSegments.First(s => !segments.Select(s1 => s1.End).Contains(s.Start));
                NamedSegment lastSegment = currentSegments.First(s => !segments.Select(s1 => s1.Start).Contains(s.End));

                // Add the first segment to the route
                route.Fixes.Add(FileParserUtils.GetFixFromPoint(Result, firstSegment.Start));
                route.Fixes.Add(FileParserUtils.GetFixFromPoint(Result, firstSegment.End));
                while (true)
                {
                    // Find the next segment
                    Fix lastFix = route.Fixes.Last();
                    NamedSegment nextSegment = currentSegments.First(s => s.Start == lastFix.Location);

                    // Add it in
                    route.Fixes.Add(FileParserUtils.GetFixFromPoint(Result, nextSegment.End));

                    // If the next segment is the same as the last segment, then we've finished with this route
                    if (nextSegment == lastSegment) break;
                }

                routes.Add(route);
            }

            return routes;
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectorFileParser.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Application.SectorFileParsers.SectorFile
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using OneSim.Traffic.Domain.Entities;
    using OneSim.Traffic.Domain.Entities.Aeronautical;

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
        ///     The <see cref="Regex"/> for the <see cref="Airway.Identifier"/>.
        /// </summary>
        private static readonly Regex AirwayIdentifierRegex = new Regex(@"[A-Za-z]{1,2}[0-9]{1,3}");

        /// <summary>
        ///     The <see cref="Regex"/> for coordinates.
        /// </summary>
        private static readonly Regex CoordinateRegex = new Regex(@"[NESW]{1}[0-9]{3}\.[0-9]{2}\.[0-9]{2}\.[0-9]{3}");

        /// <summary>
        ///     The <see cref="Regex"/> for <see cref="Fix"/>es.
        /// </summary>
        private static readonly Regex FixIdentifierRegex = new Regex(@"[A-Z]{2,5}");

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
        ///     Parses the given sector file content.
        /// </summary>
        /// <param name="fileContent">
        ///     The sector file content in the form of a <see cref="string"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="SectorFileParseResult"/>.
        /// </returns>
        public SectorFileParseResult Parse(string fileContent)
        {
            Result = new SectorFileParseResult();

            int infoSectionLine = 0;
            string[] lines = fileContent.Split('\r', '\n');

            // Need 2 passes, 1 to get all the fixes, 2 to get segments, diagrams, etc (since they rely on fixes)
            for (int pass = 1; pass <= 2; pass++)
            {
                _currentSection = FileSection.None;

                _lineNumber = 0;
                foreach (string line in lines)
                {
                    _currentLine = line;
                    if (_currentLine == null) continue;

                    _lineNumber++;

                    // Skip empty lines.
                    if (string.IsNullOrEmpty(_currentLine.Trim())) continue;

                    // Skip lines that contain only a comment.
                    if (_currentLine.Trim().Substring(0, 1) == ";" ||
                        _currentLine.Trim().Substring(0, 1) == "//")
                        continue;

                    // Skip #define lines
                    if (_currentLine.Trim().ToUpper().StartsWith("#DEFINE", StringComparison.Ordinal)) continue;

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
                                    _currentSection = FileSection.Vor;
                                    continue;

                                case "NDB":
                                    _currentSection = FileSection.Ndb;
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

                                case FileSection.Vor:
                                case FileSection.Ndb:
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

                                case FileSection.Artcc:
                                case FileSection.ArtccHigh:
                                case FileSection.ArtccLow:
                                case FileSection.HighAirway:
                                case FileSection.LowAirway:
                                    ParseNamedSegmentLine();
                                    break;

                                case FileSection.Sid:
                                case FileSection.Star:
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
            Result.LowAirways.AddRange(
                GetRoutesFromSegments(
                    Result.LowAirwaySegments,
                    identifier => new Airway(identifier)));
            Result.HighAirways.AddRange(
                GetRoutesFromSegments(
                    Result.HighAirwaySegments,
                    identifier => new Airway(identifier)));

            return Result;
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
                    FileSection.Vor => NavaidType.Vor,
                    FileSection.Ndb => NavaidType.Ndb,
                    _ => throw new InvalidOperationException("Invalid file section.")
                };

                // Create the navaid
                string identifier = _currentMatch.Groups[1].Value.ToUpper();
                Navaid navaid = new Navaid(
                    identifier,
                    new Coordinate(
                        _currentMatch.Groups[3].Value,
                        _currentMatch.Groups[4].Value),
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
                    new Coordinate(
                        _currentMatch.Groups[3].Value,
                        _currentMatch.Groups[4].Value));

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
                    new Coordinate(
                        _currentMatch.Groups[2].Value,
                        _currentMatch.Groups[3].Value));

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
                    new Coordinate(
                        _currentMatch.Groups[5].Value,
                        _currentMatch.Groups[6].Value),
                    firstHeading);

                // Create the second runway
                int.TryParse(_currentMatch.Groups[4].Value, out int secondHeading);
                Runway secondRunway = new Runway(
                    _currentMatch.Groups[2].Value.ToUpper(),
                    new Coordinate(
                        _currentMatch.Groups[7].Value,
                        _currentMatch.Groups[8].Value),
                    secondHeading);

                // Find the midpoint between the thresholds
                Point2D runwayMidpoint = new Point2D(
                    (firstRunway.ThresholdLocation.GetPoint().Latitude + secondRunway.ThresholdLocation.GetPoint().Latitude) / 2,
                    (firstRunway.ThresholdLocation.GetPoint().Longitude + secondRunway.ThresholdLocation.GetPoint().Longitude) / 2);

                // Find the closest airport so we can add it
                Airport closestAirport = null;
                double distance = 0;
                foreach (Airport airport in Result.Airports)
                {
                    double currentDistance =
                        Math.Sqrt(
                            Math.Pow(airport.Location.GetPoint().Latitude - runwayMidpoint.Latitude, 2) +
                            Math.Pow(airport.Location.GetPoint().Longitude - runwayMidpoint.Longitude, 2));

                    // If we don't have an airport set, just use the current one, otherwise override if we have a
                    // closer one
                    if (closestAirport == null ||
                        currentDistance < distance)
                    {
                        closestAirport = airport;
                        distance = currentDistance;
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
                    GetCoordinate(
                        _currentMatch.Groups[3].Value,
                        _currentMatch.Groups[2].Value),
                    GetCoordinate(
                        _currentMatch.Groups[5].Value,
                        _currentMatch.Groups[4].Value));

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
                string cleanIdentifier = AirwayIdentifierRegex.Match(identifier).Value;

                // Get all segments with the current identifier, and create a new route
                List<NamedSegment> currentSegments = segments.Where(s => AirwayIdentifierRegex.Match(s.Label).Value == cleanIdentifier).ToList();

                // If we couldn't find any segments, then something has seriously fucked up...
                if (!currentSegments.Any())
                {
                    AddParseError($"Couldn't find any segments when building Airway \"{identifier}\".");
                    continue;
                }

                // If the airway only has 1 segment, then just make the airway out of that
                TRoute route = routeFactory(cleanIdentifier);
                if (currentSegments.Count == 1)
                {
                    NamedSegment segment = currentSegments.First();
                    route.Fixes.Add(FileParserUtils.GetFixFromCoordinate(Result, segment.Start));
                    route.Fixes.Add(FileParserUtils.GetFixFromCoordinate(Result, segment.End));
                }
                else
                {
                    // Otherwise, build the airway like a linked list
                    // Find our first segment (start point doesn't match any of the end points), and our last segment
                    // (end point doesn't match any of the start points)
                    NamedSegment firstSegment = currentSegments.First(s => currentSegments.All(s1 => s1.End != s.Start));
                    NamedSegment lastSegment = currentSegments.First(s => currentSegments.All(s1 => s1.Start != s.End));

                    // Add the first segment to the route
                    route.Fixes.Add(FileParserUtils.GetFixFromCoordinate(Result, firstSegment.Start));
                    route.Fixes.Add(FileParserUtils.GetFixFromCoordinate(Result, firstSegment.End));
                    while (true)
                    {
                        // Find the next segment
                        Fix lastFix = route.Fixes.Last();
                        NamedSegment nextSegment = currentSegments.First(s => s.Start.Equals(lastFix.Location));

                        // Add it in
                        Fix fix = FileParserUtils.GetFixFromCoordinate(Result, nextSegment.End);
                        if (route.Fixes.Any(f => f.Identifier == fix.Identifier))
                        {
                            throw new Exception(
                                $"The fix \"{fix.Identifier}\" was found multiple times in airway \"{route.Identifier}\".");
                        }

                        route.Fixes.Add(fix);

                        // If the next segment is the same as the last segment, then we've finished with this route
                        if (nextSegment == lastSegment) break;
                    }
                }

                routes.Add(route);
            }

            return routes;
        }

        /// <summary>
        ///     Gets a new <see cref="Coordinate"/> instance given a longitude and latitude string.
        ///     This also checks for any matching fixes in the <paramref name="longitude"/> or <see cref="latitude"/>
        ///     components.
        /// </summary>
        /// <param name="longitude">
        ///     The longitude.
        /// </param>
        /// <param name="latitude">
        ///     The latitude.
        /// </param>
        /// <returns>
        ///     The <see cref="Coordinate"/>.
        /// </returns>
        private Coordinate GetCoordinate(string longitude, string latitude)
        {
            CoordinateComponent longitudeComponent = FixIdentifierRegex.Match(longitude).Success ? FileParserUtils.GetFixFromName(Result, longitude).Location.Longitude : CoordinateComponent.Parse(longitude);
            CoordinateComponent latitudeComponent = FixIdentifierRegex.Match(latitude).Success ? FileParserUtils.GetFixFromName(Result, latitude).Location.Latitude : CoordinateComponent.Parse(latitude);

            return new Coordinate(latitudeComponent, longitudeComponent);
        }
    }
}

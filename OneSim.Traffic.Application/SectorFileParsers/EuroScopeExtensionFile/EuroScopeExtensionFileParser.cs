// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EuroScopeExtensionFileParser.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Application.SectorFileParsers.EuroScopeExtensionFile
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using OneSim.Traffic.Application.SectorFileParsers.SectorFile;
    using OneSim.Traffic.Domain.Entities;
    using OneSim.Traffic.Domain.Entities.Ais;

    /// <summary>
    ///     The EuroScope Extension File (.ese) parser.
    /// </summary>
    public class EuroScopeExtensionFileParser
    {
        /// <summary>
        ///     Gets or sets the <see cref="EuroScopeExtensionFileParseResult"/>.
        /// </summary>
        private EuroScopeExtensionFileParseResult Result { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="SectorFileParseResult"/> of the corresponding Sector File.
        /// </summary>
        private SectorFileParseResult SectorFileResult { get; set; }

        /// <summary>
        ///     The <see cref="Regex"/> for the Section header.
        /// </summary>
        private static readonly Regex SectionHeaderRegex = new Regex(@"^\[(.+)\]$", RegexOptions.Compiled);

        /// <summary>
        ///     The current line number.
        /// </summary>
        private int _lineNumber = 0;

        /// <summary>
        ///     The current line content.
        /// </summary>
        private string _currentLine = string.Empty;

        /// <summary>
        ///     The current <see cref="FileSection"/>.
        /// </summary>
        private FileSection _currentSection = FileSection.None;

        /// <summary>
        ///     Gets or sets the <see cref="SectorLine.LineId"/> of the <see cref="SectorLine"/> currently being parsed.
        /// </summary>
        private string CurrentSectorLineName { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="List{T}"/> of found <see cref="SectorLine"/>s.
        /// </summary>
        private List<SectorLine> SectorLines { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="Sector"/> currently being parsed.
        /// </summary>
        private Sector CurrentSector { get; set; }

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
        ///     The EuroScope Extension file content.
        /// </param>
        /// <param name="sectorFileParseResult">
        ///     The <see cref="SectorFileParseResult"/> of the corresponding Sector File.
        /// </param>
        /// <returns>
        ///     The <see cref="EuroScopeExtensionFileParseResult"/>.
        /// </returns>
        public EuroScopeExtensionFileParseResult Parse(string fileContent, SectorFileParseResult sectorFileParseResult)
        {
            Result = new EuroScopeExtensionFileParseResult(sectorFileParseResult.SectorSet);
            SectorFileResult = sectorFileParseResult;
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

                // Look for [SECTION] headers.
                if (_currentLine.Substring(0, 1) == "[")
                {
                    Match match = SectionHeaderRegex.Match(_currentLine);
                    if (match.Success)
                    {
                        // Switch to the new section.
                        string header = match.Groups[1].Value.ToUpper();
                        switch (header)
                        {
                            case "POSITIONS":
                                _currentSection = FileSection.Positions;
                                continue;

                            case "SIDSSTARS":
                                _currentSection = FileSection.SidsStars;
                                continue;

                            case "AIRSPACE":
                                _currentSection = FileSection.Airspace;
                                continue;

                            case "FREETEXT":
                            case "RADAR":
                            case "GROUND":
                                _currentSection = FileSection.Unsupported;
                                continue;

                            default:
                                AddParseError($"Unknown section header encountered ({header}).");
                                continue;
                        }
                    }
                }

                // If we're in an unsupported section, just skip
                if (_currentSection == FileSection.Unsupported) continue;

                // If we get here, and we're not in a [SECTION], then we've found an orphaned line.
                if (_currentSection == FileSection.None)
                {
                    AddParseError("Orphaned line.");
                    continue;
                }

                switch (_currentSection)
                {
                    case FileSection.Positions:
                        ParsePositionLine();
                        break;

                    case FileSection.SidsStars:
                        ParseSidStarLine();
                        break;

                    case FileSection.Airspace:
                        ParseAirspaceLine();
                        break;
                }
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

        /// <summary>
        ///     Parses the <see cref="_currentLine"/> as a SID or a STAR.
        /// </summary>
        private void ParseSidStarLine()
        {
            // Split by ":"
            string[] sections = _currentLine.Split(":");

            if (sections.Length != 5)
            {
                AddParseError(
                    $"Unexpected number of sections in the SID or STAR definition. Expected 5, found {sections.Length}.");
                return;
            }

            TerminalRouteType routeType;
            switch (sections[0])
            {
                case "SID":
                    routeType = TerminalRouteType.StandardInstrumentDeparture;
                    break;

                case "STAR":
                    routeType = TerminalRouteType.StandardTerminalArrivalRoute;
                    break;

                default:
                    AddParseError($"Unexpected route type. Expected \"SID\" or \"STAR\", found \"{sections[0]}\".");
                    return;
            }

            Airport targetAirport = SectorFileResult.Airports.FirstOrDefault(a => a.Identifier == sections[1]);
            if (targetAirport == null)
            {
                AddParseError($"Couldn't find airport {sections[1]} in sector file.");
                return;
            }

            Runway targetRunway = targetAirport.Runways.FirstOrDefault(r => r.Identifier == sections[2]);
            if (targetRunway == null)
            {
                AddParseError($"Couldn't find runway {sections[2]} at {targetAirport.Identifier}.");
                return;
            }

            List<Fix> fixes = sections[4]
                             .Split(" ")
                             .Select(f => FileParserUtils.GetFixFromName(SectorFileResult, f))
                             .ToList();

            TerminalRoute route = new TerminalRoute(sections[3], routeType, fixes, new List<Runway> { targetRunway });

            Result.TerminalRoutes.Add(route);
        }

        /// <summary>
        ///     Parses the <see cref="_currentLine"/> as an Airspace definition.
        /// </summary>
        public void ParseAirspaceLine()
        {
            string[] sections = _currentLine.Split(":");

            string subSectionName = sections[0];
            switch (subSectionName)
            {
                case "SECTORLINE":
                    CurrentSectorLineName = sections[1];
                    SectorLines.Add(new SectorLine(CurrentSectorLineName));
                    break;

                case "CIRCLE_SECTORLINE":

                    Coordinate centerPoint;
                    int radius;
                    if (sections.Length == 5)
                    {
                        centerPoint = new Coordinate(sections[2], sections[3]);
                        radius = int.Parse(sections[4]);
                    }
                    else if (sections.Length == 4)
                    {
                        centerPoint = FileParserUtils.GetFixFromName(SectorFileResult, sections[2]).Location;
                        radius = int.Parse(sections[3]);
                    }
                    else
                    {
                        AddParseError($"Unexpected amount of sections in CIRCLE_SECTORLINE. Expected 4 or 5, found {sections.Length}.");
                        return;
                    }

                    const int ArcResolution = 1;
                    List<Coordinate> points = new List<Coordinate>();
                    for (int i = 0; i < 360; i += ArcResolution)
                    {
                        // Todo: Defined in two places, move somewhere more common
                        const int NauticalMilesPerDegree = 60;
                        double latitude = centerPoint.GetPoint().Latitude + ((radius * NauticalMilesPerDegree) * Math.Cos(i));
                        double longitude = centerPoint.GetPoint().Longitude + ((radius * NauticalMilesPerDegree) * Math.Sin(i));
                        points.Add(new Coordinate(latitude, longitude));
                    }

                    SectorLine circleLine = new SectorLine(sections[0]);
                    circleLine.Points.AddRange(points);

                    SectorLines.Add(circleLine);
                    break;

                case "COORD":
                    Coordinate point = new Coordinate(sections[1], sections[2]);
                    SectorLines.First(l => l.LineId == CurrentSectorLineName).Points.Add(point);
                    break;

                case "SECTOR":

                    // Finish off the current sector
                    if (CurrentSector != null) Result.Sectors.Add(CurrentSector);
                    CurrentSector = new Sector
                                    {
                                        Identifier = sections[1],
                                        LowerLevel = int.Parse(sections[2]),
                                        UpperLevel = int.Parse(sections[3]),
                                    };

                    break;

                case "OWNER":
                    if (CurrentSector == null)
                    {
                        AddParseError("Orphaned Sector Owner Definition.");
                        return;
                    }

                    for (int i = 1; i < sections.Length; i++)
                    {
                        string sectorId = sections[i];
                        ControllerPosition position =
                            Result.ControllerPositions.FirstOrDefault(p => p.SectorId == sectorId);

                        if (position == null)
                        {
                            AddParseError($"No Position with ID \"{sectorId}\" was defined.");
                            return;
                        }

                        CurrentSector.Positions.Add(new ControllerPriority { Position = position, Priority = i });
                    }

                    break;

                case "BORDER":
                    if (CurrentSector == null)
                    {
                        AddParseError("Orphaned Sector Border Definition.");
                        return;
                    }

                    for (int i = 1; i < sections.Length; i++)
                    {
                        string sectorLineId = sections[i];
                        SectorLine line = SectorLines.FirstOrDefault(l => l.LineId == sectorLineId);

                        if (line == null)
                        {
                            AddParseError($"No Sector Line with ID \"{sectorLineId}\" was defined.");
                            return;
                        }

                        CurrentSector.Border.AddRange(line.Points);
                    }

                    break;

                case "ACTIVE":
                    if (CurrentSector == null)
                    {
                        AddParseError("Orphaned Sector Active Runway Definition.");
                        return;
                    }

                    string icaoCode = sections[1];
                    string runwayId = sections[2];

                    Airport targetAirport = SectorFileResult.Airports.FirstOrDefault(a => a.Identifier == icaoCode);
                    if (targetAirport == null)
                    {
                        AddParseError($"No Airport with ID \"{icaoCode}\" was defined.");
                        return;
                    }

                    Runway targetRunway = targetAirport.Runways.FirstOrDefault(r => r.Identifier == runwayId);
                    if (targetRunway == null)
                    {
                        AddParseError($"No Runway with ID \"{runwayId}\" was defined.");
                        return;
                    }

                    CurrentSector.ActiveRunways.Add(targetRunway);

                    break;

                // Ignored sub-sections
                case "DISPLAY":
                case "DISPLAY_SECTORLINE":
                case "ALTOWNER":
                    break;
            }
        }
    }
}

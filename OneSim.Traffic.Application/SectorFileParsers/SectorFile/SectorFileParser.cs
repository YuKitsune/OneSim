// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectorFileParser.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Application.SectorFileParsers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using OneSim.Traffic.Application.SectorFileParsers.SectorFile;
    using OneSim.Traffic.Domain.Entities;
    using OneSim.Traffic.Domain.Entities.Ais;

    /// <summary>
    ///     The Sector File (.sct or .stc2) parser.
    /// </summary>
    /// <remarks>
    ///     The GEO, REGIONS and LABELS sections, as well as color definitions are ignored, as OneSim does not have a
    ///     use for them.
    /// </remarks>
    public class SectorFileParser : IDisposable
    {
        /// <summary>
        ///     Gets or sets the current <see cref="SectorSet"/>.
        /// </summary>
        private SectorSet SectorSet { get; set; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="Airport"/>s.
        /// </summary>
        private List<Airport> Airports { get; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="Fix"/>es.
        /// </summary>
        private List<Fix> Fixes { get; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="Navaid"/>s.
        /// </summary>
        private List<Navaid> Navaids { get; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="NamedSegment"/>s which were found in the low-level
        ///     <see cref="Airway"/> section.
        /// </summary>
        private List<NamedSegment> LowAirwaySegments { get; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of low-level <see cref="Airway"/>.
        /// </summary>
        private List<Airway> LowAirways { get; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="NamedSegment"/>s which were found in the
        ///     high-level <see cref="Airway"/> section.
        /// </summary>
        private List<NamedSegment> HighAirwaySegments { get; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of high-level <see cref="Airway"/>.
        /// </summary>
        private List<Airway> HighAirways { get; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="NamedSegment"/> which were found in the
        ///     STARs section.
        /// </summary>
        private List<NamedSegment> StarSegments { get; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="TerminalRoute"/>s which are <see cref="TerminalRouteType.StandardTerminalArrivalRoute"/>s.
        /// </summary>
        private List<TerminalRoute> Stars { get; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="NamedSegment"/> which were found in the
        ///     SIDs section.
        /// </summary>
        private List<NamedSegment> SidSegments { get; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="TerminalRoute"/>s which are <see cref="TerminalRouteType.StandardInstrumentDepartures"/>s.
        /// </summary>
        private List<TerminalRoute> Sids { get; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="ParseError"/>s.
        /// </summary>
        private List<ParseError> ParseErrors { get; }

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
        private static readonly Regex BundarySegmentRegex = new Regex(@"^(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)", RegexOptions.Compiled);

        /// <summary>
        ///     The <see cref="Regex"/> for the diagrams headers.
        /// </summary>
        private static readonly Regex DiagramBeginRegex = new Regex(@"^(.{26})\s*(\S+)\s+(\S+)\s+(\S+)\s+(\S+)(?:\s+(\S+))?", RegexOptions.Compiled);

        /// <summary>
        ///     The <see cref="Regex"/> for the diagrams continuations.
        /// </summary>
        private static readonly Regex DiagramContinuationRegex = new Regex(@"^\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)(?:\s+(\S+))?", RegexOptions.Compiled);

        /// <summary>
        ///     The <see cref="CultureInfo"/>.
        /// </summary>
        private readonly CultureInfo _culture = new CultureInfo("en-US");

        /// <summary>
        ///     The current <see cref="Match"/>.
        /// </summary>
        private Match _currentMatch;

        /// <summary>
        ///     The <see cref="Dictionary{TKey,TValue}"/> of parsed <see cref="Fix"/>es.
        /// </summary>
        private readonly Dictionary<string, Fix> _fixes = new Dictionary<string, Fix>();

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
        ///     Initializes a new instance of the <see cref="SectorFileParser"/> class.
        /// </summary>
        public SectorFileParser()
        {
            Airports = new List<Airport>();
            Fixes = new List<Fix>();
            Navaids = new List<Navaid>();

            LowAirwaySegments = new List<NamedSegment>();
            LowAirways = new List<Airway>();
            HighAirwaySegments = new List<NamedSegment>();
            HighAirways = new List<Airway>();

            StarSegments = new List<NamedSegment>();
            Stars = new List<TerminalRoute>();
            SidSegments = new List<NamedSegment>();
            Sids = new List<TerminalRoute>();

            ParseErrors = new List<ParseError>();
        }

        /// <summary>
        ///     Adds a new <see cref="ParseError"/> to the error list.
        /// </summary>
        /// <param name="message">
        ///     The error message.
        /// </param>
        private void AddParseError(string message) => ParseErrors.Add(new ParseError(_lineNumber, _currentLine, message));

        /// <summary>
        ///     Determines whether or not the given <see cref="char"/> is whitespace.
        /// </summary>
        /// <param name="c">
        ///     The <see cref="char"/>.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="c"/> is whitespace, <c>false</c> otherwise.
        /// </returns>
        private static bool IsWhitespace(char c) => c == ' ' || c == '\t' || c == '\r' || c == '\n' || c == '\f';

        /// <summary>
        ///     Strips any comments from the given <paramref name="line"/>.
        /// </summary>
        /// <param name="line">
        ///     The <see cref="string"/> to strip the comments from.
        /// </param>
        private static void StripComments(ref string line)
        {
            int pos = line.IndexOf(';');
            if (pos > -1) line = line.Substring(0, pos);
            pos = line.IndexOf("//");
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
        private int ParseFrequency(string s)
        {
            // Frequencies are stored in integer form.
            double freq = 0.0;
            double.TryParse(s, NumberStyles.Float, _culture, out freq);
            return (int)(freq * 1000);
        }

        /// <summary>
        ///     Translates the given <see cref="string"/> inta coordinate.
        /// </summary>
        /// <param name="s">
        ///     The <see cref="string"/> to parse.
        /// </param>
        /// <param name="wantLat">
        ///     Whether or not we're looking for the latitude.
        /// </param>
        /// <returns>
        ///     The coordinate in the form of a <see cref="double"/>.
        /// </returns>
        private double TranslateCoordinate(string s, bool wantLat)
        {
            // Make sure we got a value.
            if (string.IsNullOrEmpty(s))
            {
                RaiseParseError("Empty lat/lon value.");
                return 0.0;
            }

            // If the string contains a decimal point, we assume it is in Degrees/Minutes/Seconds format,
            // in which case we translate to decimal. Otherwise, we look it up in the fixes table.
            if (s.IndexOfAny(new char[] { ',', '.' }) > -1)
            {
                return DmsToDecimal(s);
            }
            else
            {
                return GetCoordFromFix(s, wantLat);
            }
        }

        /// <summary>
        ///     Converts a DMS coordinate to a <see cref="double"/>.
        /// </summary>
        /// <param name="s">
        ///     The DMS coordinate in the form of a <see cref="string"/>.
        /// </param>
        /// <returns>
        ///     The coordinate in the form of a <see cref="double"/>.
        /// </returns>
        private double DmsToDecimal(string s)
        {
            // Make sure we got a value.
            if (!string.IsNullOrEmpty(s))
            {
                // Find out which side of the axis we're on, then strip out the N, S, E or W.
                bool neg = s.IndexOfAny(new char[] { 'S', 's', 'W', 'w' }) > -1;
                s = s.Substring(1);

                // Get the whole degrees portion.
                char[] sep = new char[] { '.', ',' };
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
                                double.TryParse(sec, NumberStyles.Float, _culture, out double seconds);

                                // Do the math.
                                double result = (double)degrees;
                                result += (double)minutes / 60.0;
                                result += seconds / 3600.0;

                                // Return the result, negated if necessary.
                                return Math.Round(neg ? (result * -1.0) : result, 7);
                            }
                        }
                    }
                }
            }

            // If we fall through to this line, then there was a problem with the value.
            AddParseError($"Invalid formatting in lat/lon value: {s}");
            return 0.0;
        }

        /// <summary>
        ///     Gets the coordinate from a given <see cref="Fix"/>.
        /// </summary>
        /// <param name="s">
        ///     The <see cref="Fix.Identifier"/>.
        /// </param>
        /// <param name="wantLat">
        ///     Whether or not we're looking for the latitude.
        /// </param>
        /// <returns>
        ///     The coordinate in the form of a <see cref="double"/>.
        /// </returns>
        private double GetCoordFromFix(string s, bool wantLat)
        {
            s = s.ToUpper();
            if (_fixes.ContainsKey(s))
            {
                return wantLat ? _fixes[s].Location.Latitude : _fixes[s].Location.Longitude;
            }
            else
            {
                RaiseParseError($"Unknown fix name: {s}");
                return 0.0;
            }
        }

        public void Parse(Stream stream)
        {
            SectorSet = new SectorSet();
            using (StreamReader sr = new StreamReader(stream))
            {
                int infoSectionLine = 0;
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
                        if (_currentLine.Trim().Substring(0, 1) == ";" || _currentLine.Trim().Substring(0, 1) == "//") continue;

                        // Strip off trailing comments.
                        StripComments(ref _currentLine);

                        // Trim trailing whitespace from the line.
                        _currentLine = _currentLine.TrimEnd(new char[] { ' ', '\t', '\r', '\n', '\f' });

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
                                    case "ARTCC":
                                        _currentSection = FileSection.ARTCC;
                                        continue;
                                    case "ARTCC HIGH":
                                        _currentSection = FileSection.ArtccHigh;
                                        continue;
                                    case "ARTCC LOW":
                                        _currentSection = FileSection.ArtccLow;
                                        continue;
                                    case "SID":
                                        _currentSection = FileSection.SID;
                                        continue;
                                    case "STAR":
                                        _currentSection = FileSection.STAR;
                                        continue;
                                    case "LOW AIRWAY":
                                        _currentSection = FileSection.LowAirway;
                                        continue;
                                    case "HIGH AIRWAY":
                                        _currentSection = FileSection.HighAirway;
                                        continue;
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
                                        ParseDiagramLine();
                                        break;
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
                LowAirways.AddRange(GetAirwaysFromSegments(LowAirwaySegments));
                HighAirways.AddRange(GetAirwaysFromSegments(HighAirwaySegments));

                // Convert segments to SIDs and STARs
            }
        }

        private void ParseInfoLine(int infoLine)
        {
            switch (infoLine)
            {
                case 1:
                    SectorSet.Name = _currentLine;
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
                NavaidType type = _currentSection switch
                {
                    FileSection.VOR => NavaidType.VOR,
                    FileSection.NDB => NavaidType.NDB,
                    _ => throw new InvalidOperationException("Invalid file section.")
                };

                string id = _currentMatch.Groups[1].Value.ToUpper();
                Navaid n = new Navaid(
                    id,
                    new Point2D(
                        DmsToDecimal(_currentMatch.Groups[4].Value),
                        DmsToDecimal(_currentMatch.Groups[3].Value)),
                    ParseFrequency(_currentMatch.Groups[2].Value),
                    type);
                if (_fixes.ContainsKey(id))
                {
                    _fixes[id] = n;
                }
                else
                {
                    _fixes.Add(id, n);
                }

                Navaids.Add(n);
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
                string id = _currentMatch.Groups[1].Value.ToUpper();
                Airport a = new Airport(
                    id,
                    new Point2D(
                        DmsToDecimal(_currentMatch.Groups[4].Value),
                        DmsToDecimal(_currentMatch.Groups[3].Value)));

                // Todo: CTAF: ParseFrequency(mMatch.Groups[2].Value),
                if (!string.IsNullOrEmpty(_currentMatch.Groups[5].Value)) a.Class = (AirspaceClass)Enum.Parse(typeof(AirspaceClass), _currentMatch.Groups[5].Value);

                if (_fixes.ContainsKey(id))
                {
                    _fixes[id] = a;
                }
                else
                {
                    _fixes.Add(id, a);
                }

                Airports.Add(a);
            }
            else
            {
                AddParseError($"Unrecognized formatting in {_currentSection} section.");
            }
        }

        private void ParseFixLine()
        {
            _currentMatch = FixRegex.Match(_currentLine);
            if (_currentMatch.Success)
            {
                string id = _currentMatch.Groups[1].Value.ToUpper();
                Fix f = new Fix(
                    id,
                    new Point2D(
                        DmsToDecimal(_currentMatch.Groups[3].Value),
                        DmsToDecimal(_currentMatch.Groups[2].Value)));
                if (_fixes.ContainsKey(id))
                {
                    _fixes[id] = f;
                }
                else
                {
                    _fixes.Add(id, f);
                }

                Fixes.Add(f);
            }
            else
            {
                AddParseError($"Unrecognized formatting in {_currentSection} section.");
            }
        }

        private void ParseRunwayLine()
        {
            _currentMatch = RunwayRegex.Match(_currentLine);
            if (_currentMatch.Success)
            {
                int hdg1 = 0;
                int.TryParse(_currentMatch.Groups[3].Value, out hdg1);
                int hdg2 = 0;
                int.TryParse(_currentMatch.Groups[4].Value, out hdg2);
                Runway r1 = new Runway(
                    _currentMatch.Groups[1].Value.ToUpper(),
                    new Point2D(
                        TranslateCoordinate(_currentMatch.Groups[6].Value, false),
                        TranslateCoordinate(_currentMatch.Groups[5].Value, true)),
                    hdg1);
                Runway r2 = new Runway(
                    _currentMatch.Groups[2].Value.ToUpper(),
                    new Point2D(
                        TranslateCoordinate(_currentMatch.Groups[8].Value, false),
                        TranslateCoordinate(_currentMatch.Groups[7].Value, true)),
                    hdg2);

                // Todo: Figure out how to get the closest airport
                if (RunwayFound != null) RunwayFound(this, new DataFoundEventArgs<Runway>(r));
            }
            else
            {
                AddParseError($"Unrecognized formatting in {_currentSection} section.");
            }
        }

        private void ParseNamedSegmentLine()
        {
            _currentMatch = BundarySegmentRegex.Match(_currentLine);
            if (_currentMatch.Success)
            {
                NamedSegment segment = new NamedSegment(
                    _currentMatch.Groups[1].Value,
                    new Point2D(
                        TranslateCoordinate(_currentMatch.Groups[3].Value, false),
                        TranslateCoordinate(_currentMatch.Groups[2].Value, true)),
                    new Point2D(
                        TranslateCoordinate(_currentMatch.Groups[5].Value, false),
                        TranslateCoordinate(_currentMatch.Groups[4].Value, true)));

                // Get the airway if it already exists
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
                        HighAirwaySegments.Add(segment);
                        break;

                    case FileSection.LowAirway:
                        LowAirwaySegments.Add(segment);
                        break;
                }
            }
            else
            {
                AddParseError($"Unrecognized formatting in {_currentSection} section.");
            }
        }

        /// <summary>
        ///     Parses the <see cref="_currentLine"/> as a diagram, but stores it as a <see cref="NamedSegment"/>.
        /// </summary>
        private void ParseDiagramLine()
        {
            // Check if the line starts with whitespace. If not, it's the start of a new diagram.
            // If so, it's a continuation of an existing diagram definition.
            NamedSegment segment = null;
            if (!IsWhitespace(_currentLine[0]))
            {
                _currentMatch = DiagramBeginRegex.Match(_currentLine);
                if (_currentMatch.Success)
                {
                    segment = new NamedSegment(
                        _currentMatch.Groups[1].Value.Trim(),
                        new Point2D(
                            TranslateCoordinate(_currentMatch.Groups[3].Value, false),
                            TranslateCoordinate(_currentMatch.Groups[2].Value, true)),
                        new Point2D(
                            TranslateCoordinate(_currentMatch.Groups[5].Value, false),
                            TranslateCoordinate(_currentMatch.Groups[4].Value, true)));
                }
                else
                {
                    AddParseError($"Unrecognized formatting in {_currentSection} section.");
                }
            }
            else
            {
                _currentMatch = DiagramContinuationRegex.Match(_currentLine);
                if (_currentMatch.Success)
                {
                    // Get the identifier of the last SID or STAR
                    string lastIdentifier;
                    switch (_currentSection)
                    {
                        case FileSection.SID:
                            lastIdentifier = SidSegments.Last().Label;
                            break;

                        case FileSection.STAR:
                            lastIdentifier = StarSegments.Last().Label;
                            break;

                        default:
                            AddParseError("Found diagram continuation line without prior diagram start line.");
                            return;
                    }

                    segment = new NamedSegment(
                        lastIdentifier,
                        new Point2D(
                            TranslateCoordinate(_currentMatch.Groups[2].Value, false),
                            TranslateCoordinate(_currentMatch.Groups[1].Value, true)),
                        new Point2D(
                            TranslateCoordinate(_currentMatch.Groups[4].Value, false),
                            TranslateCoordinate(_currentMatch.Groups[3].Value, true)));
                }
                else
                {
                    AddParseError($"Unrecognized formatting in {_currentSection} section.");
                }
            }

            if (segment == null) return;
            switch (_currentSection)
            {
                case FileSection.SID:
                    SidSegments.Add(segment);
                    break;

                case FileSection.STAR:
                    SidSegments.Add(segment);
                    break;

                default:
                    AddParseError("Found diagram continuation line without prior diagram start line.");
                    return;
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
                route.Fixes.Add(GetFixFromPoint(firstSegment.Start));
                route.Fixes.Add(GetFixFromPoint(firstSegment.End));
                while (true)
                {
                    // Find the next segment
                    Fix lastFix = route.Fixes.Last();
                    NamedSegment nextSegment = currentSegments.First(s => s.Start == lastFix.Location);

                    // Add it in
                    route.Fixes.Add(GetFixFromPoint(nextSegment.End));

                    // If the next segment is the same as the last segment, then we've finished with this route
                    if (nextSegment == lastSegment) break;
                }

                routes.Add(route);
            }


        private Fix GetFixFromPoint(Point2D point)
        {
            throw new NotImplementedException();
            return routes;
        }
    }
}

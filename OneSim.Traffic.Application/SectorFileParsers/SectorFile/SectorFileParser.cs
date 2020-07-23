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
    using System.Text.RegularExpressions;

    using OneSim.Traffic.Application.SectorFileParsers.SectorFile;
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
        public event EventHandler<DataFoundEventArgs<SectorInfo>> SectorInfoFound;
        public event EventHandler<DataFoundEventArgs<LabeledSegment>> ArtccSegmentFound;
        public event EventHandler<DataFoundEventArgs<LabeledSegment>> ArtccLowSegmentFound;
        public event EventHandler<DataFoundEventArgs<LabeledSegment>> ArtccHighSegmentFound;

        /// <summary>
        ///     Gets or sets the <see cref="List{T}"/> of <see cref="Airport"/>s.
        /// </summary>
        private List<Airport> Airports { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="List{T}"/> of <see cref="Fix"/>es.
        /// </summary>
        private List<Fix> Fixes { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="List{T}"/> of <see cref="Navaid"/>s.
        /// </summary>
        private List<Navaid> Navaids { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="List{T}"/> of low-level <see cref="Airway"/>.
        /// </summary>
        private List<Airway> LowAirways { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="List{T}"/> of high-level <see cref="Airway"/>.
        /// </summary>
        private List<Airway> HighAirways { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="List{T}"/> of <see cref="TerminalRoute"/>s which are <see cref="TerminalRouteType.StandardTerminalArrivalRoute"/>s.
        /// </summary>
        private List<TerminalRoute> Stars { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="List{T}"/> of <see cref="TerminalRoute"/>s which are <see cref="TerminalRouteType.StandardInstrumentDepartures"/>s.
        /// </summary>
        private List<TerminalRoute> Sids { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="List{T}"/> of <see cref="ParseError"/>s.
        /// </summary>
        private List<ParseError> ParseErrors { get; set; }

        ////////////////////////////////////////////////////////////////////////////////
        // Fields:

        private static Regex sRegexColorDefinition = new Regex(@"^#define\s+(\S+)\s+(.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex sRegexSectionHeader = new Regex(@"^\[(.+)\]$", RegexOptions.Compiled);
        private static Regex sRegexVorNdb = new Regex(@"^(\S+)\s+(\S+)\s+(\S+)\s+(\S+)", RegexOptions.Compiled);
        private static Regex sRegexFix = new Regex(@"^(\S+)\s+(\S+)\s+(\S+)", RegexOptions.Compiled);
        private static Regex sRegexAirport = new Regex(@"^(\S+)\s+(\S+)\s+(\S+)\s+(\S+)(?:\s+([A-Z]))?", RegexOptions.Compiled);
        private static Regex sRegexRunway = new Regex(@"^(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)", RegexOptions.Compiled);
        private static Regex sRegexBoundarySegment = new Regex(@"^(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)", RegexOptions.Compiled);
        private static Regex sRegexDiagramBegin = new Regex(@"^(.{26})\s*(\S+)\s+(\S+)\s+(\S+)\s+(\S+)(?:\s+(\S+))?", RegexOptions.Compiled);
        private static Regex sRegexDiagramContinuation = new Regex(@"^\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)(?:\s+(\S+))?", RegexOptions.Compiled);
        private static Regex sRegexGeoSegment = new Regex(@"^(\S+)\s+(\S+)\s+(\S+)\s+(\S+)(?:\s+(\S+))?", RegexOptions.Compiled);
        private static Regex sRegexRegionBegin = new Regex(@"^(\S+)\s+(\S+)\s+(\S+)", RegexOptions.Compiled);
        private static Regex sRegexRegionContinuation = new Regex(@"^\s+(\S+)\s+(\S+)", RegexOptions.Compiled);
        private static Regex sRegexStaticText = new Regex(@"^""(.+)""\s+(\S+)\s+(\S+)\s+(\S+)", RegexOptions.Compiled);
        private CultureInfo mCulture = new CultureInfo("en-US");
        private Match mMatch;
        private Dictionary<string, Fix> mFixes = new Dictionary<string, Fix>();
        private int mLineNumber = 0;
        private string mCurrentLine = string.Empty;
        private FileSection mCurrentSection = FileSection.None;
        private SectorInfo mSectorInfo;
        private Region mCurrentRegion;
        private Diagram mCurrentDiagram;

        /// <summary>
        ///     Adds a new <see cref="ParseError"/> to the error list.
        /// </summary>
        /// <param name="message">
        ///     The error message.
        /// </param>
        private void RaiseParseError(string message) => ParseErrors.Add(new ParseError(mLineNumber, mCurrentLine, message));

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
            double.TryParse(s, NumberStyles.Float, mCulture, out freq);
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
        ///     Converts a DMS cooridinate to a <see cref="double"/>.
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
                                int degrees = 0;
                                int minutes = 0;
                                double seconds = 0.0f;
                                int.TryParse(deg, out degrees);
                                int.TryParse(min, out minutes);
                                double.TryParse(sec, NumberStyles.Float, mCulture, out seconds);

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
            RaiseParseError(string.Format("Invalid formatting in lat/lon value: {0}", s));
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
            if (mFixes.ContainsKey(s))
            {
                return wantLat ? mFixes[s].Location.Latitude : mFixes[s].Location.Longitude;
            }
            else
            {
                RaiseParseError(string.Format("Unknown fix name: {0}", s));
                return 0.0;
            }
        }

        public void Parse(Stream stream)
        {
            mSectorInfo = new SectorInfo();
            using (StreamReader sr = new StreamReader(stream))
            {
                int infoSectionLine = 0;
                for (int pass = 1; pass <= 2; pass++)
                {
                    mCurrentSection = FileSection.None;
                    mLineNumber = 0;

                    // Rewind the file for pass 2.
                    if (pass == 2) sr.BaseStream.Seek(0, SeekOrigin.Begin);

                    // Step through the file.
                    while ((mCurrentLine = sr.ReadLine()) != null)
                    {
                        mLineNumber++;

                        // Skip empty lines.
                        if (string.IsNullOrEmpty(mCurrentLine.Trim())) continue;

                        // Skip lines that contain only a comment.
                        if (mCurrentLine.Trim().Substring(0, 1) == ";" || mCurrentLine.Trim().Substring(0, 1) == "//") continue;

                        // Strip off trailing comments.
                        StripComments(ref mCurrentLine);

                        // Trim trailing whitespace from the line.
                        mCurrentLine = mCurrentLine.TrimEnd(new char[] { ' ', '\t', '\r', '\n', '\f' });

                        // If the line ends up empty, skip it.
                        if (string.IsNullOrEmpty(mCurrentLine)) continue;

                        // Look for [SECTION] headers.
                        if (mCurrentLine.Substring(0, 1) == "[")
                        {
                            mMatch = sRegexSectionHeader.Match(mCurrentLine);
                            if (mMatch.Success)
                            {
                                // Finish up any pending multi-line objects.
                                FinalizePendingObjects();

                                // Switch to the new section.
                                switch (mMatch.Groups[1].Value.ToUpper())
                                {
                                    case "INFO":
                                        mCurrentSection = FileSection.Info;
                                        infoSectionLine = 0;
                                        continue;
                                    case "VOR":
                                        mCurrentSection = FileSection.VOR;
                                        continue;
                                    case "NDB":
                                        mCurrentSection = FileSection.NDB;
                                        continue;
                                    case "AIRPORT":
                                        mCurrentSection = FileSection.Airport;
                                        continue;
                                    case "RUNWAY":
                                        mCurrentSection = FileSection.Runway;
                                        continue;
                                    case "FIXES":
                                        mCurrentSection = FileSection.Fixes;
                                        continue;
                                    case "ARTCC":
                                        mCurrentSection = FileSection.ARTCC;
                                        continue;
                                    case "ARTCC HIGH":
                                        mCurrentSection = FileSection.ArtccHigh;
                                        continue;
                                    case "ARTCC LOW":
                                        mCurrentSection = FileSection.ArtccLow;
                                        continue;
                                    case "SID":
                                        mCurrentSection = FileSection.SID;
                                        continue;
                                    case "STAR":
                                        mCurrentSection = FileSection.STAR;
                                        continue;
                                    case "GEO":
                                        mCurrentSection = FileSection.Geo;
                                        continue;
                                    case "LOW AIRWAY":
                                        mCurrentSection = FileSection.LowAirway;
                                        continue;
                                    case "HIGH AIRWAY":
                                        mCurrentSection = FileSection.HighAirway;
                                        continue;
                                    case "REGIONS":
                                        mCurrentSection = FileSection.Regions;
                                        continue;
                                    case "LABELS":
                                        mCurrentSection = FileSection.Labels;
                                        continue;
                                    default:
                                        if (pass == 1) RaiseParseError("Unknown section header encountered.");
                                        continue;
                                }
                            }
                        }

                        // If we get here, and we're not in a [SECTION], then we've found an orphaned line.
                        if (mCurrentSection == FileSection.None)
                        {
                            if (pass == 1) RaiseParseError("Orphaned line.");
                            continue;
                        }

                        // If we get this far, we've got a data line. Call the appropriate parsing
                        // method based on the current pass and section.
                        switch (pass)
                        {
                            case 1:
                                switch (mCurrentSection)
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
                                switch (mCurrentSection)
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
                                    case FileSection.Geo:
                                        ParseGeoLine();
                                        break;
                                    case FileSection.Regions:
                                        ParseRegionLine();
                                        break;
                                    case FileSection.SID:
                                    case FileSection.STAR:
                                        ParseDiagramLine();
                                        break;
                                    case FileSection.Labels:
                                        ParseLabelLine();
                                        break;
                                }

                                break;
                        }
                    }
                }

                // Finish up any pending multi-line objects.
                FinalizePendingObjects();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////

        private void ParseInfoLine(int infoLine)
        {
            switch (infoLine)
            {
                case 1:
                    mSectorInfo.SectorName = mCurrentLine;
                    break;
                case 2:
                case 3:
                    break;
                case 4:
                    mSectorInfo.Center.Lat = DmsToDecimal(mCurrentLine);
                    break;
                case 5:
                    mSectorInfo.Center.Lon = DmsToDecimal(mCurrentLine);
                    break;
                case 6:
                {
                    double d = 0.0;
                    double.TryParse(mCurrentLine, NumberStyles.Float, mCulture, out d);
                    mSectorInfo.MilesPerDegLat = d;
                    break;
                }

                case 7:
                {
                    double d = 0.0;
                    double.TryParse(mCurrentLine, NumberStyles.Float, mCulture, out d);
                    mSectorInfo.MilesPerDegLon = d;
                    break;
                }

                case 8:
                {
                    double d = 0.0;
                    double.TryParse(mCurrentLine, NumberStyles.Float, mCulture, out d);
                    mSectorInfo.MagneticVariation = d;
                    break;
                }

                case 9:
                {
                    double d = 0.0;
                    double.TryParse(mCurrentLine, NumberStyles.Float, mCulture, out d);
                    mSectorInfo.Scale = d;
                    if (SectorInfoFound != null) SectorInfoFound(this, new DataFoundEventArgs<SectorInfo>(mSectorInfo));
                    break;
                }

                default:
                    RaiseParseError("Extra line in [INFO] section.");
                    break;
            }
        }

        /// <summary>
        ///     Parses the <see cref="mCurrentLine"/> as a <see cref="Navaid"/>.
        /// </summary>
        private void ParseVorNdbLine()
        {
            mMatch = sRegexVorNdb.Match(mCurrentLine);
            if (mMatch.Success)
            {
                NavaidType type;
                if (mCurrentSection == FileSection.VOR)
                {
                    type = NavaidType.VOR;
                }
                else if (mCurrentSection == FileSection.NDB)
                {
                    type = NavaidType.NDB;
                }
                else
                {
                    throw new InvalidOperationException("Invalid file section.");
                }

                string id = mMatch.Groups[1].Value.ToUpper();
                Navaid n = new Navaid(
                    id,
                    new Domain.Entities.Point2D(DmsToDecimal(mMatch.Groups[4].Value), DmsToDecimal(mMatch.Groups[3].Value)),
                    ParseFrequency(mMatch.Groups[2].Value),
                    type);
                if (mFixes.ContainsKey(id))
                {
                    mFixes[id] = n;
                }
                else
                {
                    mFixes.Add(id, n);
                }

                Navaids.Add(n);
            }
            else
            {
                RaiseParseError(string.Format("Unrecognized formatting in {0} section.", mCurrentSection));
            }
        }

        /// <summary>
        ///      Parses the <see cref="mCurrentLine"/> as an <see cref="Airport"/>.
        /// </summary>
        private void ParseAirportLine()
        {
            mMatch = sRegexAirport.Match(mCurrentLine);
            if (mMatch.Success)
            {
                string id = mMatch.Groups[1].Value.ToUpper();
                Airport a = new Airport(
                    id,
                    new Domain.Entities.Point2D(DmsToDecimal(mMatch.Groups[4].Value), DmsToDecimal(mMatch.Groups[3].Value)));

                // Todo: CTAF: ParseFrequency(mMatch.Groups[2].Value),
                if (!string.IsNullOrEmpty(mMatch.Groups[5].Value)) a.Class = (AirspaceClass)Enum.Parse(typeof(AirspaceClass);

                if (mFixes.ContainsKey(id))
                {
                    mFixes[id] = a;
                }
                else
                {
                    mFixes.Add(id, a);
                }

                Airports.Add(a);
            }
            else
            {
                RaiseParseError(string.Format("Unrecognized formatting in {0} section.", mCurrentSection));
            }
        }

        ////////////////////////////////////////////////////////////////////////////////

        private void ParseFixLine()
        {
            mMatch = sRegexFix.Match(mCurrentLine);
            if (mMatch.Success)
            {
                string id = mMatch.Groups[1].Value.ToUpper();
                Fix f = new Fix(
                    id,
                    new SectorPoint(DmsToDecimal(mMatch.Groups[3].Value), DmsToDecimal(mMatch.Groups[2].Value))
                );
                if (mFixes.ContainsKey(id))
                {
                    mFixes[id] = f;
                }
                else
                {
                    mFixes.Add(id, f);
                }

                if (FixFound != null) FixFound(this, new DataFoundEventArgs<Fix>(f));
            }
            else
            {
                RaiseParseError(string.Format("Unrecognized formatting in {0} section.", mCurrentSection));
            }
        }

        ////////////////////////////////////////////////////////////////////////////////

        private void ParseRunwayLine()
        {
            mMatch = sRegexRunway.Match(mCurrentLine);
            if (mMatch.Success)
            {
                int hdg1 = 0;
                int.TryParse(mMatch.Groups[3].Value, out hdg1);
                int hdg2 = 0;
                int.TryParse(mMatch.Groups[4].Value, out hdg2);
                Runway r = new Runway(
                    mMatch.Groups[1].Value.ToUpper(),
                    mMatch.Groups[2].Value.ToUpper(),
                    hdg1,
                    hdg2,
                    new SectorPoint(TranslateCoordinate(mMatch.Groups[6].Value, false), TranslateCoordinate(mMatch.Groups[5].Value, true)),
                    new SectorPoint(TranslateCoordinate(mMatch.Groups[8].Value, false), TranslateCoordinate(mMatch.Groups[7].Value, true))
                );
                if (RunwayFound != null) RunwayFound(this, new DataFoundEventArgs<Runway>(r));
            } else {
                RaiseParseError(string.Format("Unrecognized formatting in {0} section.", mCurrentSection));
            }
        }

        ////////////////////////////////////////////////////////////////////////////////

        private void ParseNamedSegmentLine()
        {
            mMatch = sRegexBoundarySegment.Match(mCurrentLine);
            if (mMatch.Success)
            {
                LabeledSegment segment = new LabeledSegment(
                    mMatch.Groups[1].Value,
                    new SectorPoint(TranslateCoordinate(mMatch.Groups[3].Value, false), TranslateCoordinate(mMatch.Groups[2].Value, true)),
                    new SectorPoint(TranslateCoordinate(mMatch.Groups[5].Value, false), TranslateCoordinate(mMatch.Groups[4].Value, true))
                );
                switch (mCurrentSection)
                {
                    case FileSection.ARTCC:
                        if (ArtccSegmentFound != null) ArtccSegmentFound(this, new DataFoundEventArgs<LabeledSegment>(segment));
                        break;
                    case FileSection.ArtccHigh:
                        if (ArtccHighSegmentFound != null) ArtccHighSegmentFound(this, new DataFoundEventArgs<LabeledSegment>(segment));
                        break;
                    case FileSection.ArtccLow:
                        if (ArtccLowSegmentFound != null) ArtccLowSegmentFound(this, new DataFoundEventArgs<LabeledSegment>(segment));
                        break;
                    case FileSection.HighAirway:
                        if (HighAirwaySegmentFound != null) HighAirwaySegmentFound(this, new DataFoundEventArgs<LabeledSegment>(segment));
                        break;
                    case FileSection.LowAirway:
                        if (LowAirwaySegmentFound != null) LowAirwaySegmentFound(this, new DataFoundEventArgs<LabeledSegment>(segment));
                        break;
                }
            }
            else
            {
                RaiseParseError(string.Format("Unrecognized formatting in {0} section.", mCurrentSection));
            }
        }

        ////////////////////////////////////////////////////////////////////////////////

        private void ParseGeoLine()
        {
            mMatch = sRegexGeoSegment.Match(mCurrentLine);
            if (mMatch.Success)
            {
                ColoredSegment s = new ColoredSegment(
                    new SectorPoint(TranslateCoordinate(mMatch.Groups[2].Value, false), TranslateCoordinate(mMatch.Groups[1].Value, true)),
                    new SectorPoint(TranslateCoordinate(mMatch.Groups[4].Value, false), TranslateCoordinate(mMatch.Groups[3].Value, true)),
                    ParseColor(mMatch.Groups[5].Value)
                );
                if (GeoSegmentFound != null) GeoSegmentFound(this, new DataFoundEventArgs<ColoredSegment>(s));
            }
            else
            {
                RaiseParseError(string.Format("Unrecognized formatting in {0} section.", mCurrentSection));
            }
        }

        ////////////////////////////////////////////////////////////////////////////////

        private void ParseRegionLine()
        {
            // Check if the line starts with whitespace. If not, it's the start of a new
            // region. If so, it's a continuation of an existing region definition.
            if (!IsWhitespace(mCurrentLine[0]))
            {
                mMatch = sRegexRegionBegin.Match(mCurrentLine);
                if (mMatch.Success)
                {

                    // Finish up any existing region and start a new one.
                    FinalizePendingRegion();
                    mCurrentRegion = new Region(ParseColor(mMatch.Groups[1].Value));
                    mCurrentRegion.Points.Add(new SectorPoint(TranslateCoordinate(mMatch.Groups[3].Value, false), TranslateCoordinate(mMatch.Groups[2].Value, true)));
                }
                else
                {
                    RaiseParseError(string.Format("Unrecognized formatting in {0} section.", mCurrentSection));
                }
            }
            else
            {
                mMatch = sRegexRegionContinuation.Match(mCurrentLine);
                if (mMatch.Success)
                {

                    // Make sure we have an existing region object started.
                    if (mCurrentRegion == null) {
                        RaiseParseError("Found region continuation line without prior region start line.");
                        return;
                    }

                    // Add the point.
                    mCurrentRegion.Points.Add(new SectorPoint(TranslateCoordinate(mMatch.Groups[2].Value, false), TranslateCoordinate(mMatch.Groups[1].Value, true)));
                }
                else
                {
                    RaiseParseError(string.Format("Unrecognized formatting in {0} section.", mCurrentSection));
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////

        private void ParseDiagramLine()
        {
            // Check if the line starts with whitespace. If not, it's the start of a new
            // diagram. If so, it's a continuation of an existing diagram definition.
            if (!IsWhitespace(mCurrentLine[0]))
            {
                mMatch = sRegexDiagramBegin.Match(mCurrentLine);
                if (mMatch.Success)
                {

                    // Finish up any existing diagram and start a new one.
                    FinalizePendingDiagram();
                    mCurrentDiagram = new Diagram(mMatch.Groups[1].Value.Trim());
                    mCurrentDiagram.Segments.Add(new ColoredSegment(
                        new SectorPoint(TranslateCoordinate(mMatch.Groups[3].Value, false), TranslateCoordinate(mMatch.Groups[2].Value, true)),
                        new SectorPoint(TranslateCoordinate(mMatch.Groups[5].Value, false), TranslateCoordinate(mMatch.Groups[4].Value, true)),
                        ParseColor(mMatch.Groups[6].Value)
                    ));
                }
                else
                {
                    RaiseParseError(string.Format("Unrecognized formatting in {0} section.", mCurrentSection));
                }
            }
            else
            {
                mMatch = sRegexDiagramContinuation.Match(mCurrentLine);
                if (mMatch.Success)
                {
                    // Make sure we have an existing diagram object started.
                    if (mCurrentDiagram == null)
                    {
                        RaiseParseError("Found diagram continuation line without prior diagram start line.");
                        return;
                    }

                    // Add the segment.
                    mCurrentDiagram.Segments.Add(new ColoredSegment(
                        new SectorPoint(TranslateCoordinate(mMatch.Groups[2].Value, false), TranslateCoordinate(mMatch.Groups[1].Value, true)),
                        new SectorPoint(TranslateCoordinate(mMatch.Groups[4].Value, false), TranslateCoordinate(mMatch.Groups[3].Value, true)),
                        ParseColor(mMatch.Groups[5].Value)
                    ));

                }
                else
                {
                    RaiseParseError(string.Format("Unrecognized formatting in {0} section.", mCurrentSection));
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////

        private void ParseLabelLine()
        {
            mMatch = sRegexStaticText.Match(mCurrentLine);
            if (mMatch.Success)
            {
                StaticText s = new StaticText(
                    new SectorPoint(TranslateCoordinate(mMatch.Groups[3].Value, false), TranslateCoordinate(mMatch.Groups[2].Value, true)),
                    mMatch.Groups[1].Value,
                    ParseColor(mMatch.Groups[4].Value)
                );
                if (StaticTextFound != null) StaticTextFound(this, new DataFoundEventArgs<StaticText>(s));
            }
            else
            {
                RaiseParseError(string.Format("Unrecognized formatting in {0} section.", mCurrentSection));
            }
        }

        ////////////////////////////////////////////////////////////////////////////////

        private void FinalizePendingObjects()
        {
            FinalizePendingDiagram();
            FinalizePendingRegion();
        }

        ////////////////////////////////////////////////////////////////////////////////

        private void FinalizePendingDiagram()
        {
            if (mCurrentDiagram != null)
            {
                if (mCurrentSection == FileSection.SID)
                {
                    if (SidFound != null) SidFound(this, new DataFoundEventArgs<Diagram>(mCurrentDiagram));
                }
                else if (mCurrentSection == FileSection.STAR)
                {
                    if (StarFound != null) StarFound(this, new DataFoundEventArgs<Diagram>(mCurrentDiagram));
                }
                else
                {
                    throw new InvalidOperationException("Invalid file section.");
                }

                mCurrentDiagram = null;
            }
        }
    }
}

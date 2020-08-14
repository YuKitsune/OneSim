// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectorFileParserTests.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Tests.Aeronautical
{
    using System.Linq;

    using NUnit.Framework;

    using OneSim.Traffic.Application.SectorFileParsers.SectorFile;
    using OneSim.Traffic.Domain.Entities.Aeronautical;

    /// <summary>
    ///     The <see cref="SectorFileParser"/> Tests.
    /// </summary>
    [TestFixture]
    public class SectorFileParserTests
    {
        /// <summary>
        ///     The Sector file content.
        /// </summary>
        public const string TestSectorFile =
            @"; DUMMY SECTOR FILE
// for testing purposes only
// Using data from VATPAC VRC Sector File AIRAC 2006

#define testDefiniton 8882055

[INFO]
Queensland - South TMA (incl CFS) AIRAC 2006 VATPAC 
BN
YBBN
S027.21.45.910
E153.08.19.273
60
53
-11.0
1
[RUNWAY]
01L 19R  016 196 S027.22.59.110 E153.06.24.670 S027.21.23.580 E153.07.19.090 ;YBBN BRISBANE   ;DB060# 110149
01R 19L  016 196 S027.24.10.090 E153.07.05.580 S027.22.28.820 E153.08.03.500 ;YBBN BRISBANE   ;DB060# 110148
10L 28R  097 277 S027.34.13.420 E152.59.49.980 S027.34.27.480 E153.00.39.310 ;YBAF BRISBANE/ARCHERFIELD   ;DB060# 110152
04R 22L  041 221 S027.34.18.080 E153.00.15.720 S027.33.55.860 E153.00.47.140 ;YBAF BRISBANE/ARCHERFIELD   ;DB060# 110155
10R 28L  096 277 S027.34.19.910 E152.59.53.020 S027.34.30.810 E153.00.31.230 ;YBAF BRISBANE/ARCHERFIELD   ;DB060# 110153
04L 22R  041 221 S027.34.15.530 E153.00.06.700 S027.33.56.250 E153.00.33.990 ;YBAF BRISBANE/ARCHERFIELD   ;DB060# 110154

[AIRPORT]
YBAF 118.100 S027.34.13.000 E153.00.29.000 D ;BRISBANE/ARCHERFIELD QLD
YBBN 118.000 S027.23.03.000 E153.07.03.000 C ;BRISBANE QLD

[VOR]
AMB  114.700 S027.38.29.600 E152.42.57.700 ; AMBERLEY   ;DB033# 1040424
BN   113.200 S027.21.57.900 E153.08.21.200 ; BRISBANE   ;DB033# 1040437

[FIXES]
SANAD S031.23.11.000 E151.24.44.600   ;DB045# 1037110
ADMAR S030.19.22.000 E151.54.42.700   ;DB045# 1031802
TESSI S029.27.03.700 E152.18.45.200   ;DB045# 1037672
APAGI S028.50.56.000 E152.35.07.100   ;DB045# 1031909
HUUGO S028.05.23.300 E152.55.29.200   ;DB045# 1034402
SANEG S027.49.04.200 E153.02.38.300   ;DB045# 1037113
ISRIB S027.19.20.800 E153.13.23.900   ;DB045# 1034527
LUMDI S027.21.35.000 E153.18.19.000   ;DB045# 1035204
MAVPI S027.31.31.500 E153.13.35.100   ;DB045# 1039241
BOSVU S027.24.53.500 E153.06.40.900   ;DB045# 1032497
DADAN S027.29.48.500 E153.08.58.700   ;DB045# 1033354
DENIS S027.33.22.600 E153.08.05.300   ;DB045# 1033398
LILEE S027.36.55.600 E153.06.41.800   ;DB045# 1035063

[HIGH AIRWAY]
H91(66)                   SANAD          SANAD          ADMAR          ADMAR             ;DB065# 636844
H91(66)                   ADMAR          ADMAR          TESSI          TESSI             ;DB065# 636843
H91(64)                   TESSI          TESSI          APAGI          APAGI             ;DB065# 636842
H91(59)                   APAGI          APAGI          HUUGO          HUUGO             ;DB065# 636841
H91(33)                   HUUGO          HUUGO          SANEG          SANEG             ;DB065# 636840
H91(23)                   SANEG          SANEG          BN             BN                ;DB065# 636839";

        /// <summary>
        ///     Ensures that <see cref="Fix"/>es can be parsed.
        /// </summary>
        [Test]
        public void CanParseFix()
        {
            // Arrange
            SectorFileParser parser = new SectorFileParser();

            // Act
            SectorFileParseResult result = parser.Parse(TestSectorFile);

            // Assert
            Assert.AreEqual(6, result.Fixes.Count);
            Fix sanadFix = result.Fixes.FirstOrDefault(f => f.Identifier == "SANAD");
            Assert.IsNotNull(sanadFix);
            Assert.AreEqual(
                new Coordinate(
                    new CoordinateComponent(CardinalDirection.South, 031, 23, 11),
                    new CoordinateComponent(CardinalDirection.East, 151, 24, 44.6)),
                sanadFix.Location);
        }

        /// <summary>
        ///     Ensures that <see cref="Navaid"/>s can be parsed.
        /// </summary>
        [Test]
        public void CanParseNavaid()
        {
            // Arrange
            SectorFileParser parser = new SectorFileParser();

            // Act
            SectorFileParseResult result = parser.Parse(TestSectorFile);

            // Assert
            Assert.AreEqual(2, result.Navaids.Count);
            Navaid brisbaneNavaid = result.Navaids.FirstOrDefault(n => n.Identifier == "BN");
            Assert.IsNotNull(brisbaneNavaid);
            Assert.AreEqual(
                new Coordinate(
                    new CoordinateComponent(CardinalDirection.South, 027, 21, 57.900),
                    new CoordinateComponent(CardinalDirection.East, 153, 08, 21.200)),
                brisbaneNavaid.Location);
            Assert.AreEqual(113200, brisbaneNavaid.Frequency);
        }

        /// <summary>
        ///     Ensures that <see cref="Airport"/>s can be parsed.
        /// </summary>
        [Test]
        public void CanParseAirport()
        {
            // Arrange
            SectorFileParser parser = new SectorFileParser();

            // Act
            SectorFileParseResult result = parser.Parse(TestSectorFile);

            // Assert
            Assert.AreEqual(2, result.Navaids.Count);
            Airport brisbaneAirport = result.Airports.FirstOrDefault(n => n.Identifier == "YBBN");
            Assert.IsNotNull(brisbaneAirport);
            Assert.AreEqual(
                new Coordinate(
                    new CoordinateComponent(CardinalDirection.South, 027, 23, 03.000),
                    new CoordinateComponent(CardinalDirection.East, 153, 07, 03.000)),
                brisbaneAirport.Location);
            Assert.AreEqual(brisbaneAirport.Class, AirspaceClass.C);
        }

        /// <summary>
        ///     Ensures that <see cref="Runway"/>s can be parsed.
        /// </summary>
        [Test]
        public void CanParseRunway()
        {
            // Arrange
            SectorFileParser parser = new SectorFileParser();

            // Act
            SectorFileParseResult result = parser.Parse(TestSectorFile);

            // Assert
            // Check the airport
            Assert.AreEqual(2, result.Navaids.Count);
            Airport brisbaneAirport = result.Airports.FirstOrDefault(n => n.Identifier == "YBBN");
            Assert.IsNotNull(brisbaneAirport);

            // Check the runway
            Assert.AreEqual(4, brisbaneAirport.Runways.Count);
            Runway runway19L = brisbaneAirport.Runways.FirstOrDefault(r => r.Identifier == "19L");
            Assert.IsNotNull(runway19L);
            Assert.AreEqual(
                new Coordinate(
                    new CoordinateComponent(CardinalDirection.South, 027, 22, 28.820),
                    new CoordinateComponent(CardinalDirection.East, 153, 08, 03.500)),
                runway19L.ThresholdLocation);
            Assert.AreEqual(196, runway19L.Heading);
        }

        /// <summary>
        ///     Ensures that <see cref="Runway"/>s are assigned to the correct <see cref="Airport"/>.
        /// </summary>
        [Test]
        public void RunwaysAssignToCorrectAirport()
        {
            // Arrange
            SectorFileParser parser = new SectorFileParser();

            // Act
            SectorFileParseResult result = parser.Parse(TestSectorFile);

            // Assert
            // Check the airport
            Assert.AreEqual(2, result.Airports.Count);
            Assert.AreEqual(4, result.Airports.First(a => a.Identifier == "YBBN").Runways.Count);
            Assert.AreEqual(8, result.Airports.First(a => a.Identifier == "YBAF").Runways.Count);
        }

        /// <summary>
        ///     Ensures that <see cref="Airway"/>s can be parser.
        /// </summary>
        [Test]
        public void CanParseAirways()
        {
            // Arrange
            SectorFileParser parser = new SectorFileParser();

            // Act
            SectorFileParseResult result = parser.Parse(TestSectorFile);

            // Assert
            Airway airway = result.HighAirways.FirstOrDefault(a => a.Identifier == "H91");
            Assert.IsNotNull(airway);
            Assert.IsTrue(
                airway.Fixes.Select(f => f.Identifier)
                      .ToArray()
                      .SequenceEqual(new[] { "SANAD", "ADMAR", "TESSI", "APAGI", "HUUGO", "SANEG", "BN" }));
        }
    }
}

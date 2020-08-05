// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EuroScopeExtensionFileParserTests.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Tests.Ais
{
    using System.Linq;

    using NUnit.Framework;

    using OneSim.Traffic.Application.SectorFileParsers.EuroScopeExtensionFile;
    using OneSim.Traffic.Application.SectorFileParsers.SectorFile;
    using OneSim.Traffic.Domain.Entities.Ais;

    /// <summary>
    ///     The <see cref="EuroScopeExtensionFileParser"/> tests.
    /// </summary>
    [TestFixture]
    public class EuroScopeExtensionFileParserTests
    {
        /// <summary>
        ///     The EuroScope Extension file content.
        /// </summary>
        private const string TestExtensionFile = @"; DUMMY EUROSCOPE EXTENSION FILE
// for testing purposes only
// Using data from VATPAC EuroScope Sector File AIRAC 2006

[POSITIONS]
Brisbane Approach (North):Brisbane Approach:124.700:BP:A:BN:APP:BN:APP:1411:1525
Brisbane Departures (North)*:Brisbane Departures:133.450:BD:D:BN:DEP:BN:DEP:1420:1480
Brisbane Tower:Brisbane Tower:120.500:BT:T:BN:TWR:BN:TWR:1428:1523

[SIDSSTARS]
SID:YBBN:01R:SANEG1:ISRIB LUMDI MAVPI SANEG
SID:YBBN:19L:SANEG1:BOSVU DADAN DENIS LILEE SANEG

[SECTORS]
SECTORLINE:BN-N_APP01
COORD:S026.53.45.584:E152.56.55.625
COORD:S026.54.43.263:E152.54.11.936
COORD:S026.55.55.177:E152.51.35.376
COORD:S026.57.18.939:E152.49.06.381
COORD:S026.58.53.919:E152.46.46.078
COORD:S027.00.39.400:E152.44.35.528
COORD:S027.02.34.589:E152.42.35.724
COORD:S027.04.38.616:E152.40.47.578
COORD:S027.06.50.545:E152.39.11.918
COORD:S027.09.09.380:E152.37.49.480
COORD:S027.11.34.070:E152.36.40.899
COORD:S027.12.00.000:E152.36.35.000
COORD:S027.12.00.000:E152.36.35.000
COORD:S027.13.25.000:E152.41.03.000
COORD:S027.42.34.000:E152.52.59.000
COORD:S027.44.13.000:E152.51.55.000
COORD:S027.47.06.000:E152.49.49.000
COORD:S027.47.37.493:E152.50.50.567
COORD:S027.48.09.108:E152.51.51.944
COORD:S027.48.38.802:E152.52.54.540
COORD:S027.48.41.414:E152.53.03.448
COORD:S027.23.13.164:E153.07.37.396
COORD:S027.20.18.097:E153.00.15.656
COORD:S026.53.45.584:E152.56.55.625

SECTORLINE:BN-N_APP19
COORD:S027.02.39.199:E152.42.31.618
COORD:S027.00.43.544:E152.44.30.842
COORD:S026.58.57.684:E152.46.41.007
COORD:S026.57.22.298:E152.49.00.964
COORD:S026.55.58.104:E152.51.29.654
COORD:S026.54.45.735:E152.54.05.951
COORD:S026.53.45.737:E152.56.48.677
COORD:S026.52.58.560:E152.59.36.607
COORD:S026.52.24.558:E153.02.28.477
COORD:S026.52.03.987:E153.05.22.994
COORD:S026.51.57.001:E153.08.18.848
COORD:S026.52.03.653:E153.11.14.718
COORD:S026.52.23.894:E153.14.09.284
COORD:S026.52.57.570:E153.17.01.234
COORD:S026.53.44.429:E153.19.49.275
COORD:S026.54.44.119:E153.22.32.143
COORD:S026.55.16.191:E153.23.37.251
COORD:S027.23.26.766:E153.07.30.638
COORD:S027.02.39.199:E152.42.31.618

SECTORLINE:BN-N_DEP01
COORD:S026.53.45.756:E152.56.55.690
COORD:S026.52.57.048:E152.59.42.976
COORD:S026.52.23.545:E153.02.34.968
COORD:S026.52.03.480:E153.05.29.559
COORD:S026.51.57.004:E153.08.25.438
COORD:S026.52.04.167:E153.11.21.283
COORD:S026.52.24.913:E153.14.15.774
COORD:S026.52.59.088:E153.17.07.601
COORD:S026.53.46.435:E153.19.55.471
COORD:S026.54.46.597:E153.22.38.122
COORD:S026.55.16.191:E153.23.37.251
COORD:S027.23.13.164:E153.07.37.396
COORD:S027.20.18.097:E153.00.15.656
COORD:S026.53.45.756:E152.56.55.690

SECTORLINE:BN-N_DEP19
COORD:S027.02.39.199:E152.42.31.618
COORD:S027.04.43.419:E152.40.43.766
COORD:S027.06.55.626:E152.39.08.589
COORD:S027.09.14.699:E152.37.46.658
COORD:S027.11.39.588:E152.36.38.608
COORD:S027.12.00.000:E152.36.35.000
COORD:S027.12.00.000:E152.36.35.000
COORD:S027.13.25.000:E152.41.03.000
COORD:S027.42.34.000:E152.52.59.000
COORD:S027.44.13.000:E152.51.55.000
COORD:S027.47.06.000:E152.49.49.000
COORD:S027.47.37.493:E152.50.50.567
COORD:S027.48.09.108:E152.51.51.944
COORD:S027.48.38.802:E152.52.54.540
COORD:S027.48.41.414:E152.53.03.448
COORD:S027.23.26.766:E153.07.30.638
COORD:S027.02.39.199:E152.42.31.618

SECTOR:BNA01:0:18000
OWNER:BP:BD
ARRAPT:YBBN:YBAF
DEPAPT:YBBN:YBAF
BORDER:BN-N_APP01
ACTIVE:YBBN:01R
ACTIVE:YBBN:01L

SECTOR:BNA19:0:18000
OWNER:BP:BD
ARRAPT:YBBN:YBAF
DEPAPT:YBBN:YBAF
BORDER:BN-N_APP19
ACTIVE:YBBN:19L
ACTIVE:YBBN:19R

SECTOR:BND01:0:18000
OWNER:BD:BP
ARRAPT:YBBN
DEPAPT:YBBN
BORDER:BN-N_DEP01
ACTIVE:YBBN:01R
ACTIVE:YBBN:01L

SECTOR:BND19:0:18000
OWNER:BD:BP
ARRAPT:YBBN:YBAF
DEPAPT:YBBN:YBAF
BORDER:BN-N_DEP19
ACTIVE:YBBN:19L
ACTIVE:YBBN:19R";

        /// <summary>
        ///     Ensures that <see cref="TerminalRoute"/>s can be parsed.
        /// </summary>
        [Test]
        public void CanParseTerminalRoutes()
        {
            // Arrange
            SectorFileParser sectorFileParser = new SectorFileParser();
            EuroScopeExtensionFileParser parser = new EuroScopeExtensionFileParser();

            // Act
            SectorFileParseResult sectorFileParseResult = sectorFileParser.Parse(SectorFileParserTests.TestSectorFile);
            EuroScopeExtensionFileParseResult result = parser.Parse(TestExtensionFile, sectorFileParseResult);

            // Assert
            Assert.AreEqual(2, result.TerminalRoutes.Count);
            TerminalRoute saneg19L = result.TerminalRoutes.FirstOrDefault(r => r.Identifier == "SANEG1" && r.ValidRunways.Any(rw => rw.Identifier == "19L"));
            Assert.IsNotNull(saneg19L);
            foreach (Fix sidFix in saneg19L.Fixes)
            {
                Fix correspondingFix =
                    sectorFileParseResult.Fixes.FirstOrDefault(f => f.Identifier == sidFix.Identifier);
                Assert.IsNotNull(correspondingFix);
                Assert.AreEqual(correspondingFix.Location, sidFix.Location);
            }
        }

        /// <summary>
        ///     Ensures that <see cref="ControllerPosition"/>s can be parsed.
        /// </summary>
        [Test]
        public void CanParsePositions()
        {
            // Arrange
            SectorFileParser sectorFileParser = new SectorFileParser();
            EuroScopeExtensionFileParser parser = new EuroScopeExtensionFileParser();

            // Act
            SectorFileParseResult sectorFileParseResult = sectorFileParser.Parse(SectorFileParserTests.TestSectorFile);
            EuroScopeExtensionFileParseResult result = parser.Parse(TestExtensionFile, sectorFileParseResult);

            // Assert
            Assert.AreEqual(3, result.ControllerPositions.Count);
            ControllerPosition approachPosition = result.ControllerPositions.FirstOrDefault(p => p.PositionId == "BP");
            Assert.IsNotNull(approachPosition);
            Assert.AreEqual("Brisbane Approach", approachPosition.RadioCallsign);
            Assert.AreEqual("Brisbane Approach (North)", approachPosition.Name);
            Assert.AreEqual(124700, approachPosition.Frequency);
            Assert.AreEqual("BN", approachPosition.CallsignPrefix);
            Assert.AreEqual("APP", approachPosition.CallsignSuffix);
        }

        /// <summary>
        ///     Ensures that <see cref="Sector"/>s can be parsed.
        /// </summary>
        [Test]
        public void CanParseSectors()
        {
            // Arrange
            SectorFileParser sectorFileParser = new SectorFileParser();
            EuroScopeExtensionFileParser parser = new EuroScopeExtensionFileParser();

            // Act
            SectorFileParseResult sectorFileParseResult = sectorFileParser.Parse(SectorFileParserTests.TestSectorFile);
            EuroScopeExtensionFileParseResult result = parser.Parse(TestExtensionFile, sectorFileParseResult);

            // Assert
            Assert.AreEqual(4, result.Sectors.Count);
            Sector brisbaneApproachRunway01 = result.Sectors.FirstOrDefault(s => s.SectorIdentifier == "BNA01");
            Assert.IsNotNull(brisbaneApproachRunway01);
            Assert.AreEqual(24, brisbaneApproachRunway01.Border.Count);
            Assert.IsTrue(brisbaneApproachRunway01.Positions.Select(p => p.Position.PositionId).ToArray().SequenceEqual(new[] { "BP", "BD" }));
            Assert.AreEqual(2, brisbaneApproachRunway01.ActiveRunways.Count);
            Assert.IsTrue(brisbaneApproachRunway01.ActiveRunways.Any(r => r.Identifier == "01L"));
            Assert.IsTrue(brisbaneApproachRunway01.ActiveRunways.Any(r => r.Identifier == "01R"));
        }
    }
}

namespace OneSim.Map.Tests.Infrastructure
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using NUnit.Framework;

	using OneSim.Map.Application;
	using OneSim.Map.Domain.Entities;
	using OneSim.Map.Infrastructure;

	/// <summary>
	/// 	The VATSIM Status File Parser Tests.
	/// </summary>
	[TestFixture]
	public class VatsimStatusFileParserTests
	{
		/// <summary>
		/// 	Gets a line containing a <see cref="Pilot"/> with a <see cref="FlightPlan"/> in the VATSIM Status file
		/// 	format.
		/// </summary>
		public string PilotWithFlightPlan =>
			"QFA238:1283544:Cristian Torz YMML:PILOT::1.36475:103.98409:14:164:B738/L:407:WSSS:24000:WMKK:SYD-1:100:1:2000:::2:I:435:435:0:37:2:21:WSSS:+VFPS+/V/PBN/A1B2B3B4B5B6O2O3O4D2D3D4S1 DOF/191226 REG/VHVXS EET/WMFC0003 SEL/GRDK OPR/QVIRTUAL.COM.AU PER/C RMK/TCAS EQUIPPED:MASBO A457 GUPTA:0:0:0:0:::20191226040930:22:29.914:1013:";

		/// <summary>
		/// 	Gets a line containing a <see cref="Pilot"/> without a <see cref="FlightPlan"/> in the VATSIM Status
		/// 	file format.
		/// </summary>
		public string PilotWithoutFlightPlan => "SWA6264:1384148:Zac Stephenson KOKC:PILOT::42.36476:-71.01649:15:0::0::::USA-WEST:100:1:2000::::::::::::::::::::20191226042425:119:29.92:1013:";

		/// <summary>
		/// 	Gets a line containing a <see cref="AirTrafficController"/> in the VATSIM Status file format.
		/// </summary>
		public string ControllerLine => "AS_TWR:1249373:Peter Story /O:ATC:118.300:-23.80833:133.90083:0:::0::::SYD-1:100:5::4:50::::::::::::::::::20191226042858::::";

		/// <summary>
		/// 	Gets a line containing a <see cref="FlightNotification"/> in the VATSIM Status file format.
		/// </summary>
		public string PreFileNoticeLine => "JST442:1020541:Kristian Demian YMML:::::::A321/L:452:YMML:F350:YBCG:::::::0:I:605:605:1:47:3:15::+VFPS+/V/PBN/A1B1C1D1O2S2T1 NAV/RNP2 DOF/191226 REG/VHVWX EET/YBBB0054 SEL/JPHR OPR/JETSTAR PER/D:DCT NONIX H66 TW Y23 BERNI DCT:0:0:0:0:::::::";

		/// <summary>
		/// 	Gets a line containing a <see cref="Server"/> in the VATSIM Status file format.
		/// </summary>
		public string ServerLine => "SYD-1:107.191.56.188:Sydney, Australia:SYD-1:1:";

		/// <summary>
		/// 	Gets a string representing an example VATSIM Status data file.
		/// </summary>
		public string StatusFileExample =>
			$@"; Created at 26/12/2019 04:53:09 UTC by Data Server V4.0
;
; Data is the property of VATSIM.net and is not to be used for commercial purposes without the express written permission of the VATSIM.net Founders or their designated agent(s ).
;
; Sections are:
; !GENERAL contains general settings
; !CLIENTS contains informations about all connected clients
; !PREFILE contains informations about all prefiled flight plans
; !SERVERS contains a list of all FSD running servers to which clients can connect
; !VOICE SERVERS contains a list of all running voice servers that clients can use
;
; Data formats of various sections are:
; !GENERAL section -         VERSION is this data format version
;                            RELOAD  is time in minutes this file will be updated
;                            UPDATE is the last date and time this file has been updated. Format is yyyymmddhhnnss
;                            ATIS ALLOW MIN is time in minutes to wait before allowing manual Atis refresh by way of web page interface
;                            CONNECTED CLIENTS is the number of clients currently connected
; !CLIENTS section -         callsign:cid:realname:clienttype:frequency:latitude:longitude:altitude:groundspeed:planned_aircraft:planned_tascruise:planned_depairport:planned_altitude:planned_destairport:server:protrevision:rating:transponder:facilitytype:visualrange:planned_revision:planned_flighttype:planned_deptime:planned_actdeptime:planned_hrsenroute:planned_minenroute:planned_hrsfuel:planned_minfuel:planned_altairport:planned_remarks:planned_route:planned_depairport_lat:planned_depairport_lon:planned_destairport_lat:planned_destairport_lon:atis_message:time_last_atis_received:time_logon:heading:QNH_iHg:QNH_Mb:
; !PREFILE section -         callsign:cid:realname:clienttype:frequency:latitude:longitude:altitude:groundspeed:planned_aircraft:planned_tascruise:planned_depairport:planned_altitude:planned_destairport:server:protrevision:rating:transponder:facilitytype:visualrange:planned_revision:planned_flighttype:planned_deptime:planned_actdeptime:planned_hrsenroute:planned_minenroute:planned_hrsfuel:planned_minfuel:planned_altairport:planned_remarks:planned_route:planned_depairport_lat:planned_depairport_lon:planned_destairport_lat:planned_destairport_lon:atis_message:time_last_atis_received:time_logon:heading:QNH_iHg:QNH_Mb:
; !SERVERS section -         ident:hostname_or_IP:location:name:clients_connection_allowed:
; !VOICE SERVERS section -   hostname_or_IP:location:name:clients_connection_allowed:type_of_voice_server:
;
; Field separator is : character
;
;
!GENERAL:
VERSION = 8
RELOAD = 2
UPDATE = 20191226045309
ATIS ALLOW MIN = 5
CONNECTED CLIENTS = 317
;
;
!VOICE SERVERS:


;
;
!CLIENTS:
{PilotWithFlightPlan}
{ControllerLine}
{PilotWithoutFlightPlan}
;
;
!SERVERS:
{ServerLine}
;
;
!PREFILE:
{PreFileNoticeLine}
;
;   END";

		/// <summary>
		/// 	Ensures a valid <see cref="Pilot"/> line with a <see cref="FlightPlan"/> can parse.
		/// </summary>
		[Test]
		public void PilotWithFlightPlanCanParse()
		{
			// Arrange
			VatsimStatusFileParser parser = new VatsimStatusFileParser();
			Pilot expectedPilot = new Pilot
								  {
									  Callsign = "QFA238",
									  NetworkId = "1283544",
									  Name = "Cristian Torz YMML",
									  Latitude = 1.36475,
									  Longitude = 103.98409,
									  Altitude = 14,
									  GroundSpeed = 164,
									  Heading = 22,
									  FlightPlan = new FlightPlan
												   {
													   AircraftType = "B738/L",
													   TrueAirSpeed = "407",
													   Altitude = 24000,
													   DepartureIcao = "WSSS",
													   ArrivalIcao = "WMKK",
													   EstimatedTimeOfDeparture = new DateTime(DateTime.UtcNow.Year,
																							   DateTime.UtcNow.Month,
																							   DateTime.UtcNow.Day,
																							   4,
																							   35,
																							   0,
																							   DateTimeKind.Utc),
													   FlightRules = FlightPlanRules.InstrumentFlightRules,
													   TimeEnroute = new TimeSpan(0, 37, 0),
													   FuelOnBoard = new TimeSpan(2, 21, 0),
													   AlternateIcao = "WSSS",
													   Route = "MASBO A457 GUPTA",
													   Remarks =
														   "+VFPS+/V/PBN/A1B2B3B4B5B6O2O3O4D2D3D4S1 DOF/191226 REG/VHVXS EET/WMFC0003 SEL/GRDK OPR/QVIRTUAL.COM.AU PER/C RMK/TCAS EQUIPPED"
												   },
									  Server = "SYD-1",
									  ProtocolRevision = "100",
									  Squawk = "2000",
									  LogonTime = DateTime.SpecifyKind(new DateTime(2019, 12, 26, 04, 09, 30), DateTimeKind.Utc)
								  };

			// Act
			List<StatusFileParseError> errors = new List<StatusFileParseError>();
			Pilot pilot = parser.ParsePilotLine(PilotWithFlightPlan);

			// Assert
			Assert.IsEmpty(errors);
			Assert.AreEqual(expectedPilot.NetworkId, pilot.NetworkId);
			Assert.AreEqual(expectedPilot.Callsign, pilot.Callsign);
			Assert.AreEqual(expectedPilot.Name, pilot.Name);
			Assert.AreEqual(expectedPilot.Server, pilot.Server);
			Assert.AreEqual(expectedPilot.ProtocolRevision, pilot.ProtocolRevision);
			Assert.AreEqual(expectedPilot.LogonTime, pilot.LogonTime);

			double tolerance = 0.00001;
			Assert.IsTrue(expectedPilot.Latitude.WithinTolerance(pilot.Latitude, tolerance));
			Assert.IsTrue(expectedPilot.Longitude.WithinTolerance(pilot.Longitude, tolerance));
			Assert.AreEqual(expectedPilot.Altitude, pilot.Altitude);
			Assert.AreEqual(expectedPilot.GroundSpeed, pilot.GroundSpeed);
			Assert.AreEqual(expectedPilot.Heading, pilot.Heading);
			Assert.IsTrue(expectedPilot.Squawk == pilot.Squawk);
			Assert.IsNotNull(pilot.FlightPlan);
			Assert.AreEqual(expectedPilot.FlightPlan.AircraftType, pilot.FlightPlan.AircraftType);
			Assert.AreEqual(expectedPilot.FlightPlan.TrueAirSpeed, pilot.FlightPlan.TrueAirSpeed);
			Assert.AreEqual(expectedPilot.FlightPlan.Altitude, pilot.FlightPlan.Altitude);
			Assert.AreEqual(expectedPilot.FlightPlan.DepartureIcao, pilot.FlightPlan.DepartureIcao);
			Assert.AreEqual(expectedPilot.FlightPlan.ArrivalIcao, pilot.FlightPlan.ArrivalIcao);
			Assert.AreEqual(expectedPilot.FlightPlan.AlternateIcao, pilot.FlightPlan.AlternateIcao);
			Assert.AreEqual(expectedPilot.FlightPlan.EstimatedTimeOfDeparture, pilot.FlightPlan.EstimatedTimeOfDeparture);
			Assert.AreEqual(expectedPilot.FlightPlan.ActualTimeOfDeparture, pilot.FlightPlan.ActualTimeOfDeparture);
			Assert.AreEqual(expectedPilot.FlightPlan.FlightRules, pilot.FlightPlan.FlightRules);
			Assert.AreEqual(expectedPilot.FlightPlan.TimeEnroute, pilot.FlightPlan.TimeEnroute);
			Assert.AreEqual(expectedPilot.FlightPlan.FuelOnBoard, pilot.FlightPlan.FuelOnBoard);
			Assert.AreEqual(expectedPilot.FlightPlan.Route, pilot.FlightPlan.Route);
			Assert.AreEqual(expectedPilot.FlightPlan.Remarks, pilot.FlightPlan.Remarks);
			Assert.AreEqual(expectedPilot.FlightPlan.ScheduledTimeOfArrival, pilot.FlightPlan.ScheduledTimeOfArrival);
			Assert.IsTrue(expectedPilot.History.SequenceEqual(pilot.History));
		}

		/// <summary>
		/// 	Ensures a valid <see cref="Pilot"/> line without a <see cref="FlightPlan"/> can parse.
		/// </summary>
		[Test]
		public void PilotWithoutFlightPlanCanParse()
		{
			// Arrange
			VatsimStatusFileParser parser = new VatsimStatusFileParser();
			Pilot expectedPilot = new Pilot
								  {
									  Callsign = "SWA6264",
									  NetworkId = "1384148",
									  Name = "Zac Stephenson KOKC",
									  Latitude = 42.36476,
									  Longitude = -71.01649,
									  Altitude = 15,
									  GroundSpeed = 0,
									  Heading = 119,
									  Server = "USA-WEST",
									  ProtocolRevision = "100",
									  Squawk = "2000",
									  LogonTime = new DateTime(2019, 12, 26, 04, 24, 25, DateTimeKind.Utc)
								  };

			// Act
			List<StatusFileParseError> errors = new List<StatusFileParseError>();
			Pilot pilot = parser.ParsePilotLine(PilotWithoutFlightPlan);

			// Assert
			Assert.IsEmpty(errors);

			// Assert
			Assert.IsEmpty(errors);

			// Client wide assertions
			Assert.AreEqual(expectedPilot.NetworkId, pilot.NetworkId);
			Assert.AreEqual(expectedPilot.Callsign, pilot.Callsign);
			Assert.AreEqual(expectedPilot.Name, pilot.Name);
			Assert.AreEqual(expectedPilot.Server, pilot.Server);
			Assert.AreEqual(expectedPilot.ProtocolRevision, pilot.ProtocolRevision);
			Assert.AreEqual(expectedPilot.LogonTime, pilot.LogonTime);

			// Pilot specific assertions
			double tolerance = 0.00001;
			Assert.IsTrue(expectedPilot.Latitude.WithinTolerance(pilot.Latitude, tolerance));
			Assert.IsTrue(expectedPilot.Longitude.WithinTolerance(pilot.Longitude, tolerance));
			Assert.AreEqual(expectedPilot.Altitude, pilot.Altitude);
			Assert.AreEqual(expectedPilot.GroundSpeed, pilot.GroundSpeed);
			Assert.AreEqual(expectedPilot.Heading, pilot.Heading);
			Assert.IsTrue(expectedPilot.Squawk == pilot.Squawk);
			Assert.IsNull(pilot.FlightPlan);
			Assert.IsTrue(expectedPilot.History.SequenceEqual(pilot.History));
		}

		/// <summary>
		/// 	Ensures a valid <see cref="AirTrafficController"/> line can parse.
		/// </summary>
		[Test]
		public void ControllerLineCanParse()
		{
			// Arrange
			VatsimStatusFileParser parser = new VatsimStatusFileParser();
			AirTrafficController expectedController = new AirTrafficController
													  {
														  Callsign = "AS_TWR",
														  NetworkId = "1249373",
														  Name = "Peter Story /O",
														  Server = "SYD-1",
														  ProtocolRevision = "100",
														  LogonTime = new DateTime(2019, 12, 26, 04, 28, 58, DateTimeKind.Utc),
														  Frequency = "118.300",
														  Rating = ControllerRating.Controller1,
														  FacilityType = ControllerFacilityType.Tower,
														  VisibilityRange = 50,
														  Atis = string.Empty
													  };

			// Act
			AirTrafficController controller = parser.ParseControllerLine(ControllerLine);

			// Assert
			// Client wide assertions
			Assert.AreEqual(expectedController.NetworkId, controller.NetworkId);
			Assert.AreEqual(expectedController.Callsign, controller.Callsign);
			Assert.AreEqual(expectedController.Name, controller.Name);
			Assert.AreEqual(expectedController.Server, controller.Server);
			Assert.AreEqual(expectedController.ProtocolRevision, controller.ProtocolRevision);
			Assert.AreEqual(expectedController.LogonTime, controller.LogonTime);

			// Controller specific assertions
			Assert.AreEqual(expectedController.Frequency, controller.Frequency);
			Assert.AreEqual(expectedController.Rating, controller.Rating);
			Assert.AreEqual(expectedController.FacilityType, controller.FacilityType);
			Assert.AreEqual(expectedController.VisibilityRange, controller.VisibilityRange);
			Assert.AreEqual(expectedController.Atis, controller.Atis);
		}

		/// <summary>
		/// 	Ensures a valid <see cref="FlightNotification"/> line can parse.
		/// </summary>
		[Test]
		public void PreFileNoticeLineCanParse()
		{
			// Arrange
			VatsimStatusFileParser parser = new VatsimStatusFileParser();
			FlightNotification expectedPreFile = new FlightNotification
											{
												Callsign = "JST442",
												NetworkId = "1020541",
												Name = "Kristian Demian YMML",
												FlightPlan = new FlightPlan
															 {
																 AircraftType = "A321/L",
																 TrueAirSpeed = "452",
																 Altitude = 35000,
																 DepartureIcao = "YMML",
																 ArrivalIcao = "YBCG",
																 EstimatedTimeOfDeparture =
																	 new DateTime(DateTime.UtcNow.Year,
																				  DateTime.UtcNow.Month,
																				  DateTime.UtcNow.Day,
																				  6,
																				  5,
																				  0,
																				  DateTimeKind.Utc),
																 FlightRules = FlightPlanRules.InstrumentFlightRules,
																 TimeEnroute = new TimeSpan(1, 47, 0),
																 FuelOnBoard = new TimeSpan(3, 15, 0),
																 AlternateIcao = string.Empty,
																 Route = "DCT NONIX H66 TW Y23 BERNI DCT",
																 Remarks =
																	 "+VFPS+/V/PBN/A1B1C1D1O2S2T1 NAV/RNP2 DOF/191226 REG/VHVWX EET/YBBB0054 SEL/JPHR OPR/JETSTAR PER/D"
															 }
											};

			// Act
			FlightNotification flightNotification = parser.ParseFlightNotificationLine(PreFileNoticeLine);

			// Assert
			Assert.AreEqual(expectedPreFile.NetworkId, flightNotification.NetworkId);
			Assert.AreEqual(expectedPreFile.Callsign, flightNotification.Callsign);
			Assert.AreEqual(expectedPreFile.Name, flightNotification.Name);

			Assert.IsNotNull(flightNotification.FlightPlan);
			Assert.AreEqual(expectedPreFile.FlightPlan.AircraftType, flightNotification.FlightPlan.AircraftType);
			Assert.AreEqual(expectedPreFile.FlightPlan.TrueAirSpeed, flightNotification.FlightPlan.TrueAirSpeed);
			Assert.AreEqual(expectedPreFile.FlightPlan.Altitude, flightNotification.FlightPlan.Altitude);
			Assert.AreEqual(expectedPreFile.FlightPlan.DepartureIcao, flightNotification.FlightPlan.DepartureIcao);
			Assert.AreEqual(expectedPreFile.FlightPlan.ArrivalIcao, flightNotification.FlightPlan.ArrivalIcao);
			Assert.AreEqual(expectedPreFile.FlightPlan.AlternateIcao, flightNotification.FlightPlan.AlternateIcao);
			Assert.AreEqual(expectedPreFile.FlightPlan.EstimatedTimeOfDeparture, flightNotification.FlightPlan.EstimatedTimeOfDeparture);
			Assert.AreEqual(expectedPreFile.FlightPlan.ActualTimeOfDeparture, flightNotification.FlightPlan.ActualTimeOfDeparture);
			Assert.AreEqual(expectedPreFile.FlightPlan.FlightRules, flightNotification.FlightPlan.FlightRules);
			Assert.AreEqual(expectedPreFile.FlightPlan.TimeEnroute, flightNotification.FlightPlan.TimeEnroute);
			Assert.AreEqual(expectedPreFile.FlightPlan.FuelOnBoard, flightNotification.FlightPlan.FuelOnBoard);
			Assert.AreEqual(expectedPreFile.FlightPlan.Route, flightNotification.FlightPlan.Route);
			Assert.AreEqual(expectedPreFile.FlightPlan.Remarks, flightNotification.FlightPlan.Remarks);
			Assert.AreEqual(expectedPreFile.FlightPlan.ScheduledTimeOfArrival, flightNotification.FlightPlan.ScheduledTimeOfArrival);
		}

		/// <summary>
		/// 	Ensures a valid <see cref="Server"/> line can parse.
		/// </summary>
		[Test]
		public void ServerLineCanParse()
		{
			// Arrange
			VatsimStatusFileParser parser = new VatsimStatusFileParser();
			Server expectedServer = new Server
									{
										NetworkIdentifier = "SYD-1",
										IpAddress = "107.191.56.188",
										Location = "Sydney, Australia",
										Name = "SYD-1"
									};

			// Act
			Server server = parser.ParseServerLine(ServerLine);

			// Assert
			Assert.AreEqual(expectedServer.NetworkIdentifier, server.NetworkIdentifier);
			Assert.AreEqual(expectedServer.IpAddress, server.IpAddress);
			Assert.AreEqual(expectedServer.Location, server.Location);
			Assert.AreEqual(expectedServer.Name, server.Name);
		}

		/// <summary>
		/// 	Ensures the <see cref="VatsimStatusFileParser"/> can parse all required data from a file.
		/// </summary>
		[Test]
		public void ParserCanParseWholeFile()
		{
			// Arrange
			VatsimStatusFileParser parser = new VatsimStatusFileParser();

			// Act
			StatusFileParseResult result = parser.Parse(StatusFileExample);

			// Assert
			Assert.AreEqual(0, result.Errors.Count);
			Assert.AreEqual(1, result.Controllers.Count);
			Assert.AreEqual(2, result.Pilots.Count);
			Assert.AreEqual(1, result.FlightNotifications.Count);
			Assert.AreEqual(1, result.Servers.Count);
		}

		/// <summary>
		/// 	Ensures the <see cref="VatsimStatusFileParser.ParseFlightPlanAltitude"/> method works given an Altitude.
		/// </summary>
		[Test]
		public void AltitudeParserWorksWithAltitudes()
		{
			// Arrange
			VatsimStatusFileParser parser = new VatsimStatusFileParser();
			string altitudeString1 = "A050";
			string altitudeString2 = "050";
			string altitudeString3 = "A95";
			string altitudeString4 = "95";

			// Act
			int altitude1 = parser.ParseFlightPlanAltitude(altitudeString1);
			int altitude2 = parser.ParseFlightPlanAltitude(altitudeString2);
			int altitude3 = parser.ParseFlightPlanAltitude(altitudeString3);
			int altitude4 = parser.ParseFlightPlanAltitude(altitudeString4);

			// Assert
			Assert.AreEqual(5000, altitude1);
			Assert.AreEqual(5000, altitude2);
			Assert.AreEqual(9500, altitude3);
			Assert.AreEqual(9500, altitude4);
		}

		/// <summary>
		/// 	Ensures the <see cref="VatsimStatusFileParser.ParseFlightPlanAltitude"/> method works given a Flight Level.
		/// </summary>
		[Test]
		public void AltitudeParserWorksWithFlightLevels()
		{
			// Arrange
			VatsimStatusFileParser parser = new VatsimStatusFileParser();
			string flightLevelString1 = "FL380";
			string flightLevelString2 = "FL 380";
			string flightLevelString3 = "F280";
			string flightLevelString4 = "F 280";
			string flightLevelString5 = "295";
			string flightLevelString6 = "2500";
			string flightLevelString7 = "32000";

			// Act
			int flightLevel1 = parser.ParseFlightPlanAltitude(flightLevelString1);
			int flightLevel2 = parser.ParseFlightPlanAltitude(flightLevelString2);
			int flightLevel3 = parser.ParseFlightPlanAltitude(flightLevelString3);
			int flightLevel4 = parser.ParseFlightPlanAltitude(flightLevelString4);
			int flightLevel5 = parser.ParseFlightPlanAltitude(flightLevelString5);
			int flightLevel6 = parser.ParseFlightPlanAltitude(flightLevelString6);
			int flightLevel7 = parser.ParseFlightPlanAltitude(flightLevelString7);

			// Assert
			Assert.AreEqual(38000, flightLevel1);
			Assert.AreEqual(38000, flightLevel2);
			Assert.AreEqual(28000, flightLevel3);
			Assert.AreEqual(28000, flightLevel4);
			Assert.AreEqual(29500, flightLevel5);
			Assert.AreEqual(2500, flightLevel6);
			Assert.AreEqual(32000, flightLevel7);
		}
	}
}
namespace OneSim.Map.Infrastructure
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using OneSim.Map.Application;
	using OneSim.Map.Application.Abstractions;
	using OneSim.Map.Application.Exceptions;
	using OneSim.Map.Domain.Entities;
	using OneSim.Map.Infrastructure.Exceptions;

	/// <summary>
	/// 	The VATSIM Status File Parser.
	/// </summary>
	public class VatsimStatusFileParser : IStatusFileParser
	{
		/// <summary>
		/// 	Parses the given <see cref="string"/> as a Status File.
		/// </summary>
		/// <param name="rawStatusFile">
		///		The raw status file.
		/// </param>
		/// <returns>
		///		The <see cref="StatusFileParseResult"/>.
		/// </returns>
		public StatusFileParseResult Parse(string rawStatusFile)
		{
			StatusFileParseResult result = new StatusFileParseResult();

			// Get the pilots
			(IEnumerable<Pilot> pilots, IEnumerable<StatusFileParseError> errors) pilots = GetPilots(rawStatusFile);
			result.Pilots.AddRange(pilots.pilots);
			result.Errors.AddRange(pilots.errors);

			// Get the controllers
			(IEnumerable<AirTrafficController> controllers, IEnumerable<StatusFileParseError> errors) controllers = GetControllers(rawStatusFile);
			result.Controllers.AddRange(controllers.controllers);
			result.Errors.AddRange(controllers.errors);

			// Get the Pre-File Notices
			(IEnumerable<FlightNotification> preFileNotices, IEnumerable<StatusFileParseError> errors) preFileNotices = GetPreFileNotices(rawStatusFile);
			result.PreFileNotices.AddRange(preFileNotices.preFileNotices);
			result.Errors.AddRange(preFileNotices.errors);

			// Get the servers
			(IEnumerable<Server> servers, IEnumerable<StatusFileParseError> errors) servers = GetServers(rawStatusFile);
			result.Servers.AddRange(servers.servers);
			result.Errors.AddRange(servers.errors);

			return result;
		}

		/// <summary>
		/// 	Gets the <see cref="Pilot"/>s from the given <paramref name="rawStatusFile"/>.
		/// 	Also returns the <see cref="StatusFileParseError"/>s.
		/// </summary>
		/// <param name="rawStatusFile">
		///		The raw status file content as a <see cref="string"/>.
		/// </param>
		/// <returns>
		///		The <see cref="Pilot"/>s and <see cref="StatusFileParseError"/>s.
		/// </returns>
		public (IEnumerable<Pilot> pilots, IEnumerable<StatusFileParseError> errors) GetPilots(string rawStatusFile)
		{
			// Get the clients section
			string[] clientLines = IsolateSection(VatsimStatusFileSection.Clients, rawStatusFile);

			// Loop through each of the lines
			List<Pilot> pilots = new List<Pilot>();
			List<StatusFileParseError> errors = new List<StatusFileParseError>();
			foreach (string clientLine in clientLines)
			{
				try
				{
					// Parse the pilot
					Pilot pilot = ParsePilotLine(clientLine);
					pilots.Add(pilot);
				}
				catch (InvalidClientTypeException)
				{
					// Ignore Invalid Client Type Exceptions, they're since ATC and pilots are mixed in the same section.
					continue;
				}
				catch (Exception ex)
				{
					errors.Add(new StatusFileParseError($"Failed to parse Pilot line \"{clientLine}\". See Exception for details.", ex));
				}
			}

			return (pilots, errors);
		}

		/// <summary>
		/// 	Parses the given line as a <see cref="Pilot"/>.
		/// </summary>
		/// <param name="pilotLine">
		///     The line from the status file representing a <see cref="Pilot"/>.
		/// </param>
		/// <returns>
		/// 	The <see cref="Pilot"/>.
		/// </returns>
		public Pilot ParsePilotLine(string pilotLine)
		{
			// Make sure we have our 42 fields
			string[] pilotLineSections = pilotLine.Split(':');
			if (pilotLineSections.Length != 42)
			{
				throw new InvalidLineException(pilotLine, $"Pilot line found containing {pilotLineSections.Length} elements. Only parsing Pilot lines with 42 elements is supported.");
			}

			// Only looking for pilots
			string clientType = pilotLineSections[3];
			if (clientType != "PILOT")
			{
				throw new InvalidClientTypeException(pilotLine, $"Expected a Pilot line, found a {clientType}.");
			}

			// Create the pilot
			Pilot pilot = new Pilot
						  {
							  Callsign = pilotLineSections[0],
							  NetworkId = pilotLineSections[1],
							  Name = pilotLineSections[2],
							  Server = pilotLineSections[14],
							  ProtocolRevision = pilotLineSections[15],
							  LogonTime = ParseStatusDateTime(pilotLineSections[37]),
							  Latitude = double.Parse(pilotLineSections[5]),
							  Longitude = double.Parse(pilotLineSections[6]),
							  Altitude = int.Parse(pilotLineSections[7]),
							  GroundSpeed = int.Parse(pilotLineSections[8]),
							  Heading = int.Parse(pilotLineSections[38]),
							  Squawk = pilotLineSections[17]
						  };

			// Attempt to get a flight plan filed or not.
			try
			{
				// Only create a flight plan if there is an arrival and departure ICAO code
				// Todo: Refine conditions for creating a flight plan to avoid using a Try/Catch.
				string departureIcaoCode = pilotLineSections[11];
				string arrivalIcaoCode = pilotLineSections[13];
				if (!string.IsNullOrEmpty(departureIcaoCode) ||
					!string.IsNullOrEmpty(arrivalIcaoCode))
				{
					// Todo: Refine handling for parse errors.
					pilot.FlightPlan = new FlightPlan
									   {
										   AircraftType = pilotLineSections[9],
										   TrueAirSpeed = pilotLineSections[10],
										   Altitude = ParseFlightPlanAltitude(pilotLineSections[12]),
										   DepartureIcao = departureIcaoCode,
										   ArrivalIcao = arrivalIcaoCode,
										   EstimatedTimeOfDeparture = ParseFlightPlanDateTime(pilotLineSections[22]),
										   FlightRules = pilotLineSections[21] == "I" ? FlightPlanRules.InstrumentFlightRules : FlightPlanRules.VisualFlightRules,
										   TimeEnroute = new TimeSpan(hours: int.Parse(pilotLineSections[24]),
																	  minutes: int.Parse(pilotLineSections[25]),
																	  seconds: 0),
										   FuelOnBoard = new TimeSpan(hours: int.Parse(pilotLineSections[26]),
																	  minutes: int.Parse(pilotLineSections[27]),
																	  seconds: 0),
										   AlternateIcao = pilotLineSections[28],
										   Route = pilotLineSections[30],
										   Remarks = pilotLineSections[29],
									   };
				}
			}
			catch
			{
				pilot.FlightPlan = null;
			}

			return pilot;
		}

		/// <summary>
		/// 	Gets the <see cref="AirTrafficController"/>s from the given <paramref name="rawStatusFile"/>.
		/// 	Also returns the <see cref="StatusFileParseError"/>s.
		/// </summary>
		/// <param name="rawStatusFile">
		///		The raw status file content as a <see cref="string"/>.
		/// </param>
		/// <returns>
		///		The <see cref="AirTrafficController"/>s and <see cref="StatusFileParseError"/>s.
		/// </returns>
		public (IEnumerable<AirTrafficController> controllers, IEnumerable<StatusFileParseError> errors) GetControllers(string rawStatusFile)
		{
			// Get the clients section
			string[] clientLines = IsolateSection(VatsimStatusFileSection.Clients, rawStatusFile);

			// Loop through each of the lines
			List<AirTrafficController> controllers = new List<AirTrafficController>();
			List<StatusFileParseError> errors = new List<StatusFileParseError>();
			foreach (string clientLine in clientLines)
			{
				try
				{
					// Parse the Controller
					AirTrafficController controller = ParseControllerLine(clientLine);
					controllers.Add(controller);
				}
				catch (InvalidClientTypeException)
				{
					// Ignore Invalid Client Type Exceptions, they're since ATC and pilots are mixed in the same section.
					continue;
				}
				catch (Exception ex)
				{
					errors.Add(new StatusFileParseError($"Failed to parse line \"{clientLine}\". See Exception for details.", ex));
				}
			}

			return (controllers, errors);
		}

		/// <summary>
		/// 	Parses the given line as an <see cref="AirTrafficController"/>.
		/// </summary>
		/// <param name="controllerLine">
		///		The line from the status file representing an <see cref="AirTrafficController"/>.
		/// </param>
		/// <returns>
		///		The <see cref="AirTrafficController"/>.
		/// </returns>
		public AirTrafficController ParseControllerLine(string controllerLine)
		{
			// Make sure we have our 42 fields
			string[] controllerLineSections = controllerLine.Split(':');
			if (controllerLineSections.Length != 42)
			{
				throw new InvalidLineException(controllerLine, $"Controller line found containing {controllerLineSections.Length} elements. Only parsing Controller lines with 42 elements is supported.");
			}

			// Only looking for controllers
			string clientType = controllerLineSections[3];
			if (clientType != "ATC")
			{
				throw new InvalidClientTypeException(controllerLine, $"Expected an ATC line, found a {clientType}.");
			}

			// Create the controller
			AirTrafficController controller = new AirTrafficController
											  {
												  Callsign = controllerLineSections[0],
												  NetworkId = controllerLineSections[1],
												  Name = controllerLineSections[2],
												  Server = controllerLineSections[14],
												  ProtocolRevision = controllerLineSections[15],
												  LogonTime = ParseStatusDateTime(controllerLineSections[37]),
												  Frequency = controllerLineSections[4],
												  Rating = (ControllerRating) int.Parse(controllerLineSections[16]),
												  FacilityType = (ControllerFacilityType) int.Parse(controllerLineSections[18]),
												  VisibilityRange = int.Parse(controllerLineSections[19]),
												  Atis = controllerLineSections[35],
											  };

			return controller;
		}

		/// <summary>
		/// 	Gets the <see cref="FlightNotification"/>s from the given <paramref name="rawStatusFile"/>.
		/// 	Also returns the <see cref="StatusFileParseError"/>s.
		/// </summary>
		/// <param name="rawStatusFile">
		///		The raw status file content as a <see cref="string"/>.
		/// </param>
		/// <returns>
		///		The <see cref="FlightNotification"/>s and <see cref="StatusFileParseError"/>s.
		/// </returns>
		public (IEnumerable<FlightNotification> preFileNotices, IEnumerable<StatusFileParseError> errors) GetPreFileNotices(string rawStatusFile)
		{
			// Get the clients section
			string[] preFileNoticeLines = IsolateSection(VatsimStatusFileSection.Prefile, rawStatusFile);

			// Loop through each of the lines
			List<FlightNotification> preFileNotices = new List<FlightNotification>();
			List<StatusFileParseError> errors = new List<StatusFileParseError>();
			foreach (string preFileNoticeLine in preFileNoticeLines)
			{
				try
				{
					// Create the Pre-File Notice
					FlightNotification flightNotification = ParsePreFileNoticeLine(preFileNoticeLine);
					preFileNotices.Add(flightNotification);
				}
				catch (Exception ex)
				{
					errors.Add(new StatusFileParseError($"Failed to parse line \"{preFileNoticeLine}\". See Exception for details.", ex));
				}
			}

			return (preFileNotices, errors);
		}

		/// <summary>
		/// 	Parses the given line as a <see cref="FlightNotification"/>.
		/// </summary>
		/// <param name="preFileNoticeLine">
		///		The line from the status file representing a <see cref="FlightNotification"/>.
		/// </param>
		/// <returns>
		///		The <see cref="FlightNotification"/>.
		/// </returns>
		public FlightNotification ParsePreFileNoticeLine(string preFileNoticeLine)
		{
			// Make sure we have our 42 fields
			string[] preFileNoticeLineSections = preFileNoticeLine.Split(':');
			if (preFileNoticeLineSections.Length != 42)
			{
				throw new InvalidLineException(preFileNoticeLine, $"Pre-File Notice line found containing {preFileNoticeLineSections.Length} elements. Only parsing Pre-File Notice lines with 42 elements is supported.");
			}

			// Create the Pre-File Notice
			FlightNotification flightNotification = new FlightNotification
										  {
											  Callsign = preFileNoticeLineSections[0],
											  NetworkId = preFileNoticeLineSections[1],
											  Name = preFileNoticeLineSections[2],
											  FlightPlan = new FlightPlan
														   {
															   AircraftType = preFileNoticeLineSections[9],
															   TrueAirSpeed = preFileNoticeLineSections[10],
															   Altitude = ParseFlightPlanAltitude(preFileNoticeLineSections[12]),
															   DepartureIcao = preFileNoticeLineSections[11],
															   ArrivalIcao = preFileNoticeLineSections[13],
															   EstimatedTimeOfDeparture = ParseFlightPlanDateTime(preFileNoticeLineSections[22]),
															   FlightRules = preFileNoticeLineSections[21] == "I" ? FlightPlanRules.InstrumentFlightRules : FlightPlanRules.VisualFlightRules,
															   TimeEnroute = new TimeSpan(hours: int.Parse(preFileNoticeLineSections[24]),
																						  minutes: int.Parse(preFileNoticeLineSections[25]),
																						  seconds: 0),
															   FuelOnBoard = new TimeSpan(hours: int.Parse(preFileNoticeLineSections[26]),
																						  minutes: int.Parse(preFileNoticeLineSections[27]),
																						  seconds: 0),
															   AlternateIcao = preFileNoticeLineSections[28],
															   Route = preFileNoticeLineSections[30],
															   Remarks = preFileNoticeLineSections[29]
														   }
										  };

			return flightNotification;
		}

		/// <summary>
		/// 	Gets the <see cref="Server"/>s from the given <paramref name="rawStatusFile"/>.
		/// 	Also returns the <see cref="StatusFileParseError"/>s.
		/// </summary>
		/// <param name="rawStatusFile">
		///		The raw status file content as a <see cref="string"/>.
		/// </param>
		/// <returns>
		///		The <see cref="Server"/>s and <see cref="StatusFileParseError"/>s.
		/// </returns>
		public (IEnumerable<Server> servers, IEnumerable<StatusFileParseError> errors) GetServers(string rawStatusFile)
		{
			// Get the clients section
			string[] serverLines = IsolateSection(VatsimStatusFileSection.Servers, rawStatusFile);

			// Loop through each of the lines
			List<Server> servers = new List<Server>();
			List<StatusFileParseError> errors = new List<StatusFileParseError>();
			foreach (string serverLine in serverLines)
			{
				try
				{
					// Create the server
					Server server = ParseServerLine(serverLine);
					servers.Add(server);
				}
				catch (Exception ex)
				{
					errors.Add(new StatusFileParseError($"Failed to parse line \"{serverLine}\". See Exception for details.", ex));
				}
			}

			return (servers, errors);
		}

		/// <summary>
		/// 	Parses the given line as a <see cref="Server"/>.
		/// </summary>
		/// <param name="serverLine">
		///		The line from the status file representing a <see cref="Server"/>.
		/// </param>
		/// <returns>
		///		The <see cref="Server"/>.
		/// </returns>
		public Server ParseServerLine(string serverLine)
		{
			// Make sure we have our 6 fields
			string[] serverFields = serverLine.Split(':');
			if (serverFields.Length != 6)
			{
				throw new InvalidLineException(serverLine, $"Server line found containing {serverFields.Length} elements. Only parsing server lines with 6 elements is supported.");
			}

			// Create the server
			Server server = new Server
							{
								NetworkIdentifier = serverFields[0],
								IpAddress = serverFields[1],
								Location = serverFields[2],
								Name = serverFields[3]
							};

			return server;
		}

		/// <summary>
		/// 	Parses an altitude defined in a <see cref="FlightPlan"/>.
		/// 	This is required because the Altitude field in the flight plan is a string where the user can enter
		/// 	any string. This method tries to account for the most common things users will type into the altitude
		/// 	field of a flight plan.
		/// </summary>
		/// <param name="altitudeString">
		///		The altitude from the <see cref="FlightPlan"/>.
		/// </param>
		/// <returns>
		///		The altitude in feet represented by an <see cref="int"/>.
		/// </returns>
		public int ParseFlightPlanAltitude(string altitudeString)
		{
			// If given a bullshit value, then give back a bullshit value
			if (string.IsNullOrEmpty(altitudeString)) return 0;

			try
			{
				// Clean the input a bit
				altitudeString = altitudeString.ToUpper().Trim();

				// Todo: Have some setting that contains a list of strings that prefix a flight level
				string[] flightLevelIndicators = { "FL", "F" };

				// Check if the altitude matches any of the flight level prefixes
				foreach (string flightLevelIndicator in flightLevelIndicators)
				{
					// Ignore if the prefix doesn't match
					if (!altitudeString.StartsWith(flightLevelIndicator, StringComparison.Ordinal)) continue;

					// Remove the prefix and re-trim
					string flightLevelString = altitudeString.Replace(flightLevelIndicator, string.Empty).Trim();

					// Check if the remaining characters can be converted to an int
					if (int.TryParse(flightLevelString, out int flightLevel))
					{
						// Convert Flight Level to feet
						return flightLevel * 100;
					}

					throw new AltitudeParseException(altitudeString, $"Altitude looks like a Flight Level, but couldn't parse to a Flight Level.");
				}

				// Check if the altitude begins with A (E.g: "A050")
				// Note: This prepares the string for the next check
				if (altitudeString.StartsWith("A", StringComparison.Ordinal))
				{
					// Remove prefix and re-trim
					altitudeString = altitudeString.Replace("A", string.Empty).Trim();
				}

				// Check if we can convert it directly to an int
				if (int.TryParse(altitudeString, out int altitude))
				{
					// If the altitude is greater than 1,000 (E.g: 2500), then parse them as they are
					if (altitude >= 1000) return altitude;

					// Otherwise, treat is as a Flight Level (290 = 29,000 ft) or Altitude (050 = 5,000)
					return altitude * 100;
				}

				// Made it this far, none of the match cases passed. Somethings fucked.
				throw new AltitudeParseException(altitudeString);
			}
			catch (AltitudeParseException)
			{
				// If an AltitudeParseException was thrown, re-throw it.
				throw;
			}
			catch (Exception ex)
			{
				// If some other exception was thrown, then wrap it in an AltitudeParseException
				throw new AltitudeParseException(altitudeString, innerException: ex);
			}
		}

		/// <summary>
		/// 	Converts the given <see cref="string"/> into a <see cref="DateTime"/> using the Status file's format.
		/// </summary>
		/// <param name="dateTime">
		///		The string to parse.
		/// </param>
		/// <returns>
		///		The resulting <see cref="DateTime"/>.
		/// </returns>
		public DateTime ParseStatusDateTime(string dateTime) =>
			new DateTime(year: int.Parse(dateTime.Substring(0, 4)),
						 month: int.Parse(dateTime.Substring(4, 2)),
						 day: int.Parse(dateTime.Substring(6, 2)),
						 hour: int.Parse(dateTime.Substring(8, 2)),
						 minute:
						 int.Parse(dateTime.Substring(10, 2)),
						 second:
						 int.Parse(dateTime.Substring(12, 2)),
						 DateTimeKind.Utc);

		/// <summary>
		/// 	Converts the given <see cref="string"/> into a <see cref="DateTime"/> using the <see cref="FlightPlan"/>
		/// 	format.
		/// </summary>
		/// <param name="dateTimeString">
		///		The string to convert.
		/// </param>
		/// <returns>
		///		The resulting <see cref="DateTime"/>.
		/// </returns>
		public DateTime? ParseFlightPlanDateTime(string dateTimeString)
		{
			// for different lengths of time
			switch (dateTimeString.Length)
			{
				// 1-2 characters (Mins only)
				case 1:
				case 2:
					return new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, int.Parse(dateTimeString), 0, DateTimeKind.Utc);

				// 3 characters (Single digit hour)
				case 3: return new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, int.Parse(dateTimeString.Substring(0, 1)), int.Parse(dateTimeString.Substring(1, 2)), 0, DateTimeKind.Utc);

				// 4 characters (hhmm)
				case 4: return new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, int.Parse(dateTimeString.Substring(0, 2)), int.Parse(dateTimeString.Substring(2, 2)), 0, DateTimeKind.Utc);

				// Worse case scenario, no time
				default: return null;
			}
		}

		/// <summary>
		/// 	Isolates the given section of the Status file.
		/// </summary>
		/// <param name="section">
		///		The <see cref="VatsimStatusFileSection"/> to be isolated.
		/// </param>
		/// <param name="statusFile">
		///		The Status File content.
		/// </param>
		/// <returns>
		///		The isolated section split into an <see cref="Array"/>, where each entry is it's own line.
		/// </returns>
		private static string[] IsolateSection(VatsimStatusFileSection section, string statusFile)
		{
			// Because this method is kinda confusing, here is basically what it does:
			// 1. First, you give the method a the Status file, and the section you want to rip out
			// 2. The method looks at the section header for the section you provided. (Client section = "!CLIENTS")
			// 3. Then it scans the Status file line by line, from top to bottom, ignoring comments and blank lines.
			// 4. When the scan has reached a line that has the section header we got earlier, it starts storing those
			//    lines in a list.
			// 5. When the scan has reached a line that has the section header of a different section, then it knows
			//    it's finished scanning the desired section.
			// 6. The list of lines gets returned.
			// I probably didn't need this, but i can see how methods like these can get confusing.

			// Create a dictionary of each of the section headers along with their corresponding enum value
			// Will use this to determine what section header we're looking for in the file
			Dictionary<VatsimStatusFileSection, string> sectionHeaders =
				new Dictionary<VatsimStatusFileSection, string>
				{
					{ VatsimStatusFileSection.General, "!GENERAL" },
					{ VatsimStatusFileSection.VoiceServers, "!VOICE SERVERS" },
					{ VatsimStatusFileSection.Clients, "!CLIENTS" },
					{ VatsimStatusFileSection.Servers, "!SERVERS" },
					{ VatsimStatusFileSection.Prefile, "!PREFILE" },
				};

			// Create a list of just the header strings. Will make it easier to find out when we've finished scanning
			// the desired section and we've hit another section.
			string[] headerStrings = sectionHeaders.Select(s => s.Value).ToArray();

			// Get the header string of the section we're looking for
			string targetHeader = sectionHeaders.First(s => s.Key == section).Value;

			// Split the file by new lines into an array
			string[] lines = statusFile.Split(Environment.NewLine);

			// Use this flag to know when to start reading lines
			bool extractLines = false;

			// Store our desired lines in here
			List<string> extractedLines = new List<string>();
			foreach (string line in lines)
			{
				// Ignore empty lines, or ones starting with the comment character
				if (string.IsNullOrEmpty(line) ||
					line.StartsWith(";", StringComparison.Ordinal))
					continue;

				// If the current line starts with the header we're looking for, set the flag to start extracting lines
				if (line.StartsWith(targetHeader, StringComparison.Ordinal))
				{
					extractLines = true;

					continue;
				}

				// If we're not ready to start reading the lines, then there's no point in continuing
				if (!extractLines) continue;

				// If the current line starts with some other section header, then stop extracting lines, as we've hit
				// a different section
				if (headerStrings.Any(header => line.StartsWith(header, StringComparison.Ordinal)))
				{
					break;
				}

				extractedLines.Add(line);
			}

			return extractedLines.ToArray();
		}
	}
}
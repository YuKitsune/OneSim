﻿namespace OneSim.Map.Infrastructure
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using OneSim.Map.Application;
	using OneSim.Map.Application.Abstractions;
	using OneSim.Map.Application.Exceptions;
	using OneSim.Map.Domain.Attributes;
	using OneSim.Map.Domain.Entities;
	using OneSim.Map.Infrastructure.Exceptions;

	/// <summary>
	/// 	The <see cref="ITrafficDataParser"/> for parsing data containing online traffic data from the VATSIM network.
	/// </summary>
	[Network(NetworkType.Vatsim)]
	public class VatsimTrafficDataParser : ITrafficDataParser
	{
		///  <summary>
		///  	Parses the given <see cref="string"/> as online traffic data.
		///  </summary>
		///  <param name="trafficData">
		/// 		The online traffic data in the form of a <see cref="string"/>..
		///  </param>
		///  <exception cref="NotSupportedException"></exception>
		///  <returns>
		/// 		The <see cref="TrafficDataParseResult"/>.
		///  </returns>
		public TrafficDataParseResult Parse(string trafficData)
		{
			// Prepare our results
			TrafficDataParseResult result = new TrafficDataParseResult();

			// Create a dictionary of each of the section headers along with their corresponding enum value
			// Will use this to determine what section header we're looking for in the file
			Dictionary<VatsimTrafficDataFileSection, string> sectionHeaders =
				new Dictionary<VatsimTrafficDataFileSection, string>
				{
					{ VatsimTrafficDataFileSection.General, "!GENERAL" },
#pragma warning disable 618
					{ VatsimTrafficDataFileSection.VoiceServers, "!VOICE SERVERS" },
#pragma warning restore 618
					{ VatsimTrafficDataFileSection.Clients, "!CLIENTS" },
					{ VatsimTrafficDataFileSection.Servers, "!SERVERS" },
					{ VatsimTrafficDataFileSection.Prefile, "!PREFILE" },
				};

			// For keeping track of what section we're currently in
			VatsimTrafficDataFileSection? currentSection = null;

			// Split the status file by each new line
			string[] lines = trafficData.Split(new[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

			// Loop through ever line (Decided to use a simple for loop for performance)
			for (int i = 0; i < lines.Length; i++)
			{
				// Track the current line
				string currentLine = lines[i];

				// Ignore comments and blank lines
				if (string.IsNullOrEmpty(currentLine) ||
					currentLine.StartsWith(";", StringComparison.Ordinal) ||
					currentLine.StartsWith("#", StringComparison.Ordinal))
					continue;

				// Check if we've hit a section header
				if (sectionHeaders.Any(s => currentLine.StartsWith(s.Value, StringComparison.Ordinal)))
				{
					// Remember the section so we know how to parse the next line(s)
					currentSection = sectionHeaders.FirstOrDefault(s => currentLine.StartsWith(s.Value, StringComparison.Ordinal)).Key;

					// Can't read the current line if it's a header
					continue;
				}

				// Haven't picked up on a section yet, no need to continue
				if (!currentSection.HasValue) continue;

				try
				{
					switch (currentSection)
					{
						// Parse the current line as a Client
						case VatsimTrafficDataFileSection.Clients:
							BaseClient client = ParseClientLine(currentLine);
							switch (client)
							{
								case Pilot pilot:
									result.Pilots.Add(pilot);

									break;

								case AirTrafficController controller:
									result.Controllers.Add(controller);

									break;

								default:
									throw
										new NotSupportedException("Unexpected type found when parsing a client line.");
							}

							break;

						// Parse the current line as a Server
						case VatsimTrafficDataFileSection.Servers:
							Server server = ParseServerLine(currentLine);
							result.Servers.Add(server);

							break;

						// Parse the current line as a Flight Notification
						case VatsimTrafficDataFileSection.Prefile:
							FlightNotification flightNotification = ParseFlightNotificationLine(currentLine);
							result.FlightNotifications.Add(flightNotification);

							break;

						// Nothing else supported, so just skip
						default: continue;
					}
				}
				catch (Exception ex)
				{
					result.Errors.Add(new TrafficDataParseError(ex.Message, currentLine, ex));
				}
			}

			return result;
		}

		/// <summary>
		/// 	Parses the given line as a <see cref="BaseClient"/>.
		/// </summary>
		/// <param name="clientLine">
		///     The line from the status file representing a <see cref="BaseClient"/>.
		/// </param>
		/// <returns>
		/// 	The <see cref="BaseClient"/>.
		/// </returns>
		public BaseClient ParseClientLine(string clientLine)
		{
			string[] pilotLineSections = clientLine.Split(':');

			// Check what kind of client we're parsing
			string clientType = pilotLineSections[3];

			return clientType switch
			{
				"PILOT" => ParsePilotLine(clientLine),
				"ATC" => ParseControllerLine(clientLine),
				_ => throw new NotSupportedException($"Unexpected Client Type {clientType}.")
			};
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
			string[] pilotLineSections = pilotLine.Split(':');

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

			// Only create a flight plan if there is an arrival and departure ICAO code, route, and altitude string
			string departureIcaoCode = pilotLineSections[11];
			string arrivalIcaoCode = pilotLineSections[13];
			string route = pilotLineSections[30];
			string altitudeString = pilotLineSections[12];

			if (!string.IsNullOrEmpty(departureIcaoCode) &&
				!string.IsNullOrEmpty(arrivalIcaoCode) &&
				!string.IsNullOrEmpty(route) &&
				!string.IsNullOrEmpty(altitudeString))
			{
				pilot.FlightPlan = new FlightPlan
								   {
									   AircraftType = pilotLineSections[9],
									   TrueAirSpeed = pilotLineSections[10],
									   Altitude = ParseFlightPlanAltitude(altitudeString),
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
									   Route = route,
									   Remarks = pilotLineSections[29],
								   };
			}
			else
			{
				pilot.FlightPlan = null;
			}

			return pilot;
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
			string[] controllerLineSections = controllerLine.Split(':');

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
		/// 	Parses the given line as a <see cref="FlightNotification"/>.
		/// </summary>
		/// <param name="preFileNoticeLine">
		///		The line from the status file representing a <see cref="FlightNotification"/>.
		/// </param>
		/// <returns>
		///		The <see cref="FlightNotification"/>.
		/// </returns>
		public FlightNotification ParseFlightNotificationLine(string preFileNoticeLine)
		{
			// Make sure we have our 42 fields
			string[] preFileNoticeLineSections = preFileNoticeLine.Split(':');

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
																		 FlightRules =
																			 preFileNoticeLineSections[21] == "I" ?
																				 FlightPlanRules.InstrumentFlightRules :
																				 FlightPlanRules.VisualFlightRules,
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
			string[] serverFields = serverLine.Split(':');

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
	}
}
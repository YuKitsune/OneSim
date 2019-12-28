namespace OneSim.Map.Infrastructure.Ivao
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using OneSim.Map.Application;
	using OneSim.Map.Application.Abstractions;
	using OneSim.Map.Domain.Attributes;
	using OneSim.Map.Domain.Entities;
	using OneSim.Map.Infrastructure.Exceptions;

	/// <summary>
	/// 	The IvAo Status Data Parser.
	/// </summary>
	[Network(NetworkType.Ivao)]
	public class IvaoStatusDataParser : IStatusDataParser
	{
		/// <summary>
		/// 	Parses the given <see cref="string"/> as a set of Status data.
		/// </summary>
		/// <param name="rawStatusFile">
		///		The raw status data.
		/// </param>
		/// <returns>
		///		The <see cref="StatusParseResult"/>.
		/// </returns>
		public StatusParseResult Parse(string rawStatusFile)
		{
			// Prepare our results
			StatusParseResult result = new StatusParseResult();

			// Create a dictionary of each of the section headers along with their corresponding enum value
			// Will use this to determine what section header we're looking for in the file
			Dictionary<IvaoStatusFileSection, string> sectionHeaders =
				new Dictionary<IvaoStatusFileSection, string>
				{
					{ IvaoStatusFileSection.General, "!GENERAL" },
					{ IvaoStatusFileSection.Clients, "!CLIENTS" },
					{ IvaoStatusFileSection.Servers, "!SERVERS" },
					{ IvaoStatusFileSection.Airports, "!AIRPORTS" },
				};

			// For keeping track of what section we're currently in
			IvaoStatusFileSection? currentSection = null;

			// Split the status file by each new line
			string[] lines = rawStatusFile.Split(new[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

			// Loop through ever line (Decided to use a simple for loop for performance)
			string currentLine = string.Empty;
			for (int i = 0; i < lines.Length; i++)
			{
				currentLine = lines[i];

				// Ignore comments and blank lines
				if (string.IsNullOrEmpty(currentLine) ||
					currentLine.StartsWith(";", StringComparison.Ordinal))
					continue;

				// Check if we've hit a section header
				if (sectionHeaders.Any(s => currentLine.StartsWith(s.Value, StringComparison.Ordinal)))
				{
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
						case IvaoStatusFileSection.Clients:
							// Parse the current line as a Client
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

						case IvaoStatusFileSection.Servers:
							// Parse the current line as a Server
							Server server = ParseServerLine(currentLine);
							result.Servers.Add(server);

							break;

						// Nothing else supported, so just skip
						default: continue;
					}
				}
				catch (Exception ex)
				{
					result.Errors.Add(new StatusDataParseError(currentLine, ex.Message, ex));
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

			// Check what kind type of client we're parsing
			string clientType = pilotLineSections[3];
			switch (clientType)
			{
				case "PILOT": return ParsePilotLine(clientLine);

				case "ATC": return ParseControllerLine(clientLine);

				default: throw new NotSupportedException("Unsupported Client Type {clientType}.");
			}
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
							  LogonTime = VatsimStatusDataParser.ParseStatusDateTime(pilotLineSections[35]),
							  Latitude = double.Parse(pilotLineSections[5]),
							  Longitude = double.Parse(pilotLineSections[6]),
							  Altitude = int.Parse(pilotLineSections[7]),
							  GroundSpeed = int.Parse(pilotLineSections[8]),
							  Heading = int.Parse(pilotLineSections[43]),
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
									   Altitude = VatsimStatusDataParser.ParseFlightPlanAltitude(altitudeString),
									   DepartureIcao = departureIcaoCode,
									   ArrivalIcao = arrivalIcaoCode,
									   EstimatedTimeOfDeparture = VatsimStatusDataParser.ParseFlightPlanDateTime(pilotLineSections[22]),
									   FlightRules = pilotLineSections[21] == "I" ? FlightPlanRules.InstrumentFlightRules : FlightPlanRules.VisualFlightRules,
									   TimeEnroute = new TimeSpan(hours: int.Parse(pilotLineSections[24]),
																  minutes: int.Parse(pilotLineSections[25]),
																  seconds: 0),
									   Endurance = new TimeSpan(hours: int.Parse(pilotLineSections[26]),
																minutes: int.Parse(pilotLineSections[27]),
																seconds: 0),
									   AlternateIcao = pilotLineSections[28],
									   AlternateIcao2 = pilotLineSections[40],
									   Route = route,
									   Remarks = pilotLineSections[29],
									   PersonsOnBoard = (int.TryParse(pilotLineSections[42], out int pob) ? pob : default)
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
												  LogonTime = VatsimStatusDataParser.ParseStatusDateTime(controllerLineSections[37]),
												  Frequency = controllerLineSections[4],
												  Rating = (ControllerRating) int.Parse(controllerLineSections[16]),
												  FacilityType = (ControllerFacilityType) int.Parse(controllerLineSections[18]),
												  VisibilityRange = int.Parse(controllerLineSections[19]),
												  Atis = controllerLineSections[33]
											  };

			return controller;
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
		
		
	}
}
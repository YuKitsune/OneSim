namespace OneSim.Traffic.Infrastructure
{
	using System;

	using OneSim.Traffic.Application.Abstractions;
	using OneSim.Traffic.Domain.Attributes;
	using OneSim.Traffic.Domain.Entities;
	using OneSim.Traffic.Infrastructure.Exceptions;

	/// <summary>
	/// 	The <see cref="ITrafficDataParser"/> implementation for parsing traffic data from the IvAo Whazzup data file.
	/// 	IvAo use a slightly modified version of the Whazzup data file, and thus need to be read separately.
	/// </summary>
	[Network(NetworkType.Ivao)]
	public class IvaoWhazzupDataFileParser : BaseWhazzupDataFileParser
	{
		public override Pilot ParsePilotLine(string pilotLine)
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
							  AdministrativeRating = (AdministrativeRating) int.Parse(pilotLineSections[40]),
							  LogonTime = ParseStatusDateTime(pilotLineSections[37]),
							  Latitude = double.Parse(pilotLineSections[5]),
							  Longitude = double.Parse(pilotLineSections[6]),
							  Altitude = int.Parse(pilotLineSections[7]),
							  GroundSpeed = int.Parse(pilotLineSections[8]),
							  Heading = int.Parse(pilotLineSections[46]),
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
									   ScheduledDepartureTime = ParseFlightPlanDateTime(pilotLineSections[22]),
									   FlightRules = GetFlightPlanRules(pilotLineSections[21]),
									   EstimatedEnrouteTime = new TimeSpan(int.Parse(pilotLineSections[24]),
																		   int.Parse(pilotLineSections[25]),
																		   0),
									   Endurance = new TimeSpan(int.Parse(pilotLineSections[26]),
																int.Parse(pilotLineSections[27]),
																0),
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
	}
}
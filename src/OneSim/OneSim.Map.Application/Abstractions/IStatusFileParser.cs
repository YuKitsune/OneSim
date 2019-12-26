namespace OneSim.Map.Application.Abstractions
{
	using System;
	using System.Collections.Generic;

	using OneSim.Map.Domain.Entities;

	/// <summary>
	/// 	The status file parser interface.
	/// </summary>
	public interface IStatusFileParser
	{
		/// <summary>
		/// 	Parses the given <see cref="string"/> as a status File.
		/// </summary>
		/// <param name="rawStatusFile">
		///		The raw status file.
		/// </param>
		/// <returns>
		///		The <see cref="StatusFileParseResult"/>.
		/// </returns>
		StatusFileParseResult Parse(string rawStatusFile);

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
		(IEnumerable<Pilot> pilots, IEnumerable<StatusFileParseError> errors) GetPilots(string rawStatusFile);

		/// <summary>
		/// 	Parses the given line as a <see cref="Pilot"/>.
		/// </summary>
		/// <param name="pilotLine">
		///     The line from the status file representing a <see cref="Pilot"/>.
		/// </param>
		/// <returns>
		/// 	The <see cref="Pilot"/>.
		/// </returns>
		Pilot ParsePilotLine(string pilotLine);

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
		(IEnumerable<AirTrafficController> controllers, IEnumerable<StatusFileParseError> errors) GetControllers(string rawStatusFile);

		/// <summary>
		/// 	Parses the given line as an <see cref="AirTrafficController"/>.
		/// </summary>
		/// <param name="controllerLine">
		///		The line from the status file representing an <see cref="AirTrafficController"/>.
		/// </param>
		/// <returns>
		///		The <see cref="AirTrafficController"/>.
		/// </returns>
		AirTrafficController ParseControllerLine(string controllerLine);

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
		(IEnumerable<FlightNotification> preFileNotices, IEnumerable<StatusFileParseError> errors) GetPreFileNotices(string rawStatusFile);

		/// <summary>
		/// 	Parses the given line as a <see cref="FlightNotification"/>.
		/// </summary>
		/// <param name="preFileNoticeLine">
		///		The line from the status file representing a <see cref="FlightNotification"/>.
		/// </param>
		/// <returns>
		///		The <see cref="FlightNotification"/>.
		/// </returns>
		FlightNotification ParsePreFileNoticeLine(string preFileNoticeLine);

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
		(IEnumerable<Server> servers, IEnumerable<StatusFileParseError> errors) GetServers(string rawStatusFile);

		/// <summary>
		/// 	Parses the given line as a <see cref="Server"/>.
		/// </summary>
		/// <param name="serverLine">
		///		The line from the status file representing a <see cref="Server"/>.
		/// </param>
		/// <returns>
		///		The <see cref="Server"/>.
		/// </returns>
		Server ParseServerLine(string serverLine);

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
		int ParseFlightPlanAltitude(string altitudeString);

		/// <summary>
		/// 	Converts the given <see cref="string"/> into a <see cref="DateTime"/> using the Status file's format.
		/// </summary>
		/// <param name="dateTime">
		///		The string to parse.
		/// </param>
		/// <returns>
		///		The resulting <see cref="DateTime"/>.
		/// </returns>
		DateTime ParseStatusDateTime(string dateTime);

		/// <summary>
		/// 	Converts the given <see cref="string"/> into a <see cref="DateTime"/> using the <see cref="FlightPlan"/>
		/// 	format.
		/// </summary>
		/// <param name="dateTime">
		///		The string to convert.
		/// </param>
		/// <returns>
		///		The resulting <see cref="DateTime"/>.
		/// </returns>
		DateTime? ParseFlightPlanDateTime(string dateTime);
	}
}
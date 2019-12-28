namespace OneSim.Map.Domain.Entities
{
	/// <summary>
	/// 	The Flight Plan Rules.
	/// </summary>
	public enum FlightPlanRules
	{
		/// <summary>
		///     Instrument Flight Rules.
		/// </summary>
		InstrumentFlightRules,

		/// <summary>
		///     Visual Flight Rules.
		/// </summary>
		VisualFlightRules,

		/// <summary>
		/// 	<see cref="InstrumentFlightRules"/> first, then <see cref="VisualFlightRules"/>.
		/// 	This will indicate to ATS that during the flight a pilot will call for IFR flight cancellation.
		/// </summary>
		InstrumentThenVisual,

		/// <summary>
		/// 	<see cref="VisualFlightRules"/> first, then <see cref="InstrumentFlightRules"/>.
		/// 	This will indicate to ATS that during the flight a pilot will call for changing to IFR which will
		/// 	require ATC clearance from ATS.
		/// </summary>
		VisualThenInstrument
	}
}
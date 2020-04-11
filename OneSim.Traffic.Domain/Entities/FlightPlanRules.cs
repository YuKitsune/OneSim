namespace OneSim.Traffic.Domain.Entities
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
		/// </summary>
		InstrumentThenVisual,

		/// <summary>
		/// 	<see cref="VisualFlightRules"/> first, then <see cref="InstrumentFlightRules"/>.
		/// </summary>
		VisualThenInstrument
	}
}
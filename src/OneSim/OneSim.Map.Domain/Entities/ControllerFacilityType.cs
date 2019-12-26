namespace OneSim.Map.Domain.Entities
{
	/// <summary>
	///     The Air Traffic Controller's Facility type.
	/// </summary>
	public enum ControllerFacilityType
	{
		/// <summary>
		///		Observer / Supervisor / Administrator
		/// </summary>
		Observer = 0,

		/// <summary>
		///		Flight Service Station.
		/// </summary>
		FlightServiceStation = 1,

		/// <summary>
		///		Clearance Delivery.
		/// </summary>
		Delivery = 2,

		/// <summary>
		///		Ground.
		/// </summary>
		Ground = 3,

		/// <summary>
		///		Tower.
		/// </summary>
		Tower = 4,

		/// <summary>
		///		Approach / Departure.
		/// </summary>
		Approach = 5,

		/// <summary>
		///		Centre.
		/// </summary>
		Centre = 6
	}
}
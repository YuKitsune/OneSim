namespace OneSim.Models.Aircraft
{
	using OneSim.Models.Attributes;

	/// <summary>
	/// 	The Aircraft Performance Codes.
	/// </summary>
	public enum Performance
	{
		/// <summary>
		/// 	Automatically defined performance.
		/// 	Todo: What does this actually do?
		/// </summary>
		Auto,

		/// <summary>
		/// 	Vref speed is less than 91 knots.
		/// </summary>
		[Abbreviation("A", "Vref < 91 kts")]
		A,

		/// <summary>
		/// 	Vref speed is between 91 and 120 knots.
		/// </summary>
		[Abbreviation("B", "Vref 91 - 120 kts")]
		B,

		/// <summary>
		/// 	Vref speed is between 121 and 140 knots.
		/// </summary>
		[Abbreviation("B", "Vref 121 - 140 kts")]
		C,

		/// <summary>
		/// 	Vref speed is between 141 and 165 knots.
		/// </summary>
		[Abbreviation("B", "Vref 141 - 165 kts")]
		D,

		/// <summary>
		/// 	Vref speed is between 166 and 220 knots.
		/// </summary>
		[Abbreviation("B", "Vref 166 - 220 kts")]
		E
	}
}
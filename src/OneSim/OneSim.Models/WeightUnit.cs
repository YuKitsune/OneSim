namespace OneSim.Models
{
	using OneSim.Models.Attributes;

	/// <summary>
	/// 	The Weight Units.
	/// </summary>
	public enum WeightUnit
	{
		/// <summary>
		/// 	Kilograms.
		/// </summary>
		[Abbreviation("KGS", "Kilograms")]
		Kilograms,

		/// <summary>
		/// 	Pounds.
		/// </summary>
		[Abbreviation("LBS", "Pounds")]
		Pounds
	}
}
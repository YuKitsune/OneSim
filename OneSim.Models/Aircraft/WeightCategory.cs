namespace OneSim.Models.Aircraft
{
	using OneSim.Models.Attributes;

	/// <summary>
	/// 	The Weight Categories.
	/// </summary>
	public enum WeightCategory
	{
		/// <summary>
		/// 	Light weight aircraft.
		/// </summary>
		[Abbreviation("L", "Light")]
		Light,

		/// <summary>
		/// 	Medium weight aircraft.
		/// </summary>
		[Abbreviation("M", "Medium")]
		Medium,

		/// <summary>
		/// 	Heavy weight aircraft.
		/// </summary>
		[Abbreviation("H", "Heavy")]
		Heavy,

		/// <summary>
		/// 	Super heavy weight aircraft.
		/// </summary>
		[Abbreviation("J", "Super Heavy")]
		SuperHeavy
	}
}
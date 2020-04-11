namespace OneSim.Models.Aircraft
{
	using OneSim.Models.Attributes;

	/// <summary>
	/// 	The Aircraft Type.
	/// </summary>
	public class AircraftType
	{
		/// <summary>
		/// 	Gets or sets the ID of the current <see cref="AircraftType"/>.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// 	Gets or sets the ICAO code.
		/// 	E.g: B748
		/// </summary>
		public string IcaoCode { get; set; }

		/// <summary>
		/// 	Gets or sets the name of the current <see cref="AircraftType"/>.
		/// 	E.g: B747-8
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 	Gets or sets the default engine type to use when creating a new <see cref="Aircraft"/>.
		/// </summary>
		public string DefaultEngineType { get; set; }

		/// <summary>
		/// 	Gets or sets the default <see cref="WeightCategory"/> to use when creating a new <see cref="Aircraft"/>.
		/// </summary>
		public WeightCategory DefaultWeightCategory { get; set; }

		/// <summary>
		/// 	Gets or sets the default <see cref="Performance"/> code to use when creating a new <see cref="Aircraft"/>.
		/// </summary>
		public Performance DefaultPerformanceCode { get; set; }

		/// <summary>
		/// 	Gets or sets the default Radio Navigational equipment descriptor to use when creating a new <see cref="Aircraft"/>.
		/// 	Todo: Create a ValueObject to validate this.
		/// </summary>
		public string DefaultRadioNavigationalEquipment { get; set; }

		/// <summary>
		/// 	Gets or sets the default Performance Based Navigation descriptor to use when creating a new <see cref="Aircraft"/>.
		/// 	Todo: Create a ValueObject to validate this.
		/// </summary>
		[Abbreviation("PBN", "Performance Based Navigation")]
		public string DefaultPerformanceBasedNavigationCapability { get; set; }

		/// <summary>
		/// 	Gets or sets the default maximum number of passengers to use when creating a new <see cref="Aircraft"/>.
		/// </summary>
		public int DefaultMaxPassengers { get; set; }

		/// <summary>
		/// 	Gets or sets the default empty <see cref="Weight"/> to use when creating a new <see cref="Aircraft"/>.
		/// </summary>
		[Abbreviation("OEW", "Empty Weight")]
		public Weight DefaultEmptyWeight { get; set; }

		/// <summary>
		/// 	Gets or sets the default maximum zero fuel weight <see cref="Weight"/> to use when creating a new <see cref="Aircraft"/>.
		/// </summary>
		[Abbreviation("MZFW", "Maximum Zero Fuel Weight")]
		public Weight DefaultMaxZeroFuelWeight { get; set; }

		/// <summary>
		/// 	Gets or sets the default maximum takeoff weight <see cref="Weight"/> to use when creating a new <see cref="Aircraft"/>.
		/// </summary>
		[Abbreviation("MTOW", "Maximum Takeoff Weight")]
		public Weight DefaultMaxTakeoffWeight { get; set; }

		/// <summary>
		/// 	Gets or sets the default maximum landing weight <see cref="Weight"/> to use when creating a new <see cref="Aircraft"/>.
		/// </summary>
		[Abbreviation("MLW", "Maximum Landing Weight")]
		public Weight DefaultMaxLandingWeight { get; set; }

		/// <summary>
		/// 	Gets or sets the default maximum fuel capacity <see cref="Weight"/> to use when creating a new <see cref="Aircraft"/>.
		/// </summary>
		public Weight DefaultMaxFuelCapacity { get; set; }

		/// <summary>
		/// 	Gets or sets the default cost index to use when creating a new <see cref="Aircraft"/>.
		/// </summary>
		public int DefaultCostIndex { get; set; }
	}
}
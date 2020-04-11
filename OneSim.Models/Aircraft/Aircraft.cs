namespace OneSim.Models.Aircraft
{
	using OneSim.Models.Attributes;

	/// <summary>
	/// 	The Aircraft.
	/// </summary>
	public class Aircraft
	{
		/// <summary>
		/// 	Gets or sets the ID.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// 	Gets or sets the civil registration of the current <see cref="Aircraft"/>.
		/// </summary>
		public string CivilRegistration { get; set; }

		/// <summary>
		/// 	Gets or sets the fin number of the current <see cref="Aircraft"/>.
		/// </summary>
		public string FinNumber { get; set; }

		/// <summary>
		/// 	Gets or sets the SELCAL code.
		/// </summary>
		/// Todo: Create a ValueObject to validate this.
		[Abbreviation("SELCAL", "Selective Calling")]
		public string SelcalCode { get; set; }

		/// <summary>
		/// 	Gets or sets the Mode-S or ADS-B code.
		/// </summary>
		/// Todo: Create a ValueObject to validate this.
		public string ModeSCode { get; set; }

		/// <summary>
		/// 	Gets or sets the ICAO code.
		/// 	E.g: B748
		/// </summary>
		/// Todo: See if this can be FKd
		public string IcaoCode { get; set; }

		/// <summary>
		/// 	Gets or sets the name of the Aircraft Type.
		/// 	E.g: B747-8
		/// </summary>
		/// Todo: See if this can be FKd
		public string Name { get; set; }

		/// <summary>
		/// 	Gets or sets the engine type.
		/// </summary>
		public string EngineType { get; set; }

		/// <summary>
		/// 	Gets or sets the <see cref="WeightCategory"/>.
		/// </summary>
		public WeightCategory WeightCategory { get; set; }

		/// <summary>
		/// 	Gets or sets the <see cref="Performance"/> code.
		/// </summary>
		public Performance PerformanceCode { get; set; }

		/// <summary>
		/// 	Gets or sets the Radio Navigational equipment descriptor.
		/// 	Todo: Create a ValueObject to validate this.
		/// </summary>
		public string RadioNavigationalEquipment { get; set; }

		/// <summary>
		/// 	Gets or sets the Performance Based Navigation descriptor.
		/// 	Todo: Create a ValueObject to validate this.
		/// </summary>
		[Abbreviation("PBN", "Performance Based Navigation")]
		public string PerformanceBasedNavigationCapability { get; set; }

		/// <summary>
		/// 	Gets or sets the maximum number of passengers.
		/// </summary>
		public int MaxPassengers { get; set; }

		/// <summary>
		/// 	Gets or sets the empty <see cref="Weight"/>.
		/// </summary>
		[Abbreviation("OEW", "Empty Weight")]
		public Weight EmptyWeight { get; set; }

		/// <summary>
		/// 	Gets or sets the maximum zero fuel weight <see cref="Weight"/>.
		/// </summary>
		[Abbreviation("MZFW", "Maximum Zero Fuel Weight")]
		public Weight MaxZeroFuelWeight { get; set; }

		/// <summary>
		/// 	Gets or sets the maximum takeoff weight <see cref="Weight"/>.
		/// </summary>
		[Abbreviation("MTOW", "Maximum Takeoff Weight")]
		public Weight MaxTakeoffWeight { get; set; }

		/// <summary>
		/// 	Gets or sets the maximum landing weight <see cref="Weight"/>.
		/// </summary>
		[Abbreviation("MLW", "Maximum Landing Weight")]
		public Weight MaxLandingWeight { get; set; }

		/// <summary>
		/// 	Gets or sets the maximum fuel capacity <see cref="Weight"/>.
		/// </summary>
		public Weight MaxFuelCapacity { get; set; }

		/// <summary>
		/// 	Gets or sets the cost index.
		/// </summary>
		public int CostIndex { get; set; }

		/// <summary>
		/// 	Initializes a new instance of the <see cref="Aircraft"/> class.
		/// </summary>
		public Aircraft(AircraftType baseType = null)
		{
			// Ignore if no base type was given
			if (baseType == null) return;

			// Otherwise, pre-fill the default values
			EngineType = baseType.DefaultEngineType;
			WeightCategory = baseType.DefaultWeightCategory;
			PerformanceCode = baseType.DefaultPerformanceCode;
			RadioNavigationalEquipment = baseType.DefaultRadioNavigationalEquipment;
			PerformanceBasedNavigationCapability = baseType.DefaultPerformanceBasedNavigationCapability;
			MaxPassengers = baseType.DefaultMaxPassengers;
			EmptyWeight = baseType.DefaultEmptyWeight;
			MaxZeroFuelWeight = baseType.DefaultMaxZeroFuelWeight;
			MaxTakeoffWeight = baseType.DefaultMaxTakeoffWeight;
			MaxLandingWeight = baseType.DefaultMaxLandingWeight;
			MaxFuelCapacity = baseType.DefaultMaxFuelCapacity;
			CostIndex = baseType.DefaultCostIndex;
		}
	}
}
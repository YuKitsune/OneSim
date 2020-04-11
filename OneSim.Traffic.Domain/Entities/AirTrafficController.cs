namespace OneSim.Traffic.Domain.Entities
{
	/// <summary>
	/// 	The Air Traffic Controller <see cref="BaseClient"/>.
	/// </summary>
	public class AirTrafficController : BaseClient
	{
		// Todo: See if we can make this into a value object. Not sure if VATSIM plans to change this.

		/// <summary>
		///     Gets or sets the primary VHF radio frequency.
		/// </summary>
		public string Frequency { get; set; }

		// Todo: Different networks might have different terms for ratings and facility types. Double check.

		/// <summary>
		///     Gets or sets the <see cref="ControllerRating"/>.
		/// </summary>
		public ControllerRating Rating { get; set; }

		/// <summary>
		///     Gets or sets the <see cref="ControllerFacilityType"/>.
		/// </summary>
		public ControllerFacilityType FacilityType { get; set; }

		/// <summary>
		///     Gets or sets the visibility range in nautical miles (nm).
		/// </summary>
		public int VisibilityRange { get; set; }

		/// <summary>
		///     Gets or sets the ATIS represented by a <see cref="string"/>.
		/// </summary>
		public string Atis { get; set; }
	}
}
namespace OneSim.Map.Domain.Entities
{
	using System.Collections.Generic;

	using OneSim.Map.Domain.ValueObjects;

	/// <summary>
	/// 	The Pilot.
	/// </summary>
	public class Pilot : BaseClient
	{
        /// <summary>
        ///     Gets or sets the Latitude.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        ///     Gets or sets the Longitude.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        ///     Gets or sets the altitude in feet (ft).
        /// </summary>
        public int Altitude { get; set; }

        /// <summary>
        ///     Gets or sets the ground speed in knots (kts).
        /// </summary>
        public int GroundSpeed { get; set; }

        /// <summary>
        ///     Gets or sets the heading in degrees magnetic.
        /// </summary>
        public int Heading { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="SquawkCode"/>.
        /// </summary>
        public SquawkCode Squawk { get; set; }

        /// <summary>
        ///     Gets the aircraft type ICAO code.
        /// </summary>
        /// <remarks>
        ///     Will be an empty string if no <see cref="FlightPlan"/> has been filed.
        /// </remarks>
        public string AircraftType => FlightPlanFiled ? FlightPlan.AircraftType : string.Empty;

        /// <summary>
        ///     Gets or sets the <see cref="FlightPlan"/>.
        /// </summary>
        public FlightPlan FlightPlan { get; set; }

		/// <summary>
		/// 	Gets or sets the <see cref="Point3d"/>s representing the current <see cref="Pilot"/>'s previous positions.
		/// </summary>
		public virtual ICollection<Point3d> History { get; set; }

        /// <summary>
        ///     Gets a value indicating whether or not a <see cref="FlightPlan"/> has been filed.
        /// </summary>
        public bool FlightPlanFiled => FlightPlan != null;

		/// <summary>
		/// 	Initializes a new instance of the <see cref="Pilot"/> class.
		/// </summary>
		public Pilot() => History = new List<Point3d>();
	}
}

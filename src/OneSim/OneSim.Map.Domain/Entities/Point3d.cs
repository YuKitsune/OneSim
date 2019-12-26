namespace OneSim.Map.Domain.Entities
{
	using System;

	/// <summary>
	/// 	A 3D point in space.
	/// </summary>
	public class Point3d
	{
		/// <summary>
		/// 	Gets or sets the latitude.
		/// </summary>
		public double Latitude { get; set; }

		/// <summary>
		/// 	Gets or sets the longitude.
		/// </summary>
		public double Longitude { get; set; }

		/// <summary>
		/// 	Gets or sets the altitude in feet (ft).
		/// </summary>
		public int Altitude { get; set; }

		/// <summary>
		/// 	Gets or sets the <see cref="DateTime"/> at which the owner of the current <see cref="Point3d"/> was
		/// 	located at <see cref="Point3d"/>.
		/// </summary>
		public DateTime DateTime { get; set; }
	}
}
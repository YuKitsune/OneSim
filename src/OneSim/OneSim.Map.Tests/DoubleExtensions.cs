namespace OneSim.Map.Tests
{
	using System;

	// Todo: Move this somewhere more accessible

	/// <summary>
	/// 	The <see cref="double"/> extensions.
	/// </summary>
	public static class DoubleExtensions
	{
		/// <summary>
		/// 	Determines whether or not two <see cref="double"/>s are equal within tolerance.
		/// </summary>
		/// <param name="a">
		///		The first <see cref="double"/>.
		/// </param>
		/// <param name="b">
		///		The second <see cref="double"/>.
		/// </param>
		/// <param name="tolerance">
		///		The amount of tolerance.
		/// </param>
		/// <returns>
		///		Whether or not two <see cref="double"/>s are equal within tolerance.
		/// </returns>
		public static bool WithinTolerance(this double a, double b, double tolerance) => Math.Abs(a - b) <= tolerance;
	}
}
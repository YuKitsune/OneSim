namespace OneSim.Map.Infrastructure.Exceptions
{
	using System;

	/// <summary>
	/// 	The Altitude Parse <see cref="Exception"/>.
	/// </summary>
	public class AltitudeParseException : Exception
	{
		/// <summary>
		/// 	Gets the altitude value that could not be parsed.
		/// </summary>
		public string Altitude { get; }

		/// <summary>
		/// 	Initializes a new instance of the <see cref="AltitudeParseException"/> class.
		/// </summary>
		/// <param name="altitude">
		///		The altitude that could not be parsed.
		/// </param>
		/// <param name="message">
		///		The message.
		/// </param>
		/// <param name="innerException">
		///		The inner <see cref="Exception"/> if any.
		/// </param>
		public AltitudeParseException(string altitude, string message = "", Exception innerException = null) :
			base((string.IsNullOrEmpty(message) ? $"Unable to parse altitude {(altitude == null ? "null" : $"\"{altitude}\"")}." : message), innerException) => Altitude = altitude;
	}
}
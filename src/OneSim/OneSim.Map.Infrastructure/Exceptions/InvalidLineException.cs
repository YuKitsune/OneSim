namespace OneSim.Map.Infrastructure.Exceptions
{
	using System;

	/// <summary>
	/// 	The Invalid Line <see cref="Exception"/>.
	/// </summary>
	public class InvalidLineException : Exception
	{
		/// <summary>
		/// 	Gets the line content.
		/// </summary>
		public string Line { get; }

		/// <summary>
		/// 	Initializes a new instance of the <see cref="InvalidLineException"/> class.
		/// </summary>
		/// <param name="line">
		/// 	The invalid line.
		/// </param>
		/// <param name="message">
		///		The message.
		/// </param>
		/// <param name="innerException">
		///		The inner <see cref="Exception"/> if any.
		/// </param>
		public InvalidLineException(string line, string message, Exception innerException = null) :
			base(message, innerException) => Line = line;
	}
}
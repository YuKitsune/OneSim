namespace OneSim.Map.Application
{
	using System;

	/// <summary>
	/// 	The Status File Parse error.
	/// </summary>
	public class StatusFileParseError
	{
		/// <summary>
		/// 	Gets the error message.
		/// </summary>
		public string Message { get; }

		/// <summary>
		/// 	Gets the <see cref="Exception"/> that occurred when parsing if any.
		/// </summary>
		public Exception Exception { get; }

		/// <summary>
		/// 	Initializes a new instance of the <see cref="StatusFileParseError"/>.
		/// </summary>
		/// <param name="message">
		///		The error message.
		/// </param>
		/// <param name="exception">
		///		The <see cref="Exception"/> that occurred when parsing if any.
		/// </param>
		public StatusFileParseError(string message, Exception exception = null)
		{
			if (string.IsNullOrEmpty(message)) throw new ArgumentNullException(nameof(message), "The Message cannot be null or empty.");

			Message = message;
			Exception = exception;
		}
	}
}
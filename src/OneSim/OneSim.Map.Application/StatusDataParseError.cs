namespace OneSim.Map.Application
{
	using System;

	/// <summary>
	/// 	The Status Data Parse error.
	/// </summary>
	public class StatusDataParseError
	{
		/// <summary>
		/// 	Gets the error message.
		/// </summary>
		public string Message { get; }

		// Todo: What if the status data isn't line based?

		/// <summary>
		/// 	Gets the content of the line causing the error.
		/// </summary>
		public string LineContent { get; }

		/// <summary>
		/// 	Gets the <see cref="Exception"/> that occurred when parsing if any.
		/// </summary>
		public Exception Exception { get; }

		/// <summary>
		/// 	Initializes a new instance of the <see cref="StatusDataParseError"/>.
		/// </summary>
		/// <param name="lineContent">
		///		The content of the line causing the error.
		/// </param>
		/// <param name="message">
		///		The error message.
		/// </param>
		/// <param name="exception">
		///		The <see cref="Exception"/> that occurred when parsing if any.
		/// </param>
		public StatusDataParseError(string lineContent, string message, Exception exception = null)
		{
			LineContent = lineContent;
			Message = message;
			Exception = exception;
		}
	}
}
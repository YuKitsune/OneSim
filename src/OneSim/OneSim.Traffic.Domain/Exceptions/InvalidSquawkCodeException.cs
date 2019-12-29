namespace OneSim.Traffic.Domain.Exceptions
{
	using System;

	/// <summary>
	/// 	The Invalid Squawk Code <see cref="Exception"/>.
	/// </summary>
	public class InvalidSquawkCodeException : Exception
	{
		/// <summary>
		/// 	Gets the invalid squawk code.
		/// </summary>
		public string Code { get; }

		/// <summary>
		/// 	Gets the reason why the squawk code is invalid.
		/// </summary>
		public string Reason { get; }

		/// <summary>
		/// 	Initializes a new instance of the <see cref="InvalidSquawkCodeException"/> class.
		/// </summary>
		/// <param name="code">
		///		The squawk code.
		/// </param>
		/// <param name="reason">
		///		The reason which the <paramref name="code"/> is invalid.
		/// </param>
		/// <param name="message">
		///		The message.
		/// </param>
		/// <param name="innerException">
		///		The inner <see cref="Exception"/> if any.
		/// </param>
		public InvalidSquawkCodeException(
			string code,
			string reason = "",
			string message = "",
			Exception innerException = null) : base((string.IsNullOrEmpty(message) ?
														 $"The value \"{code}\" is not a valid squawk code.{(string.IsNullOrEmpty(reason) ? string.Empty : $" Reason: {reason}")}" :
														 message),
													innerException)
		{
			Code = code;
			Reason = reason;
		}
	}
}
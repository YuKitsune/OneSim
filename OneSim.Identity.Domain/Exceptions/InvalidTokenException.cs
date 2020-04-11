namespace OneSim.Identity.Domain.Exceptions
{
	using System;

	/// <summary>
	/// 	The Invalid Token <see cref="Exception"/>.
	/// </summary>
	public class InvalidTokenException : Exception
	{
		/// <summary>
		/// 	Gets or sets the invalid token.
		/// </summary>
		public string Token { get; }

		/// <summary>
		/// 	Initializes a new instance of the <see cref="InvalidTokenException"/> class.
		/// </summary>
		/// <param name="message">
		///		The message.
		/// </param>
		/// <param name="token">
		///		The invalid token.
		/// </param>
		/// <param name="innerException">
		///		The inner <see cref="Exception"/> if any.
		/// </param>
		public InvalidTokenException(string message, string token, Exception innerException = null) :
			base(message, innerException) => Token = token;
	}
}
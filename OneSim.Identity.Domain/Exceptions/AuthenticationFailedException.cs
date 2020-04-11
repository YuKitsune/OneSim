namespace OneSim.Identity.Domain.Exceptions
{
	using System;

	/// <summary>
	/// 	The Authentication Failed <see cref="Exception"/>.
	/// </summary>
	public class AuthenticationFailedException : Exception
	{
		/// <summary>
		/// 	Initializes a new instance of the <see cref="AuthenticationFailedException"/> class.
		/// </summary>
		/// <param name="message">
		///		The message.
		/// </param>
		/// <param name="innerException">
		///		The inner <see cref="Exception"/> if any.
		/// </param>
		public AuthenticationFailedException(string message, Exception innerException = null) :
			base(message, innerException)
		{
		}
	}
}	
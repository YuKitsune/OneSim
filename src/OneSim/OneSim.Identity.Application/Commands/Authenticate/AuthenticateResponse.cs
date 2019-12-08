namespace OneSim.Identity.Application.Commands.Authenticate
{
	using System;

	using OneSim.Identity.Domain.Exceptions;

	/// <summary>
	///		The Authentication Response.
	/// </summary>
	public class AuthenticateResponse
	{
		/// <summary>
		/// 	Gets the authentication token.
		/// </summary>
		public string Token { get; }

		/// <summary>
		/// 	Initializes a new instance of the <see cref="AuthenticateResponse"/> class.
		/// </summary>
		/// <param name="token">
		///		The token.
		/// </param>
		public AuthenticateResponse(string token)
		{
			if (string.IsNullOrEmpty(token)) throw new InvalidTokenException("The token cannot be null or empty", token, new ArgumentNullException(nameof(token)));

			Token = token;
		}
	}
}
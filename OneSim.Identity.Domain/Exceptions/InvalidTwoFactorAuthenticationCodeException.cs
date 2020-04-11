namespace OneSim.Identity.Domain.Exceptions
{
	using System;

	/// <summary>
	/// 	The Invalid Two-Factor Authentication Code <see cref="Exception"/>.
	/// </summary>
	public class InvalidTwoFactorAuthenticationCodeException : Exception
	{
		/// <summary>
		/// 	Gets the invalid Two-Factor Authentication code.
		/// </summary>
		public string Code { get; }

		/// <summary>
		/// 	Initializes a new instance of the <see cref="InvalidTwoFactorAuthenticationCodeException"/>.
		/// </summary>
		/// <param name="code">
		///		The invalid Two-Factor Authentication code.
		/// </param>
		/// <param name="message">
		///		The message.
		/// </param>
		/// <param name="innerException">
		///		The inner <see cref="Exception"/> if any.
		/// </param>
		public InvalidTwoFactorAuthenticationCodeException(string code, string message = "", Exception innerException = null) :
			base(message, innerException) => Code = code;
	}
}
namespace OneSim.Api.Data.Responses
{
	/// <summary>
	/// 	The Enable Two-Factor Authentication <see cref="BaseResponse"/>.
	/// </summary>
	public class EnableTwoFactorAuthenticationResponse : BaseResponse
	{
		/// <summary>
		///		Gets or sets the shared key
		/// </summary>
		public string SharedKey { get; set; }

		/// <summary>
		///		Gets or sets the authenticator URI.
		/// </summary>
		public string AuthenticatorUri { get; set; }

		/// <summary>
		/// 	Initializes a new instance of the <see cref="RecoveryCodeResponse"/> class.
		/// </summary>
		/// <param name="status">
		///		The <see cref="ResponseStatus"/>.
		/// </param>
		/// <param name="message">
		///		The message.
		/// </param>
		public EnableTwoFactorAuthenticationResponse(ResponseStatus status, string message) : base(status, message)
		{
		}
	}
}
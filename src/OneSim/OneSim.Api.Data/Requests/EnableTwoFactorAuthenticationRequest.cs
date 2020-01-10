namespace OneSim.Api.Data.Requests
{
	/// <summary>
	/// 	The Enable Two Factor Authentication Request.
	/// </summary>
	public class EnableTwoFactorAuthenticationRequest
	{
		/// <summary>
		/// 	Gets or sets the verification code.
		/// </summary>
		public string VerificationCode { get; set; }
	}
}
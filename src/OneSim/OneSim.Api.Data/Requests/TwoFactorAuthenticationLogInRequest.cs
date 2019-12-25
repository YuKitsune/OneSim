namespace OneSim.Api.Data.Requests
{
	/// <summary>
	/// 	The Two-Factor Authentication Log In Request.
	/// </summary>
	public class TwoFactorAuthenticationLogInRequest : BaseRequest
	{
		/// <summary>
		/// 	Gets or sets the UserName.
		/// </summary>
		public string UserName { get; set; }

		/// <summary>
		/// 	Gets or sets the Two-Factor Authentication Token.
		/// </summary>
		public string Token { get; set; }
	}
}
namespace OneSim.Api.Data.Requests
{
	/// <summary>
	/// 	The Reset Password Request.
	/// </summary>
	public class ResetPasswordRequest
	{
		/// <summary>
		/// 	Gets or sets the email address.
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// 	Gets or sets the new password.
		/// </summary>
		public string NewPassword { get; set; }

		/// <summary>
		/// 	Gets or sets the reset token.
		/// </summary>
		public string ResetToken { get; set; }
	}
}
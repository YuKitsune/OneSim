namespace OneSim.Api.Data.Requests
{
	/// <summary>
	/// 	The Confirm Email Request.
	/// </summary>
	public class ConfirmEmailRequest
	{
		/// <summary>
		/// 	Gets or sets the email address.
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// 	Gets or sets the confirmation token.
		/// </summary>
		public string ConfirmationCode { get; set; }
	}
}
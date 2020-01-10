namespace OneSim.Api.Data.Requests
{
	/// <summary>
	/// 	The Confirm Email Request.
	/// </summary>
	public class ConfirmEmailRequest
	{
		/// <summary>
		/// 	Gets or sets the confirmation token.
		/// </summary>
		public string ConfirmationCode { get; set; }
	}
}
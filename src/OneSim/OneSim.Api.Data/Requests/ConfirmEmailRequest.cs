namespace OneSim.Api.Data.Requests
{
	/// <summary>
	/// 	The Confirm Email Request.
	/// </summary>
	public class ConfirmEmailRequest : BaseRequest
	{
		/// <summary>
		/// 	Gets or sets the confirmation token.
		/// </summary>
		public string ConfirmationCode { get; set; }
	}
}
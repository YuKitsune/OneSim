namespace OneSim.Api.Data.Requests
{
	/// <summary>
	/// 	The Base Request.
	/// </summary>
	public class BaseRequest
	{
		/// <summary>
		/// 	Gets or sets the email address of the user submitting the current <see cref="BaseRequest"/>.
		/// </summary>
		public string Email { get; set; }
	}
}
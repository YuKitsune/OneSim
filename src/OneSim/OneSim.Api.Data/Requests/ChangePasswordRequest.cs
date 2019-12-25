namespace OneSim.Api.Data.Requests
{
	/// <summary>
	/// 	The Change Password Request.
	/// </summary>
	public class ChangePasswordRequest : BaseRequest
	{
		/// <summary>
		/// 	Gets or sets the old password.
		/// </summary>
		public string OldPassword { get; set; }

		/// <summary>
		/// 	Gets or sets the new password.
		/// </summary>
		public string NewPassword { get; set; }
	}
}
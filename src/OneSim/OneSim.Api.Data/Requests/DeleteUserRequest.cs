namespace OneSim.Api.Data.Requests
{
	/// <summary>
	/// 	The Delete User Request.
	/// </summary>
	public class DeleteUserRequest
	{
		/// <summary>
		/// 	Gets or sets the username.
		/// </summary>
		public string UserName { get; set; }

		/// <summary>
		/// 	Gets or sets the password.
		/// </summary>
		public string Password { get; set; }
	}
}
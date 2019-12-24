namespace OneSim.Api.Data.Requests
{
	/// <summary>
	/// 	The Create User Request.
	/// </summary>
	public class CreateUserRequest
	{
		/// <summary>
		/// 	Gets or sets the UserName.
		/// </summary>
		public string UserName { get; set; }

		/// <summary>
		/// 	Gets or sets the Email address.
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// 	Gets or sets the password.
		/// </summary>
		public string Password { get; set; }
	}
}
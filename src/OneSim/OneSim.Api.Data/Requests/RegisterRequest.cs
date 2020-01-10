namespace OneSim.Api.Data.Requests
{
	/// <summary>
	/// 	The Register Request.
	/// </summary>
	public class RegisterRequest
	{
		/// <summary>
		/// 	Gets or sets the email address of the new user
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// 	Gets or sets the UserName.
		/// </summary>
		public string UserName { get; set; }

		/// <summary>
		/// 	Gets or sets the password.
		/// </summary>
		public string Password { get; set; }
	}
}
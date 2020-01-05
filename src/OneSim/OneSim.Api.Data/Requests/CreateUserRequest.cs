namespace OneSim.Api.Data.Requests
{
	/// <summary>
	/// 	The Create User Request.
	/// </summary>
	public class CreateUserRequest : BaseRequest
	{
		/// <summary>
		/// 	Gets or sets the email of the user to create.
		/// </summary>
		public string NewUsersEmail { get; set; }

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
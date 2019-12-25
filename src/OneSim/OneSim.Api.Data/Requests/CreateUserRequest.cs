namespace OneSim.Api.Data.Requests
{
	/// <summary>
	/// 	The Create User Request.
	/// </summary>
	public class CreateUserRequest : BaseRequest
	{
		// Todo: The Email property in BaseRequest refers to the email of the current user. Since this also registers new users, should we use a different object to prevent confusion?

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
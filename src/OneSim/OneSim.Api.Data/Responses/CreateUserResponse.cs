namespace OneSim.Api.Data.Responses
{
	/// <summary>
	/// 	The Create User response.
	/// </summary>
	public class CreateUserResponse
	{
		/// <summary>
		/// 	Gets or sets a value indicating whether or not the user was created.
		/// </summary>
		public bool UserCreated { get; set; }

		/// <summary>
		/// 	Gets or sets the message.
		/// </summary>
		public string Message { get; set; }
	}
}
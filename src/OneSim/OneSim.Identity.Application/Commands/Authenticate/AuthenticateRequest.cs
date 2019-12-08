namespace OneSim.Identity.Application.Commands.Authenticate
{
	using MediatR;

	/// <summary>
	///		The Authentication <see cref="IRequest"/>.
	/// </summary>
	public class AuthenticateRequest : IRequest<AuthenticateResponse>
	{
		/// <summary>
		/// 	Gets or sets the users username.
		/// </summary>
		public string UserNameOrEmail { get; set; }

		/// <summary>
		/// 	Gets or sets the users password.
		/// </summary>
		public string Password { get; set; }
	}
}
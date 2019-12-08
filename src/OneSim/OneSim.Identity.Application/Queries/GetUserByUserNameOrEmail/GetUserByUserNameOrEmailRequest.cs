namespace OneSim.Identity.Application.Queries.GetUserByUserNameOrEmail
{
	using MediatR;

	using OneSim.Identity.Domain.Entities;

	/// <summary>
	///		The Get <see cref="ApplicationUser"/> By <see cref="ApplicationUser.UserName"/> or <see cref="ApplicationUser.Email"/> <see cref="IRequest{TResponse}"/>.
	/// </summary>
	public class GetUserByUserNameOrEmailRequest : IRequest<GetUserResponse>
	{
		/// <summary>
		/// 	Gets or sets the user name or email.
		/// </summary>
		public string UserNameOrEmail { get; set; }
	}
}
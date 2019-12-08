namespace OneSim.Identity.Application.Queries.GetUserById
{
	using MediatR;

	using OneSim.Identity.Domain.Entities;

	/// <summary>
	///		The Get <see cref="ApplicationUser"/> By <see cref="ApplicationUser.Id"/> <see cref="IRequest{TResponse}"/>.
	/// </summary>
	public class GetUserByIdRequest : IRequest<GetUserResponse>
	{
		/// <summary>
		/// 	Gets or sets the <see cref="ApplicationUser.Id"/>.
		/// </summary>
		public string UserId { get; set; }
	}
}
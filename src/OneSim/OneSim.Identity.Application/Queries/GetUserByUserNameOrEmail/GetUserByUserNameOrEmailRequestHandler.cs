namespace OneSim.Identity.Application.Queries.GetUserByUserNameOrEmail
{
	using System.Threading;
	using System.Threading.Tasks;

	using MediatR;

	using Microsoft.EntityFrameworkCore;

	using OneSim.Identity.Application.Interfaces;
	using OneSim.Identity.Application.Queries.GetUserById;
	using OneSim.Identity.Domain.Entities;
	using OneSim.Identity.Domain.Exceptions;

	/// <summary>
	///		The <see cref="GetUserByUserNameOrEmailRequest"/> <see cref="IRequestHandler{TRequest}"/>.
	/// </summary>
	public class GetUserByUserNameOrEmailRequestHandler : IRequestHandler<GetUserByUserNameOrEmailRequest, GetUserResponse>
	{
		/// <summary>
		/// 	The <see cref="IIdentityDbContext"/>.
		/// </summary>
		public readonly IIdentityDbContext DbContext;

		/// <summary>
		/// 	Initializes a new instance of the <see cref="GetUserByUserNameOrEmailRequestHandler"/>.
		/// </summary>
		/// <param name="dbContext">
		///		The <see cref="IIdentityDbContext"/>.
		/// </param>
		public GetUserByUserNameOrEmailRequestHandler(IIdentityDbContext dbContext) => DbContext = dbContext;

		/// <summary>
		/// 	Handles the <see cref="GetUserByUserNameOrEmailRequest"/>.
		/// </summary>
		/// <param name="request">
		///		The <see cref="GetUserByUserNameOrEmailRequest"/>.
		/// </param>
		/// <param name="cancellationToken">
		///		The <see cref="CancellationToken"/>.
		/// </param>
		/// <returns>
		///		The <see cref="GetUserResponse"/>.
		/// </returns>
		public async Task<GetUserResponse> Handle(GetUserByUserNameOrEmailRequest request, CancellationToken cancellationToken)
		{
			// Try the UserName first
			ApplicationUser user = await DbContext.Users.FirstOrDefaultAsync(u => u.UserName == request.UserNameOrEmail, cancellationToken);

			// Try the email if not found
			if (user == null) user = await DbContext.Users.FirstOrDefaultAsync(u => u.Email == request.UserNameOrEmail, cancellationToken);

			// Throw if still not found
			if (user == null) throw new UserNotFoundException($"Unable to find user with UserName or Email \"{request.UserNameOrEmail}\".");

			// Return response otherwise
			return new GetUserResponse(user);
		}
	}
}
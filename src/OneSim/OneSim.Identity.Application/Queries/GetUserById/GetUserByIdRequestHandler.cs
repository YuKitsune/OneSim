namespace OneSim.Identity.Application.Queries.GetUserById
{
	using System.Threading;
	using System.Threading.Tasks;

	using MediatR;

	using Microsoft.EntityFrameworkCore;

	using OneSim.Identity.Application.Interfaces;
	using OneSim.Identity.Domain.Entities;
	using OneSim.Identity.Domain.Exceptions;

	/// <summary>
	///		The <see cref="GetUserByIdRequest"/> <see cref="IRequestHandler{TRequest, TResponse}"/>.
	/// </summary>
	public class GetUserByIdRequestHandler : IRequestHandler<GetUserByIdRequest, GetUserResponse>
	{
		/// <summary>
		/// 	The <see cref="IIdentityDbContext"/>.
		/// </summary>
		public readonly IIdentityDbContext DbContext;

		/// <summary>
		/// 	Initializes a new instance of the <see cref="GetUserByIdRequestHandler"/>.
		/// </summary>
		/// <param name="dbContext">
		///		The <see cref="IIdentityDbContext"/>.
		/// </param>
		public GetUserByIdRequestHandler(IIdentityDbContext dbContext) => DbContext = dbContext;

		/// <summary>
		/// 	Handles the <see cref="GetUserByIdRequest"/>.
		/// </summary>
		/// <param name="request">
		///		The <see cref="GetUserByIdRequest"/>.
		/// </param>
		/// <param name="cancellationToken">
		///		The <see cref="CancellationToken"/>.
		/// </param>
		/// <returns>
		///		The <see cref="GetUserResponse"/>.
		/// </returns>
		public async Task<GetUserResponse> Handle(GetUserByIdRequest request, CancellationToken cancellationToken)
		{
			// Get the user
			ApplicationUser user = await DbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

			// Throw if still not found
			if (user == null) throw new UserNotFoundException($"Unable to find user with ID \"{request.UserId}\".");

			// Return response otherwise
			return new GetUserResponse(user);
		}
	}
}
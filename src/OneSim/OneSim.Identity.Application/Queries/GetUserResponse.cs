namespace OneSim.Identity.Application.Queries
{
	using System;

	using OneSim.Identity.Domain.Entities;

	/// <summary>
	/// 	The GetUserBy[whatever] response.
	/// </summary>
	public class GetUserResponse
	{
		/// <summary>
		/// 	Gets the <see cref="ApplicationUser"/> found.
		/// </summary>
		public ApplicationUser User { get; }

		/// <summary>
		/// 	Initializes a new instance of the <see cref="GetUserResponse"/>.
		/// </summary>
		/// <param name="user"></param>
		public GetUserResponse(ApplicationUser user) => User = user ?? throw new ArgumentNullException(nameof(user));
	}
}
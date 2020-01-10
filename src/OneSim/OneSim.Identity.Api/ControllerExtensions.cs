namespace OneSim.Identity.Web
{
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Mvc;
	using Microsoft.EntityFrameworkCore;

	using OneSim.Identity.Application.Exceptions;
	using OneSim.Identity.Domain.Entities;
	using OneSim.Identity.Persistence;

	/// <summary>
	/// 	The <see cref="Controller"/> extensions.
	/// </summary>
	public static class ControllerExtensions
	{
		/// <summary>
		/// 	Gets the currently logged in <see cref="ApplicationUser"/>.
		/// </summary>
		/// <param name="controller">
		///		The <see cref="Controller"/>.
		/// </param>
		/// <param name="dbContext">
		///		The <see cref="ApplicationIdentityDbContext"/>.
		/// </param>
		/// <returns>
		///		The <see cref="ApplicationUser"/>.
		/// </returns>
		public static async Task<ApplicationUser> GetCurrentUserAsync(this Controller controller, ApplicationIdentityDbContext dbContext)
		{
            // Get the user from the user ID
			string userId = controller.HttpContext.User.Identity.Name;
			ApplicationUser user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

			if (user == null) throw new UserNotFoundException(userId, $"Unable to find user with ID \"{userId}\".");

			return user;
		}
	}
}
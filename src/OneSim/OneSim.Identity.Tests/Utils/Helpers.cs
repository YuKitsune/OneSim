namespace OneSim.Identity.Tests.Utils
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Microsoft.AspNetCore.Authentication;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Options;

	using Moq;

	using OneSim.Identity.Application.Interfaces;
	using OneSim.Identity.Domain.Entities;

	/// <summary>
	///     The helpers.
	/// </summary>
	public static class Helpers
	{
		/// <summary>
		///     Gets a new <see cref="IIdentityDbContext"/>.
		/// </summary>
		/// <returns>
		/// 	A mocked <see cref="IIdentityDbContext"/>.
		/// </returns>
		public static IIdentityDbContext GetDbContext() => new MockDbContext(new DbContextOptionsBuilder<MockDbContext>()
																			.UseInMemoryDatabase(Guid.NewGuid().ToString())
																			.Options);

		/// <summary>
		/// 	Gets a new <see cref="UserManager{TUser}"/> for the <see cref="ApplicationUser"/>.
		/// </summary>
		/// <param name="dbContext">
		///		The <see cref="IIdentityDbContext"/>.
		/// </param>
		/// <returns>
		///		The mocked <see cref="UserManager{TUser}"/>.
		/// </returns>
		public static UserManager<ApplicationUser> GetUserManager(IIdentityDbContext dbContext)
		{
			Mock<IUserStore<ApplicationUser>> mockStore = new Mock<IUserStore<ApplicationUser>>();
			Mock<UserManager<ApplicationUser>> mockManager = new Mock<UserManager<ApplicationUser>>(mockStore.Object, null, null, null, null, null, null, null, null);
			mockManager.Object.UserValidators.Add(new UserValidator<ApplicationUser>());
			mockManager.Object.PasswordValidators.Add(new PasswordValidator<ApplicationUser>());

			mockManager.Setup(x => x.DeleteAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
			mockManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
					   .ReturnsAsync(IdentityResult.Success)
					   .Callback<ApplicationUser, string>((x, y) =>
														  {
															  dbContext.Users.Add(x);
															  dbContext.SaveChanges();
														  });
			mockManager.Setup(x => x.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
					   .ReturnsAsync(IdentityResult.Success)
					   .Callback<ApplicationUser, string>((user, token) =>
														  {
															  ApplicationUser targetUser = dbContext
																 .Users.FirstOrDefault(u => u.Email == user.Email);
															  targetUser.EmailConfirmed = true;
															  dbContext.SaveChanges();
														  });
			mockManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);

			return mockManager.Object;
		}

		/// <summary>
		/// 	Gets a new <see cref="SignInManager{TUser}"/> for the <see cref="ApplicationUser"/>.
		/// </summary>
		/// <param name="userManager">
		///		The <see cref="UserManager{TUser}"/>.
		/// </param>
		/// <returns>
		///		The mocked <see cref="SignInManager{TUser}"/>.
		/// </returns>
		public static SignInManager<ApplicationUser> GetSignInManager(UserManager<ApplicationUser> userManager)
		{
			return new SignInManager<ApplicationUser>(userManager,
													  new HttpContextAccessor(),
													  new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
													  new Mock<IOptions<IdentityOptions>>().Object,
													  new Mock<ILogger<SignInManager<ApplicationUser>>>().Object,
													  new Mock<IAuthenticationSchemeProvider>().Object);
		}

		/// <summary>
		/// 	Gets a new <see cref="IUrlHelper"/>.
		/// </summary>
		/// <returns>
		///		The mocked <see cref="IUrlHelper"/>.
		/// </returns>
		public static IUrlHelper GetUrlHelper()
		{
			Mock<IUrlHelper> mockUrlHelper = new Mock<IUrlHelper>();
			mockUrlHelper
			   .Setup(u => u.EmailConfirmationLink(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
			   .Returns((
							string
								userId,
							string
								token,
							string
								scheme) => $"{scheme}://test.com?user={userId}&token={token}");
			mockUrlHelper.Setup(u => u.ResetPasswordCallbackLink(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
						 .Returns((
									  string
										  userId,
									  string
										  token,
									  string
										  scheme) => $"{scheme}://test.com?user={userId}&token={token}");

			return mockUrlHelper.Object;
		}
	}
}
namespace OneSim.Identity.Tests.Utils
{
	using Microsoft.AspNetCore.Identity;
	using Microsoft.Extensions.Logging;

	using Moq;

	using OneSim.Identity.Application.Interfaces;
	using OneSim.Identity.Domain.Entities;

	/// <summary>
	/// 	A collection of pre-made mock objects.
	/// </summary>
	public class MockSet <TLogCategory> where TLogCategory : class
	{
		/// <summary>
		/// 	Gets the <see cref="IIdentityDbContext"/>.
		/// </summary>
		public IIdentityDbContext DbContext { get; }

		/// <summary>
		/// 	Gets the <see cref="UserManager{TUser}"/> for the <see cref="ApplicationUser"/>.
		/// </summary>
		public UserManager<ApplicationUser> UserManager { get; }

		/// <summary>
		/// 	Gets the <see cref="SignInManager{TUser}"/> for the <see cref="ApplicationUser"/>.
		/// </summary>
		public SignInManager<ApplicationUser> SignInManager { get; }

		/// <summary>
		/// 	Gets the <see cref="ILogger{TCategoryName}"/>.
		/// </summary>
		public ILogger<TLogCategory> Logger { get; }

		/// <summary>
		/// 	Gets the <see cref="IUrlHelper"/>.
		/// </summary>
		public IUrlHelper UrlHelper { get; }

		/// <summary>
		/// 	Gets the <see cref="MockEmailSender"/>.
		/// </summary>
		public MockEmailSender EmailSender { get; }

		/// <summary>
		/// 	Initializes a new instance of the <see cref="MockSet{TLogCategory}"/>.
		/// </summary>
		public MockSet()
		{
			DbContext = Helpers.GetDbContext();
			UserManager = Helpers.GetUserManager(DbContext);
			SignInManager = Helpers.GetSignInManager(UserManager);
			Logger = new Mock<ILogger<TLogCategory>>().Object;
			UrlHelper = Helpers.GetUrlHelper();
			EmailSender = new MockEmailSender();
		}
	}
}
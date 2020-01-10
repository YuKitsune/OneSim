namespace OneSim.Identity.Web.Controllers
{
	using System;
	using System.Security.Claims;
	using System.Threading.Tasks;

	using IdentityModel;

	using IdentityServer4;
	using IdentityServer4.Models;
	using IdentityServer4.Services;

	using Microsoft.AspNetCore.Authentication;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.Logging;

	using OneSim.Identity.Domain.Entities;
	using OneSim.Identity.Persistence;
	using OneSim.Identity.Web.Models.ViewModels.Authentication;

	using SignInResult = Microsoft.AspNetCore.Mvc.SignInResult;

	/// <summary>
	/// 	The Authentication <see cref="Controller"/>.
	/// </summary>
	public class AuthenticationController : Controller
	{
		/// <summary>
		///     The <see cref="ApplicationIdentityDbContext"/>.
		/// </summary>
		private readonly ApplicationIdentityDbContext _dbContext;

		/// <summary>
		///     The <see cref="AuthenticationService"/>.
		/// </summary>
		private readonly AuthenticationService _authenticationService;

		/// <summary>
		/// 	The <see cref="IIdentityServerInteractionService"/>.
		/// </summary>
		private readonly IIdentityServerInteractionService _interactionService;

		/// <summary>
		/// 	The <see cref="IConfiguration"/>.
		/// </summary>
		private readonly IConfiguration _configuration;

		/// <summary>
		///     The <see cref="ILogger"/>.
		/// </summary>
		private readonly ILogger<AuthenticationController> _logger;

		/// <summary>
		///     Initializes a new instance of the <see cref="AuthenticationController"/> class.
		/// </summary>
		/// <param name="service">
		///     The <see cref="AuthenticationService"/>.
		/// </param>
		/// <param name="dbContext">
		///     The <see cref="ApplicationIdentityDbContext"/>.
		/// </param>
		/// <param name="identityService">
		///		The <see cref="IIdentityServerInteractionService"/>.
		/// </param>
		/// <param name="configuration">
		///		The <see cref="IConfiguration"/>.
		/// </param>
		/// <param name="logger">
		///    The <see cref="ILogger{TCategoryName}"/>.
		/// </param>
		public AuthenticationController(
			AuthenticationService service,
			ApplicationIdentityDbContext dbContext,
			IIdentityServerInteractionService identityService,
			IConfiguration configuration,
			ILogger<AuthenticationController> logger)
		{
			_authenticationService = service;
			_dbContext = dbContext;
			_interactionService = identityService;
			_configuration = configuration;
			_logger = logger;
		}

		/// <summary>
		/// 	Returns the LogIn view.
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> Login(string returnUrl)
		{
			// Get the authorization request
			AuthorizationRequest context = await _interactionService.GetAuthorizationContextAsync(returnUrl);

			if (context?.IdP != null) throw new NotImplementedException("External login is not implemented!");

			// Create the ViewModel
			LoginViewModel viewModel = new LoginViewModel { ReturnUrl = returnUrl, Email = context?.LoginHint };

			ViewData["ReturnUrl"] = returnUrl;

			return View(viewModel);
		}

		/// <summary>
		/// 	Handles the LogIn request.
		/// </summary>
		/// <param name="model">
		///		The <see cref="LoginViewModel"/>.
		/// </param>
		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			// If the model is valid, continue with the login
			if (ModelState.IsValid)
			{
				// Get the user
				ApplicationUser user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

				// Check if the user and their credentials are valid
				if (user != null &&
					await _authenticationService.ValidateCredentials(user, model.Password))
				{
					// Get the token lifetime from the configuration file
					int tokenLifetime = _configuration.GetValue("TokenLifetimeMinutes", 120);

					AuthenticationProperties props = new AuthenticationProperties
													 {
														 ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(tokenLifetime),
														 AllowRefresh = true,
														 RedirectUri = model.ReturnUrl
													 };

					// If requested to remember the login, then configure the persistent login 
					if (model.RememberMe)
					{
						int permanentTokenLifetime = _configuration.GetValue("PermanentTokenLifetimeDays", 365);

						props.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(permanentTokenLifetime);
						props.IsPersistent = true;
					}

					// Sign the user in
					// Todo: Redirect for 2FA
					SignInResult result = await _authenticationService.SignInAsync(user, props);

					// Make sure the returnUrl is still valid, and if so, redirect back to authorize endpoint
					return Redirect(_interactionService.IsValidReturnUrl(model.ReturnUrl) ? model.ReturnUrl : "~/");
				}

				// Throw a generic error if invalid
				ModelState.AddModelError("", "Invalid username or password.");
			}

			// Something went wrong, show form with error
			LoginViewModel vm = await BuildLoginViewModelAsync(model);

			ViewData["ReturnUrl"] = model.ReturnUrl;

			return View(vm);
		}

		/// <summary>
		/// 	Returns the Logout view.
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> Logout(string logoutId)
		{
			// If the user is not authenticated, then just show logged out page
			if (!User.Identity.IsAuthenticated)
			{
				return await Logout(new LogoutViewModel { LogoutId = logoutId });
			}

			// Test for Xamarin
			LogoutRequest context = await _interactionService.GetLogoutContextAsync(logoutId);
			if (context?.ShowSignoutPrompt == false)
			{
				// It's safe to automatically sign-out
				return await Logout(new LogoutViewModel { LogoutId = logoutId });
			}

			// Show the logout prompt.
			// This prevents attacks where the user is automatically signed out by another malicious web page.
			LogoutViewModel vm = new LogoutViewModel
								 {
									 LogoutId = logoutId
								 };

			return View(vm);
		}

		/// <summary>
		/// 	Handles the LogOut request.
		/// </summary>
		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> Logout(LogoutViewModel model)
		{
			// Get the identity provider
			string identityProvider = User?.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;

			// Check if the provider matches the local provider
			if (!string.IsNullOrEmpty(identityProvider) &&
				identityProvider != IdentityServerConstants.LocalIdentityProvider)
			{
				if (model.LogoutId == null)
				{
					// if there's no current logout context, we need to create one
					// this captures necessary info from the current logged in user
					// before we sign out and redirect away to the external IdP for sign out
					model.LogoutId = await _interactionService.CreateLogoutContextAsync();
				}

				// Todo: Update this URL
				string url = "/Account/Logout?logoutId=" + model.LogoutId;

				try
				{
					// Hack: try/catch to handle social providers that throw
					await HttpContext.SignOutAsync(identityProvider,
												   new AuthenticationProperties { RedirectUri = url });
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "LOGOUT ERROR: {ExceptionMessage}", ex.Message);
				}
			}

			// Delete authentication cookie
			await HttpContext.SignOutAsync();

			await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

			// Set this so UI rendering sees an anonymous user
			HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

			// Get context information (client name, post logout redirect URI and iframe for federated sign out)
			LogoutRequest logout = await _interactionService.GetLogoutContextAsync(model.LogoutId);

			return Redirect(logout?.PostLogoutRedirectUri);
		}
	}
}
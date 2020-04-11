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

	using MimeKit.Text;

	using OneSim.Identity.Application;
	using OneSim.Identity.Domain.Entities;
	using OneSim.Identity.Persistence;
	using OneSim.Identity.Web.Models.ViewModels.Authentication;

	using AuthenticationService = Microsoft.AspNetCore.Authentication.AuthenticationService;

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
		/// 	The <see cref="UserService"/>.
		/// </summary>
		private readonly UserService _userService;

		/// <summary>
		///     The <see cref="Microsoft.AspNetCore.Authentication.AuthenticationService"/>.
		/// </summary>
		private readonly Application.AuthenticationService _authenticationService;

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
		/// <param name="dbContext">
		///     The <see cref="ApplicationIdentityDbContext"/>.
		/// </param>
		/// <param name="userService">
		///     The <see cref="UserService"/>.
		/// </param>
		/// <param name="authenticationService">
		///     The <see cref="AuthenticationService"/>.
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
			UserService userService,
			Application.AuthenticationService authenticationService,
			ApplicationIdentityDbContext dbContext,
			IIdentityServerInteractionService identityService,
			IConfiguration configuration,
			ILogger<AuthenticationController> logger)
		{
			_dbContext = dbContext;
			_userService = userService;
			_authenticationService = authenticationService;
			_interactionService = identityService;
			_configuration = configuration;
			_logger = logger;
		}

		/// <summary>
		/// 	Returns the Login view.
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> Login(string callbackUri)
		{
			// Get the authorization request
			AuthorizationRequest context = await _interactionService.GetAuthorizationContextAsync(callbackUri);

			if (context?.IdP != null) throw new NotImplementedException("External login is not implemented!");

			return View(new LoginViewModel { CallbackUri = callbackUri, Email = context?.LoginHint });
		}

		/// <summary>
		/// 	Handles the Login request.
		/// </summary>
		/// <param name="viewModel">
		///		The <see cref="LoginViewModel"/>.
		/// </param>
		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel viewModel)
		{
			// If the model is valid, continue with the login
			if (ModelState.IsValid)
			{
				// Get the user
				ApplicationUser user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == viewModel.Email);

				// Check if the user and their credentials are valid
				if (user != null &&
					await _userService.UserManager.CheckPasswordAsync(user, viewModel.Password))
				{
					// Get the token lifetime from the configuration file, otherwise default to 120
					int tokenLifetime = _configuration.GetValue("TokenLifetimeMinutes", 120);

					AuthenticationProperties props = new AuthenticationProperties
													 {
														 ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(tokenLifetime),
														 AllowRefresh = true,
														 RedirectUri = viewModel.CallbackUri
													 };

					// If requested to remember the login, then configure the permanent token
					if (viewModel.RememberMe)
					{
						int permanentTokenLifetime = _configuration.GetValue("PermanentTokenLifetimeDays", 365);

						props.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(permanentTokenLifetime);
						props.IsPersistent = true;
					}

					// Sign the user in
					Microsoft.AspNetCore.Identity.SignInResult result = await _authenticationService.LogIn(user, viewModel.Password);

					// If successful, make sure the callback URI is still valid,
					// and if so, redirect back to authorize endpoint
					if (result.Succeeded)
					{
						if (_interactionService.IsValidReturnUrl(viewModel.CallbackUri))
						{
							return Redirect(viewModel.CallbackUri);
						}

						return RedirectToAction(nameof(AccountController.Index),
												Utils.GetControllerName(nameof(AccountController)));
					}

					// If 2FA is enabled, then redirect to the 2FA login view
					if (result.RequiresTwoFactor)
						return RedirectToAction(nameof(LoginWithTwoFactorAuthentication), new { viewModel.RememberMe, ReturnUrl = viewModel.CallbackUri });

					// If locked out, then redirect to the lockout view
					if (result.IsLockedOut) return RedirectToAction(nameof(Lockout));
				}

				// Throw a generic error if invalid, can't be specific about what went wrong for security reasons
				ModelState.AddModelError("", "Invalid username or password.");
			}

			// Something went wrong, clean the view model and go back
			return View(await BuildLoginViewModelAsync(viewModel));
		}

		/// <summary>
		///		Returns the Two-Factor Authentication Login View.
		/// </summary>
		/// <param name="rememberMe">
		///		The value indicating whether or not to remember the user.
		/// </param>
		/// <param name="callbackUri">
		///		The URI to redirect to once login has completed.
		/// </param>
		/// <returns>
		///		The <see cref="IActionResult"/>.
		/// </returns>
		[HttpGet]
		public async Task<IActionResult> LoginWithTwoFactorAuthentication(bool rememberMe, string callbackUri)
		{
			// Ensure the user has gone through the username & password screen first
			ApplicationUser user = await _authenticationService.SignInManager.GetTwoFactorAuthenticationUserAsync();

			// Throw if we don't have a user
			if (user == null) throw new ApplicationException("Unable to load two-factor authentication user.");

			// Create the model and send back with the view
			return View(new TwoFactorLoginViewModel { RememberMe = rememberMe, CallbackUri = callbackUri });
		}

		/// <summary>
		///		Logs the user in with Two-Factor Authentication.
		/// </summary>
		/// <param name="viewModel">
		///		The <see cref="TwoFactorLoginViewModel"/>.
		/// </param>
		/// <returns>
		///		The <see cref="IActionResult"/>.
		/// </returns>
		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> LoginWithTwoFactorAuthentication(TwoFactorLoginViewModel viewModel)
		{
			// If invalid, re-display the form and try again
			if (!ModelState.IsValid) return View(viewModel);

			// Get the user
			ApplicationUser user = await _authenticationService.SignInManager.GetTwoFactorAuthenticationUserAsync();
			try
			{
				// Attempt to log in
				Microsoft.AspNetCore.Identity.SignInResult result =
					await _authenticationService.TwoFactorAuthenticationLogIn(user,
																			  viewModel.TwoFactorCode,
																			  viewModel.RememberMe,
																			  viewModel.RememberMachine);

				// If successful, make sure the callback URI is still valid,
				// and if so, redirect back to authorize endpoint
				if (result.Succeeded)
				{
					if (_interactionService.IsValidReturnUrl(viewModel.CallbackUri))
					{
						return Redirect(viewModel.CallbackUri);
					}

					return RedirectToAction(nameof(AccountController.Index),
											Utils.GetControllerName(nameof(AccountController)));
				}

				// If locked out, then redirect to the lockout view
				if (result.IsLockedOut) return RedirectToAction(nameof(Lockout));

				// 2FA code was wrong, try again
				ModelState.AddModelError(string.Empty, "Invalid authenticator code.");

				return View(new TwoFactorLoginViewModel
							{
								CallbackUri = viewModel.CallbackUri,
								RememberMe = viewModel.RememberMe,
								RememberMachine = viewModel.RememberMachine
							});
			}
			catch (Exception ex)
			{
				// Log the error
				_logger.LogError(ex, $"An error has occurred logging in user using 2FA with email \"{user.Email}\".");

				ModelState.AddModelError(string.Empty, "An error has occurred when attempting to log in.");

				return View(new TwoFactorLoginViewModel
							{
								CallbackUri = viewModel.CallbackUri,
								RememberMe = viewModel.RememberMe,
								RememberMachine = viewModel.RememberMachine
							});
			}
		}

		/// <summary>
		///		Returns the Two-Factor Authentication recovery code login view.
		/// </summary>
		/// <returns>
		///		The <see cref="IActionResult"/>.
		/// </returns>
		[HttpGet]
		public async Task<IActionResult> LoginWithRecoveryCode()
		{
			// Ensure the user has gone through the username & password screen first
			ApplicationUser user = await _authenticationService.SignInManager.GetTwoFactorAuthenticationUserAsync();

			// Throw if we don't have a user
			if (user == null) throw new ApplicationException("Unable to load two-factor authentication user.");

			return View();
		}

		/// <summary>
		///		Logs the user in using a 2FA recovery code.
		/// </summary>
		/// <param name="viewModel">
		///		The <see cref="TwoFactorRecoveryCodeLoginViewModel"/>.
		/// </param>
		/// <returns>
		///		The <see cref="IActionResult"/>.
		/// </returns>
		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> LoginWithRecoveryCode(TwoFactorRecoveryCodeLoginViewModel viewModel)
		{
			// If invalid, re-display the form and try again
			if (!ModelState.IsValid) return View(viewModel);

			// Get the user
			ApplicationUser user = await _authenticationService.SignInManager.GetTwoFactorAuthenticationUserAsync();

			if (user == null) throw new ApplicationException("Unable to load two-factor authentication user.");

			try
			{
				// Attempt to log in
				Microsoft.AspNetCore.Identity.SignInResult result = await _authenticationService.RecoveryCodeLogIn(user, viewModel.RecoveryCode);

				// If successful, make sure the callback URI is still valid,
				// and if so, redirect back to authorize endpoint
				if (result.Succeeded)
				{
					if (_interactionService.IsValidReturnUrl(viewModel.CallbackUri))
					{
						return Redirect(viewModel.CallbackUri);
					}

					return RedirectToAction(nameof(AccountController.Index),
											Utils.GetControllerName(nameof(AccountController)));
				}

				// If locked out, then redirect to the lockout view
				if (result.IsLockedOut) return RedirectToAction(nameof(Lockout));

				// 2FA recovery code was wrong, try again
				ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");

				return View(new TwoFactorRecoveryCodeLoginViewModel { CallbackUri = viewModel.CallbackUri });
			}
			catch (Exception ex)
			{
				// Log the error
				_logger.LogError(ex, $"An error has occurred logging in user using 2FA recovery code with email \"{user.Email}\".");

				ModelState.AddModelError(string.Empty, "An error has occurred when attempting to log in.");

				return View(new TwoFactorRecoveryCodeLoginViewModel { CallbackUri = viewModel.CallbackUri });
			}
		}

		/// <summary>
		/// 	Returns the Logout view.
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> Logout(string logoutId)
		{
			// If the user is not authenticated, then just show logged out page
			if (!User.Identity.IsAuthenticated) return await Logout(new LogoutViewModel { LogoutId = logoutId });

			// Test for Xamarin
			LogoutRequest context = await _interactionService.GetLogoutContextAsync(logoutId);
			if (context?.ShowSignoutPrompt == false)
			{
				// It's safe to automatically sign-out
				return await Logout(new LogoutViewModel { LogoutId = logoutId });
			}

			// Show the logout prompt.
			// This prevents attacks where the user is automatically signed out by another malicious web page.
			return View(new LogoutViewModel { LogoutId = logoutId });
		}

		/// <summary>
		/// 	Handles the Logout request.
		/// </summary>
		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> Logout(LogoutViewModel viewModel)
		{
			// Get the identity provider
			string identityProvider = User?.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;

			// Check if the provider matches the local provider
			if (!string.IsNullOrEmpty(identityProvider) &&
				identityProvider != IdentityServerConstants.LocalIdentityProvider)
			{
				if (viewModel.LogoutId == null)
				{
					// if there's no current logout context, we need to create one
					// this captures necessary info from the current logged in user
					// before we sign out and redirect away to the external IdP for sign out
					viewModel.LogoutId = await _interactionService.CreateLogoutContextAsync();
				}

				// Todo: Update this URL
				string url = "/Account/Logout?logoutId=" + viewModel.LogoutId;

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
			LogoutRequest logout = await _interactionService.GetLogoutContextAsync(viewModel.LogoutId);

			return Redirect(logout?.PostLogoutRedirectUri);
		}

		/// <summary>
		/// 	Handles the Logout request from a device.
		/// </summary>
		/// <param name="redirectUrl">
		///		The URL to redirect to after logging out.
		/// </param>
		/// <returns>
		///		The <see cref="IActionResult"/>.
		/// </returns>
		public async Task<IActionResult> DeviceLogout(string redirectUrl)
		{
			// Delete authentication cookie
			await HttpContext.SignOutAsync();

			// Set this so UI rendering sees an anonymous user
			HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

			return Redirect(redirectUrl);
		}

		/// <summary>
		/// 	Returns the redirecting view.
		/// </summary>
		/// <returns>
		///		The <see cref="IActionResult"/>.
		/// </returns>
		[HttpGet]
		public IActionResult Redirecting() => View();

		/// <summary>
		/// 	Returns the lockout view.
		/// </summary>
		/// <returns>
		///		The <see cref="IActionResult"/>.
		/// </returns>
		[HttpGet]
		public IActionResult Lockout() => View();

		#region Helpers

		/// <summary>
		/// 	Builds a clean <see cref="LoginViewModel"/> (for no password)
		/// 	given a dirty <see cref="LoginViewModel"/> (with password).
		/// </summary>
		/// <param name="viewModel">
		///		The dirty <see cref="LoginViewModel"/>.
		/// </param>
		/// <returns>
		///		The clean <see cref="LoginViewModel"/>.
		/// </returns>
		private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginViewModel viewModel)
		{
			AuthorizationRequest context = await _interactionService.GetAuthorizationContextAsync(viewModel.CallbackUri);
			LoginViewModel vm = new LoginViewModel { CallbackUri = viewModel.CallbackUri, Email = context?.LoginHint };
			vm.Email = viewModel.Email;
			vm.RememberMe = viewModel.RememberMe;

			return vm;
		}

		#endregion
	}
}
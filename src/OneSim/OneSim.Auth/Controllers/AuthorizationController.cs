namespace OneSim.Auth.Controllers
{
	using System;
	using System.Security.Claims;
	using System.Threading.Tasks;

	using IdentityModel;

	using IdentityServer4;
	using IdentityServer4.Models;
	using IdentityServer4.Services;

	using Microsoft.Extensions.Configuration;
	using Microsoft.AspNetCore.Authentication;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;

	using OneSim.Auth.Models.ViewModels.Authorization;
	using OneSim.Identity.Domain.Entities;

	/// <summary>
	/// 	The Authorization <see cref="Controller"/>.
	/// </summary>
	public class AuthorizationController : Controller
	{
		/// <summary>
		/// 	The <see cref="IIdentityServerInteractionService"/>.
		/// </summary>
		private readonly IIdentityServerInteractionService _interactionService;

		/// <summary>
		/// 	The <see cref="IConfiguration"/>.
		/// </summary>
		private readonly IConfiguration _configuration;

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
			LogInViewModel vm = new LogInViewModel { ReturnUrl = returnUrl, Email = context?.LoginHint };

			ViewData["ReturnUrl"] = returnUrl;

			return View(vm);
		}

		/// <summary>
		/// 	Handles the LogIn request.
		/// </summary>
		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LogInViewModel model)
		{
			// If the model is valid, continue with the login
			if (ModelState.IsValid)
			{
				// Get the user
				ApplicationUser user = await _loginService.FindByUsername(model.Email);

				// Check if the credentials are valid
				if (await _loginService.ValidateCredentials(user, model.Password))
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
					await _loginService.SignInAsync(user, props);

					// Make sure the returnUrl is still valid, and if so, redirect back to authorize endpoint
					return Redirect(_interactionService.IsValidReturnUrl(model.ReturnUrl) ? model.ReturnUrl : "~/");
				}

				// Throw a generic error if invalid
				ModelState.AddModelError("", "Invalid username or password.");
			}

			// Something went wrong, show form with error
			LogInViewModel vm = await BuildLoginViewModelAsync(model);

			ViewData["ReturnUrl"] = model.ReturnUrl;

			return View(vm);
		}

		/// <summary>
		/// 	Returns the LogOut view.
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> Logout(string logoutId)
		{
			// If the user is not authenticated, then just show logged out page
			if (!User.Identity.IsAuthenticated)
			{
				return await Logout(new LogOutViewModel { LogoutId = logoutId });
			}

			// Test for Xamarin
			LogoutRequest context = await _interactionService.GetLogoutContextAsync(logoutId);
			if (context?.ShowSignoutPrompt == false)
			{
				// It's safe to automatically sign-out
				return await Logout(new LogOutViewModel { LogoutId = logoutId });
			}

			// Show the logout prompt.
			// This prevents attacks where the user is automatically signed out by another malicious web page.
			LogOutViewModel vm = new LogOutViewModel
								 {
									 LogoutId = logoutId
								 };

			return View(vm);
		}

		/// <summary>
		/// 	Handles the LogOut request.
		/// </summary>
		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> Logout(LogOutViewModel model)
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

		/// <summary>
		/// 	Handles the LogOut request from a device.
		/// </summary>
		/// <param name="redirectUrl">
		///		The URL to redirect to after logging out.
		/// </param>
		/// <returns>
		///		The <see cref="IActionResult"/>.
		/// </returns>
		public async Task<IActionResult> DeviceLogOut(string redirectUrl)
		{
			// Delete authentication cookie
			await HttpContext.SignOutAsync();

			// Set this so UI rendering sees an anonymous user
			HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

			return Redirect(redirectUrl);
		}

		/// <summary>
		/// 	Gets the redirecting view.
		/// </summary>
		/// <returns>
		///		The <see cref="IActionResult"/>.
		/// </returns>
		[HttpGet]
		public IActionResult Redirecting() => View();

		#region Helpers

		/// <summary>
		/// 	Builds a clean <see cref="LogInViewModel"/> (for no password)
		/// 	given a dirty <see cref="LogInViewModel"/> (with password).
		/// </summary>
		/// <param name="model">
		///		The dirty <see cref="LogInViewModel"/>.
		/// </param>
		/// <returns>
		///		The clean <see cref="LogInViewModel"/>.
		/// </returns>
		private async Task<LogInViewModel> BuildLoginViewModelAsync(LogInViewModel model)
		{
			AuthorizationRequest context = await _interactionService.GetAuthorizationContextAsync(model.ReturnUrl);
			LogInViewModel vm = new LogInViewModel { ReturnUrl = model.ReturnUrl, Email = context?.LoginHint };
			vm.Email = model.Email;
			vm.RememberMe = model.RememberMe;

			return vm;
		}

		#endregion
	}
}
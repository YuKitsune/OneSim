namespace OneSim.Identity.Web.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Logging;

	using OneSim.Api.Data.Responses;
	using OneSim.Identity.Application;
	using OneSim.Identity.Application.Abstractions;
	using OneSim.Identity.Domain.Entities;
	using OneSim.Identity.Persistence;
	using OneSim.Identity.Web.Models.ViewModels.Account;
	using OneSim.Identity.Web.Models.ViewModels.Authentication;

	using IUrlHelper = OneSim.Identity.Application.Abstractions.IUrlHelper;

	/// <summary>
	/// 	The user account management MVC <see cref="Controller"/>.
	/// </summary>
	[Authorize]
	public class AccountController : Controller, IAccountController
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
		///     The <see cref="ILogger{TCategoryName}"/>.
		/// </summary>
		private readonly ILogger<AccountController> _logger;

		/// <summary>
		/// 	The <see cref="IUrlHelper"/>.
		/// </summary>
		private readonly IUrlHelper _urlHelper;

		/// <summary>
		/// 	The <see cref="IEmailSender"/>.
		/// </summary>
		private readonly IEmailSender _emailSender;

		/// <summary>
		///     Initializes a new instance of the <see cref="AccountController"/> class.
		/// </summary>
		/// <param name="service">
		///     The <see cref="UserService"/>.
		/// </param>
		/// <param name="dbContext">
		///     The <see cref="ApplicationIdentityDbContext"/>.
		/// </param>
		/// <param name="logger">
		///    The <see cref="ILogger{TCategoryName}"/>.
		/// </param>
		/// <param name="urlHelper">
		///    The <see cref="IUrlHelper"/>.
		/// </param>
		/// <param name="emailSender">
		///    The <see cref="IEmailSender"/>.
		/// </param>
		public AccountController(
			UserService service,
			ApplicationIdentityDbContext dbContext,
			ILogger<AccountController> logger,
			IUrlHelper urlHelper,
			IEmailSender emailSender)
		{
			_userService = service;
			_dbContext = dbContext;
			_logger = logger;
			_urlHelper = urlHelper;
			_emailSender = emailSender;
		}

		/// <summary>
		/// 	Returns the index view.
		/// </summary>
		/// <param name="viewModel">
		///		The <see cref="IndexViewModel"/>.
		/// </param>
		/// <returns>
		///		The <see cref="IActionResult"/>.
		/// </returns>
		[HttpGet]
		public IActionResult Index(IndexViewModel viewModel = null) => View(viewModel ?? new IndexViewModel());

		/// <summary>
		/// 	Returns the account registration view.
		/// </summary>
		/// <returns>
		///		The <see cref="IActionResult"/>.
		/// </returns>
		[HttpGet, AllowAnonymous]
		public IActionResult Register() => View();

		/// <summary>
		/// 	Handles the request to register a new account.
		/// </summary>
		/// <param name="viewModel">
		///		The <see cref="RegisterViewModel"/>.
		/// </param>
		/// <returns>
		///		The <see cref="IActionResult"/>.
		/// </returns>
		[HttpPost, AllowAnonymous]
		public async Task<IActionResult> Register(RegisterViewModel viewModel)
		{
			try
			{
				// Check no conflicting users exist
				ApplicationUser existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == viewModel.Email);
				if (existingUser != null)
				{
					ModelState.AddModelError(string.Empty, "The given email address is already registered to an account.");

					return View();
				}

				existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == viewModel.UserName);
				if (existingUser != null)
				{
					ModelState.AddModelError(string.Empty, "The given username is already registered to an account.");

					return View();
				}

				// Create a new user
				ApplicationUser user = new ApplicationUser { Email = viewModel.Email, UserName = viewModel.UserName };

				// Register the user
				await _userService.RegisterUser(user, viewModel.Password);

				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error has occurred registering an account with email \"{viewModel.Email}\" and username \"{viewModel.UserName}\".");
				ModelState.AddModelError(string.Empty, $"An error has occurred registering an account with email \"{viewModel.Email}\" and username \"{viewModel.UserName}\".");

				return View();
			}
		}

		/// <summary>
		/// 	Returns the account deletion view.
		/// </summary>
		/// <returns>
		///		The <see cref="IActionResult"/> containing the <see cref="BaseResponse"/>.
		/// </returns>
		[HttpGet]
		public IActionResult DeleteAccount() => View();

		/// <summary>
		/// 	Handles the request to delete an account.
		/// </summary>
		/// <param name="viewModel">
		///		The <see cref="DeleteAccountViewModel"/>.
		/// </param>
		/// <returns>
		///		The <see cref="IActionResult"/>.
		/// </returns>
		[HttpPost]
		public async Task<IActionResult> DeleteAccount(DeleteAccountViewModel viewModel)
		{
			// Get the current user
			ApplicationUser user = await this.GetCurrentUserAsync(_dbContext);

			try
			{
				// Check the password is correct
				bool passwordMatches = await _userService.UserManager.CheckPasswordAsync(user, viewModel.Password);
				if (passwordMatches)
				{
					await _userService.DeleteUser(user);

					return View("AccountDeleted");
				}

				// Todo: Log security stuff separately, and also keep in mind the amount of failed attempts
				// 	Might pose a security issue
				ModelState.AddModelError(string.Empty, "Unable to delete account.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error has occurred deleting an account with email \"{user.Email}\".");
				ModelState.AddModelError(string.Empty, "An error occurred while attempting to delete the account.");
			}

			return View();
		}

		/// <summary>
		/// 	Handles the request to send an email confirmation email.
		/// </summary>
		/// <returns>
		///		The <see cref="IActionResult"/>.
		/// </returns>
		[HttpGet]
		public async Task<IActionResult> SendEmailConfirmationEmail()
		{
			// Get the current user
			ApplicationUser user = await this.GetCurrentUserAsync(_dbContext);

			try
			{
				// Send the email confirmation email
				await _userService.SendEmailConfirmationEmail(user, _emailSender, HttpContext.Request.Scheme, _urlHelper);

				return RedirectToAction(nameof(Index),
										new IndexViewModel
										{
											Message = $"An Email Confirmation email has been sent to \"{user.Email}\"."
										});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error has occurred sending an Email Confirmation email to user with email \"{user.Email}\".");

				return RedirectToAction(nameof(Index),
										new IndexViewModel
										{
											Message = "An error occurred while attempting to send the Email Confirmation email.",
											MessageIsError = true
										});
			}
		}

		/// <summary>
		/// 	Handles the request to confirm an email address.
		/// </summary>
		/// <param name="confirmationCode">
		///		The confirmation code contained in the email confirmation email.
		/// </param>
		/// <returns>
		///		The <see cref="IActionResult"/>.
		/// </returns>
		[HttpGet]
		public async Task<IActionResult> ConfirmEmail(string confirmationCode)
		{
			// Get the current user
			ApplicationUser user = await this.GetCurrentUserAsync(_dbContext);

			try
			{
				// Confirm email
				await _userService.ConfirmEmail(user, confirmationCode);

				return RedirectToAction(nameof(Index),
										new IndexViewModel
										{
											Message = "Email address confirmed."
										});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error has occurred confirming the email for user with email \"{user.Email}\".");

				return RedirectToAction(nameof(Index),
										new IndexViewModel
										{
											Message = "An error occurred while attempting to confirm the email address.",
											MessageIsError = true
										});
			}
		}

		/// <summary>
		/// 	Returns the password reset view.
		/// </summary>
		/// <returns>
		///		The <see cref="IActionResult"/>.
		/// </returns>
		[HttpGet, AllowAnonymous]
		public IActionResult SendPasswordResetEmail() => View();

		/// <summary>
		/// 	Handles the request to send a password reset email.
		/// </summary>
		/// <param name="viewModel">
		///		The <see cref="SendPasswordResetEmailViewModel"/>.
		/// </param>
		/// <returns>
		///		The <see cref="IActionResult"/> containing the <see cref="BaseResponse"/>.
		/// </returns>
		[HttpPost, AllowAnonymous]
		public async Task<IActionResult> SendPasswordResetEmail(SendPasswordResetEmailViewModel viewModel)
		{
			// Get the user
			ApplicationUser user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == viewModel.Email);

			if (user != null)
			{
				try
				{
					// Send the email
					await _userService.SendPasswordResetEmail(user, _emailSender, HttpContext.Request.Scheme, _urlHelper);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, $"An error has occurred sending a Password Reset email to user with email \"{user.Email}\".");
					ModelState.AddModelError(string.Empty, "An error occurred while sending the Password Reset email.");

					return View();
				}
			}
			else
			{
				_logger.LogWarning($"Password reset requested for user with email \"{viewModel.Email}\", but no user with that email exists.");
			}

			return View("PasswordResetEmailSent");
		}

		/// <summary>
		/// 	Returns the password reset view.
		/// </summary>
		/// <returns>
		///		The <see cref="IActionResult"/>.
		/// </returns>
		[HttpGet, AllowAnonymous]
		public IActionResult ResetPassword(string token) => View(new ResetPasswordViewModel { ResetToken = token });

		/// <summary>
		/// 	Handles the request to reset the users password.
		/// </summary>
		/// <param name="viewModel">
		///		The <see cref="ResetPasswordViewModel"/>.
		/// </param>
		/// <returns>
		///		The <see cref="IActionResult"/>.
		/// </returns>
		[HttpPost, AllowAnonymous]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel viewModel)
		{
			// Get the user
			ApplicationUser user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == viewModel.Email);

			try
			{
				// Reset the password
				await _userService.ResetPassword(user, viewModel.NewPassword, viewModel.ResetToken);

				return RedirectToAction(nameof(AuthenticationController.Login),
										Utils.GetControllerName(nameof(AuthenticationController)),
										new LoginViewModel { Message = "Password has been reset." });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error has occurred resetting the password for user with email \"{user.Email}\".");
				ModelState.AddModelError(string.Empty, "An error occurred while attempting to reset the password.");

				return View(new ResetPasswordViewModel
							{
								Email = viewModel.Email,
								ResetToken = viewModel.ResetToken
							});
			}
		}

		/// <summary>
		/// 	Returns the change password view.
		/// </summary>
		/// <returns>
		///		The <see cref="IActionResult"/>.
		/// </returns>
		[HttpGet]
		public IActionResult ChangePassword() => View();

		/// <summary>
		/// 	Handles the request to change the current users password.
		/// </summary>
		/// <param name="viewModel">
		///		The <see cref="ChangePasswordViewModel"/>.
		/// </param>
		/// <returns>
		///		The <see cref="IActionResult"/>.
		/// </returns>
		[HttpPost]
		public async Task<IActionResult> ChangePassword(ChangePasswordViewModel viewModel)
		{
			// Get the current user
			ApplicationUser user = await this.GetCurrentUserAsync(_dbContext);

			try
			{
				// Change the password
				await _userService.ChangePassword(user, viewModel.OldPassword, viewModel.NewPassword);

				return RedirectToAction(nameof(Index),
										new IndexViewModel
										{
											Message = "Password has been changed."
										});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error has occurred changing the password for user with email \"{user.Email}\".");

				return RedirectToAction(nameof(Index),
										new IndexViewModel
										{
											Message = "An error occurred while attempting to change password.",
											MessageIsError = true
										});
			}
		}

		/// <summary>
		/// 	Returns the enable Two-Factor Authentication view.
		/// </summary>
		/// <returns>
		///		The <see cref="IActionResult"/>.
		/// </returns>
		[HttpGet]
		public async Task<IActionResult> EnableTwoFactorAuthentication()
		{
			// Get the current user
			ApplicationUser user = await this.GetCurrentUserAsync(_dbContext);

			// Check if 2FA is enabled
			if (user.TwoFactorEnabled)
			{
				return RedirectToAction(nameof(Index),
										new IndexViewModel
										{
											Message = "Two-Factor Authentication is already enabled."
										});
			}

			try
			{
				// Get the key and URI
				(string key, string qrCodeUri) = await _userService.GetSharedKeyAndQrCodeUriAsync(user);

				return View(new EnableTwoFactorAuthenticationViewModel
							{
								SharedKey = key,
								AuthenticatorUri = qrCodeUri
							});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error has occurred enabling Two-Factor Authentication for user with email \"{user.Email}\".");

				return RedirectToAction(nameof(Index),
										new IndexViewModel
										{
											Message = "An error occurred while attempting to enable Two-Factor Authentication.",
											MessageIsError = true
										});
			}
		}

		/// <summary>
		/// 	Handles the request to enable Two-Factor Authentication.
		/// </summary>
		/// <param name="viewModel">
		///		The <see cref="EnableTwoFactorAuthenticationViewModel"/>.
		/// </param>
		/// <returns>
		///		The <see cref="IActionResult"/>.
		/// </returns>
		[HttpPost]
		public async Task<IActionResult> EnableTwoFactorAuthentication(
			EnableTwoFactorAuthenticationViewModel viewModel)
		{
			// Get the current user
			ApplicationUser user = await this.GetCurrentUserAsync(_dbContext);

			try
			{
				// Enable 2FA
				IEnumerable<string> recoveryCodes = await _userService.EnableTwoFactorAuthentication(user, viewModel.VerificationCode);

				return View("RecoveryCodes", new RecoveryCodesViewModel { RecoveryCodes = recoveryCodes });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error has occurred enabling Two-Factor Authentication for user with email \"{user.Email}\".");
				ModelState.AddModelError(string.Empty, "An error occurred while attempting to enable Two-Factor Authentication.");

				return View(new EnableTwoFactorAuthenticationViewModel
							{
								SharedKey = viewModel.SharedKey,
								AuthenticatorUri = viewModel.AuthenticatorUri
							});
			}
		}

		/// <summary>
		/// 	Returns the Two-Factor Authentication reset warning view.
		/// </summary>
		/// <returns>
		///		The <see cref="IActionResult"/>.
		/// </returns>
		[HttpGet]
		public IActionResult ResetTwoFactorAuthenticationWarning() => View();

		/// <summary>
		/// 	Handles the request to reset Two-Factor Authentication.
		/// </summary>
		/// <returns>
		///		The <see cref="IActionResult"/>.
		/// </returns>
		[HttpGet]
		public async Task<IActionResult> ResetTwoFactorAuthentication()
		{
			// Get the current user
			ApplicationUser user = await this.GetCurrentUserAsync(_dbContext);

			try
			{
				// Reset 2FA
				await _userService.ResetTwoFactorAuthentication(user);

				// Redirect to Enable 2FA
				return RedirectToAction(nameof(EnableTwoFactorAuthentication));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error has occurred resetting Two-Factor Authentication for user with email \"{user.Email}\".");
				ModelState.AddModelError(string.Empty, "An error occurred while attempting to reset Two-Factor Authentication.");

				return RedirectToAction(nameof(ResetTwoFactorAuthenticationWarning));
			}
		}

		/// <summary>
		/// 	Returns the Two-Factor Authentication disable warning view.
		/// </summary>
		/// <returns>
		///		The <see cref="IActionResult"/>.
		/// </returns>
		[HttpGet]
		public IActionResult DisableTwoFactorAuthenticationWarning() => View();

		/// <summary>
		/// 	Handles the request to disable Two-Factor Authentication.
		/// </summary>
		/// <returns>
		///		The <see cref="IActionResult"/>.
		/// </returns>
		[HttpGet]
		public async Task<IActionResult> DisableTwoFactorAuthentication()
		{
			// Get the current user
			ApplicationUser user = await this.GetCurrentUserAsync(_dbContext);

			try
			{
				// Disable 2FA
				await _userService.DisableTwoFactorAuthentication(user);

				return RedirectToAction(nameof(Index),
										new IndexViewModel
										{
											Message = "Two-Factor Authentication disabled."
										});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error has occurred disabling Two-Factor Authentication for user with email \"{user.Email}\".");
				ModelState.AddModelError(string.Empty, "An error occurred while attempting to disable Two-Factor Authentication.");

				return RedirectToAction(nameof(DisableTwoFactorAuthenticationWarning));
			}
		}
	}
}
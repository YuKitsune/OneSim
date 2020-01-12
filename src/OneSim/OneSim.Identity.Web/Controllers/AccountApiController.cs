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

	using IUrlHelper = OneSim.Identity.Application.Abstractions.IUrlHelper;

	/// <summary>
	/// 	The user account management API <see cref="Controller"/>.
	/// </summary>
	[Authorize]
	public class AccountApiController : Controller, IAccountController
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
		private readonly ILogger<AccountApiController> _logger;

		/// <summary>
		/// 	The <see cref="IUrlHelper"/>.
		/// </summary>
		private readonly IUrlHelper _urlHelper;

		/// <summary>
		/// 	The <see cref="IEmailSender"/>.
		/// </summary>
		private readonly IEmailSender _emailSender;

		/// <summary>
		///     Initializes a new instance of the <see cref="AccountApiController"/> class.
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
		public AccountApiController(
			UserService service,
			ApplicationIdentityDbContext dbContext,
			ILogger<AccountApiController> logger,
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
		/// 	Handles the request to register a new account.
		/// </summary>
		/// <param name="viewModel">
		///		The <see cref="RegisterViewModel"/>.
		/// </param>
		/// <returns>
		///		The <see cref="IActionResult"/> containing the <see cref="BaseResponse"/>.
		/// </returns>
		[HttpPost, AllowAnonymous]
		public async Task<IActionResult> Register([FromBody] RegisterViewModel viewModel)
		{
			try
			{
				// Check no conflicting users exist
				ApplicationUser existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == viewModel.Email);
				if (existingUser != null)
				{
					return Json(new BaseResponse(ResponseStatus.Failure,
												 "The given email address is already registered to an account."));
				}

				existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == viewModel.UserName);
				if (existingUser != null)
				{
					return Json(new BaseResponse(ResponseStatus.Failure,
												 "The given username is already registered to an account."));
				}

				// Create a new user
				ApplicationUser user = new ApplicationUser { Email = viewModel.Email, UserName = viewModel.UserName };

				// Register the user
				await _userService.RegisterUser(user, viewModel.Password);

				return Json(new BaseResponse(ResponseStatus.Success, $"Account with email \"{user.Email}\" and username \"{user.UserName}\" successfully registered."));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error has occurred registering an account with email \"{viewModel.Email}\" and username \"{viewModel.UserName}\".");

				return Json(new BaseResponse(ResponseStatus.Error, "An error occurred while attempting to register the account."));
			}
		}

		/// <summary>
		/// 	Handles the request to delete an account.
		/// </summary>
		/// <param name="viewModel">
		///		The <see cref="DeleteAccountViewModel"/>.
		/// </param>
		/// <returns>
		///		The <see cref="IActionResult"/> containing the <see cref="BaseResponse"/>.
		/// </returns>
		[HttpPost]
		public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountViewModel viewModel)
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

					return Json(new BaseResponse(ResponseStatus.Success, "Account has been deleted."));
				}

				// Todo: Log security stuff separately, and also keep in mind the amount of failed attempts
				// 	Might pose a security issue
				return Json(new BaseResponse(ResponseStatus.Unauthorized, "Unable to delete account."));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error has occurred deleting an account with email \"{user.Email}\".");

				return Json(new BaseResponse(ResponseStatus.Error, "An error occurred while attempting to delete the account."));
			}
		}

		/// <summary>
		/// 	Handles the request to send an email confirmation email.
		/// </summary>
		/// <returns>
		///		The <see cref="IActionResult"/> containing the <see cref="BaseResponse"/>.
		/// </returns>
		[HttpPost]
		public async Task<IActionResult> SendEmailConfirmationEmail()
		{
			// Get the current user
			ApplicationUser user = await this.GetCurrentUserAsync(_dbContext);

			try
			{
				// Send the email confirmation email
				await _userService.SendEmailConfirmationEmail(user, _emailSender, HttpContext.Request.Scheme, _urlHelper);

				return Json(new BaseResponse(ResponseStatus.Success, "Email Confirmation email sent."));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error has occurred sending an Email Confirmation email to user with email \"{user.Email}\".");

				return Json(new BaseResponse(ResponseStatus.Error, "An error occurred while attempting to send the Email Confirmation email."));
			}
		}

		/// <summary>
		/// 	Handles the request to confirm an email address.
		/// </summary>
		/// <param name="confirmationCode">
		///		The confirmation code contained in the email confirmation email.
		/// </param>
		/// <returns>
		///		The <see cref="IActionResult"/> containing the <see cref="BaseResponse"/>.
		/// </returns>
		[HttpPost]
		public async Task<IActionResult> ConfirmEmail([FromBody] string confirmationCode)
		{
			// Get the current user
			ApplicationUser user = await this.GetCurrentUserAsync(_dbContext);

			try
			{
				// Confirm email
				await _userService.ConfirmEmail(user, confirmationCode);

				return Json(new BaseResponse(ResponseStatus.Success, "Email address confirmed."));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error has occurred confirming the email for user with email \"{user.Email}\".");

				return Json(new BaseResponse(ResponseStatus.Error, "An error occurred while attempting to confirm the email address."));
			}
		}

		/// <summary>
		/// 	Handles the request to send a password reset email.
		/// </summary>
		/// <param name="viewModel">
		///		The <see cref="ForgotPasswordViewModel"/>.
		/// </param>
		/// <returns>
		///		The <see cref="IActionResult"/> containing the <see cref="BaseResponse"/>.
		/// </returns>
		[HttpPost, AllowAnonymous]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel viewModel)
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

					return Json(new BaseResponse(ResponseStatus.Error, "An error occurred while sending the Password Reset email."));
				}
			}
			else
			{
				_logger.LogWarning($"Password reset requested for user with email \"{viewModel.Email}\", but no user with that email exists.");
			}

			return Json(new BaseResponse(ResponseStatus.Success, "Password Reset email sent."));
		}

		/// <summary>
		/// 	Handles the request to reset the users password.
		/// </summary>
		/// <param name="viewModel">
		///		The <see cref="ResetPasswordViewModel"/>.
		/// </param>
		/// <returns>
		///		The <see cref="IActionResult"/> containing the <see cref="BaseResponse"/>.
		/// </returns>
		[HttpPost, AllowAnonymous]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel viewModel)
		{
			// Get the user
			ApplicationUser user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == viewModel.Email);

			try
			{
				// Reset the password
				await _userService.ResetPassword(user, viewModel.NewPassword, viewModel.ResetToken);

				return Json(new BaseResponse(ResponseStatus.Success, "Password reset."));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error has occurred resetting the password for user with email \"{user.Email}\".");

				return Json(new BaseResponse(ResponseStatus.Error, "An error occurred while attempting to reset the password."));
			}
		}

		/// <summary>
		/// 	Handles the request to change the current users password.
		/// </summary>
		/// <param name="viewModel">
		///		The <see cref="ChangePasswordViewModel"/>.
		/// </param>
		/// <returns>
		///		The <see cref="IActionResult"/> containing the <see cref="BaseResponse"/>.
		/// </returns>
		[HttpPost]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel viewModel)
		{
			// Get the current user
			ApplicationUser user = await this.GetCurrentUserAsync(_dbContext);

			try
			{
				// Change the password
				await _userService.ChangePassword(user, viewModel.OldPassword, viewModel.NewPassword);

				return Json(new BaseResponse(ResponseStatus.Success, "Password changed."));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error has occurred changing the password for user with email \"{user.Email}\".");

				return Json(new BaseResponse(ResponseStatus.Error, "An error occurred while attempting to change password."));
			}
		}

		/// <summary>
		/// 	Handles the request for the key and URI required to enable Two-Factor Authentication.
		/// </summary>
		/// <returns>
		///		The <see cref="IActionResult"/> containing the <see cref="EnableTwoFactorAuthenticationResponse"/>, or
		/// 	<see cref="BaseResponse"/> in the event of an error.
		/// </returns>
		[HttpPost]
		public async Task<IActionResult> EnableTwoFactorAuthentication()
		{
			// Get the current user
			ApplicationUser user = await this.GetCurrentUserAsync(_dbContext);

			// Check if 2FA is enabled
			if (user.TwoFactorEnabled)
				return Json(new BaseResponse(ResponseStatus.Invariant, "Two-Factor Authentication is already enabled."));

			try
			{
				// Get the key and URI
				(string key, string qrCodeUri) = await _userService.GetSharedKeyAndQrCodeUriAsync(user);

				return Json(new EnableTwoFactorAuthenticationResponse(ResponseStatus.Invariant, "Ready to enable Two-Factor Authentication")
							{
								SharedKey = key,
								AuthenticatorUri = qrCodeUri
							});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error has occurred enabling Two-Factor Authentication for user with email \"{user.Email}\".");

				return Json(new BaseResponse(ResponseStatus.Error, "An error occurred while attempting to enable Two-Factor Authentication."));
			}
		}

		/// <summary>
		/// 	Handles the request to enable Two-Factor Authentication.
		/// </summary>
		/// <param name="viewModel">
		///		The <see cref="EnableTwoFactorAuthenticationViewModel"/>.
		/// </param>
		/// <returns>
		///		The <see cref="IActionResult"/> containing the <see cref="RecoveryCodeResponse"/>, or
		/// 	<see cref="BaseResponse"/> in the event of an error.
		/// </returns>
		[HttpPost]
		public async Task<IActionResult> EnableTwoFactorAuthentication(
			[FromBody] EnableTwoFactorAuthenticationViewModel viewModel)
		{
			// Get the current user
			ApplicationUser user = await this.GetCurrentUserAsync(_dbContext);

			try
			{
				// Enable 2FA
				IEnumerable<string> recoveryCodes = await _userService.EnableTwoFactorAuthentication(user, viewModel.VerificationCode);

				return Json(new RecoveryCodeResponse(ResponseStatus.Success, "Two-Factor Authentication Enabled")
							{
								RecoveryCodes = recoveryCodes
							});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error has occurred enabling Two-Factor Authentication for user with email \"{user.Email}\".");

				return Json(new BaseResponse(ResponseStatus.Error, "An error occurred while attempting to enable Two-Factor Authentication."));
			}
		}

		/// <summary>
		/// 	Handles the request to reset Two-Factor Authentication.
		/// </summary>
		/// <returns>
		///		The <see cref="IActionResult"/> containing the <see cref="BaseResponse"/>.
		/// </returns>
		[HttpPost]
		public async Task<IActionResult> ResetTwoFactorAuthentication()
		{
			// Get the current user
			ApplicationUser user = await this.GetCurrentUserAsync(_dbContext);

			try
			{
				// Reset 2FA
				await _userService.ResetTwoFactorAuthentication(user);

				// Revert to whatever the Enable method does
				return await EnableTwoFactorAuthentication();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error has occurred resetting Two-Factor Authentication for user with email \"{user.Email}\".");

				return Json(new BaseResponse(ResponseStatus.Error, "An error occurred while attempting to reset Two-Factor Authentication."));
			}
		}

		/// <summary>
		/// 	Handles the request to disable Two-Factor Authentication.
		/// </summary>
		/// <returns>
		///		The <see cref="IActionResult"/> containing the <see cref="BaseResponse"/>.
		/// </returns>
		[HttpPost]
		public async Task<IActionResult> DisableTwoFactorAuthentication()
		{
			// Get the current user
			ApplicationUser user = await this.GetCurrentUserAsync(_dbContext);

			try
			{
				// Disable 2FA
				await _userService.DisableTwoFactorAuthentication(user);

				return Json(new BaseResponse(ResponseStatus.Success, "Two-Factor Authentication disabled."));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error has occurred disabling Two-Factor Authentication for user with email \"{user.Email}\".");

				return Json(new BaseResponse(ResponseStatus.Error, "An error occurred while attempting to disable Two-Factor Authentication."));
			}
		}
	}
}
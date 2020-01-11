namespace OneSim.Identity.Web.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Logging;

	using OneSim.Api.Data.Requests;
	using OneSim.Api.Data.Responses;
	using OneSim.Identity.Application;
	using OneSim.Identity.Application.Abstractions;
	using OneSim.Identity.Domain.Entities;
	using OneSim.Identity.Persistence;

	using IUrlHelper = OneSim.Identity.Application.Abstractions.IUrlHelper;

	/// <summary>
	/// 	The user account management <see cref="Controller"/>.
	/// </summary>
	[Authorize]
	public class AccountController : Controller
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
		/// 	Attempts to register a new user.
		/// </summary>
		/// <param name="request">
		///		The <see cref="RegisterRequest"/>.
		/// </param>
		/// <returns>
		///		The <see cref="ActionResult"/> containing the <see cref="BaseResponse"/>.
		/// </returns>
		[HttpPost]
		public async Task<ActionResult> Register([FromBody] RegisterRequest request)
		{
			try
			{
				// Check no conflicting users exist
				ApplicationUser existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

				if (existingUser != null)
				{
					return Json(new BaseResponse(ResponseStatus.Failure,
												 "The given email address is already registered to an account."));
				}

				existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);

				if (existingUser != null)
				{
					return Json(new BaseResponse(ResponseStatus.Failure,
												 "The given username is already registered to an account."));
				}

				// Create a new user
				ApplicationUser user = new ApplicationUser { Email = request.Email, UserName = request.UserName };

				// Register the user
				await _userService.RegisterUser(user, request.Password);

				return Json(new BaseResponse(ResponseStatus.Success, $"Successfully registered a user with username {request.UserName}."));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error has occurred registering a user with username \"{request.UserName}\".");

				return Json(new BaseResponse(ResponseStatus.Error, "An error has occurred processing the Register request."));
			}
		}

		/// <summary>
		/// 	Attempts to create a new user.
		/// </summary>
		/// <param name="request">
		///		The <see cref="CreateUserRequest"/>.
		/// </param>
		/// <returns>
		///		The <see cref="ActionResult"/> containing the <see cref="BaseResponse"/>.
		/// </returns>
		[HttpPost, Authorize]
		public async Task<ActionResult> CreateUser([FromBody] CreateUserRequest request)
		{
			// Get the current user
			ApplicationUser currentUser = await this.GetCurrentUserAsync(_dbContext);

			try
			{
				// Ensure the current user is an admin
				if (currentUser.Type != UserType.Administrator)
				{
					return Json(new BaseResponse(ResponseStatus.Unauthorized,
												 "Only administrators may create new users"));
				}

				// Check no conflicting users exist
				ApplicationUser existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

				if (existingUser != null)
				{
					return Json(new BaseResponse(ResponseStatus.Failure,
												 "The given email address is already registered to an account."));
				}

				existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);

				if (existingUser != null)
				{
					return Json(new BaseResponse(ResponseStatus.Failure,
												 "The given username is already registered to an account."));
				}

				// Create a new user
				ApplicationUser user = new ApplicationUser { Email = request.Email, UserName = request.UserName };

				// Register the user
				await _userService.CreateUser(user, _emailSender, HttpContext.Request.Scheme, _urlHelper);

				return Json(new BaseResponse(ResponseStatus.Success, $"Successfully created a user with username {request.UserName}."));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error has occurred creating a user with username \"{request.UserName}\".");

				return Json(new BaseResponse(ResponseStatus.Error, "An error has occurred processing the Create User request."));
			}
		}

		/// <summary>
		/// 	Attempts to delete the current user.
		/// </summary>
		/// <param name="request">
		///		The <see cref="DeleteUserRequest"/>.
		/// </param>
		/// <returns>
		///		The <see cref="ActionResult"/> containing the <see cref="BaseResponse"/>.
		/// </returns>
		[HttpPost]
		public async Task<ActionResult> DeleteUser([FromBody] DeleteUserRequest request)
		{
			// Get the current user
			ApplicationUser user = await this.GetCurrentUserAsync(_dbContext);

			try
			{
				// Check the password is correct
				bool passwordMatches = await _userService.UserManager.CheckPasswordAsync(user, request.Password);
				if (passwordMatches)
				{
					await _userService.DeleteUser(user);

					return Json(new BaseResponse(ResponseStatus.Success, "User has been deleted."));
				}

				// Todo: Log security stuff separately, and also keep in mind the amount of failed attempts
				// 	Might pose a security issue
				return Json(new BaseResponse(ResponseStatus.Unauthorized, "Incorrect password."));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error has occurred deleting a user with username \"{request.UserName}\".");

				return Json(new BaseResponse(ResponseStatus.Error, "An error has occurred processing the Delete User request."));
			}
		}

		/// <summary>
		/// 	Attempts to send an Email Confirmation email to the currently logged in user.
		/// </summary>
		/// <returns>
		///		The <see cref="ActionResult"/> containing the <see cref="BaseResponse"/>.
		/// </returns>
		[HttpPost]
		public async Task<ActionResult> SendEmailConfirmationEmail()
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

				return Json(new BaseResponse(ResponseStatus.Error, "An error has occurred processing the Send Email Confirmation Email request."));
			}
		}

		/// <summary>
		/// 	Attempts to confirm the email address of the currently logged in user using the
		/// 	<see cref="ConfirmEmailRequest.ConfirmationCode"/>.
		/// </summary>
		/// <param name="request">
		///		The <see cref="ConfirmEmailRequest"/>.
		/// </param>
		/// <returns>
		///		The <see cref="ActionResult"/> containing the <see cref="BaseResponse"/>.
		/// </returns>
		[HttpPost]
		public async Task<ActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
		{
			// Get the current user
			ApplicationUser user = await this.GetCurrentUserAsync(_dbContext);

			try
			{
				// Confirm email
				await _userService.ConfirmEmail(user, request.ConfirmationCode);

				return Json(new BaseResponse(ResponseStatus.Success, "Email confirmed."));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error has occurred confirming the email for user with email \"{user.Email}\".");

				return Json(new BaseResponse(ResponseStatus.Error, "An error has occurred processing the Confirm Email request."));
			}
		}

		/// <summary>
		/// 	Attempts to send a password reset email to the currently logged in user.
		/// </summary>
		/// <returns>
		///		The <see cref="ActionResult"/> containing the <see cref="BaseResponse"/>.
		/// </returns>
		[HttpPost]
		public async Task<ActionResult> SendPasswordResetEmail()
		{
			// Get the current user
			ApplicationUser user = await this.GetCurrentUserAsync(_dbContext);

			try
			{
				// Send the email
				await _userService.SendPasswordResetEmail(user, _emailSender, HttpContext.Request.Scheme, _urlHelper);

				return Json(new BaseResponse(ResponseStatus.Success, "Password Reset email sent."));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error has occurred sending a Password Reset email to user with email \"{user.Email}\".");

				return Json(new BaseResponse(ResponseStatus.Error, "An error has occurred processing the Send Password Reset Email request."));
			}
		}

		/// <summary>
		/// 	Resets the password for the currently logged in user using the
		/// 	<see cref="ResetPasswordRequest.ResetToken"/>.
		/// </summary>
		/// <param name="request">
		///		The <see cref="ResetPasswordRequest"/>.
		/// </param>
		/// <returns>
		///		The <see cref="ActionResult"/> containing the <see cref="BaseResponse"/>.
		/// </returns>
		[HttpPost]
		public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
		{
			// Get the current user
			ApplicationUser user = await this.GetCurrentUserAsync(_dbContext);

			try
			{
				// Reset the password
				await _userService.ResetPassword(user, request.NewPassword, request.ResetToken);

				return Json(new BaseResponse(ResponseStatus.Success, "Password reset."));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error has occurred resetting the password for user with email \"{user.Email}\".");

				return Json(new BaseResponse(ResponseStatus.Error, "An error has occurred processing the Reset Password request."));
			}
		}

		/// <summary>
		/// 	Attempts to change the password for the currently logged in user.
		/// </summary>
		/// <param name="request">
		///		The <see cref="ChangePasswordRequest"/>.
		/// </param>
		/// <returns>
		///		The <see cref="ActionResult"/> containing the <see cref="BaseResponse"/>.
		/// </returns>
		[HttpPost]
		public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
		{
			// Get the current user
			ApplicationUser user = await this.GetCurrentUserAsync(_dbContext);

			try
			{
				// Change the password
				await _userService.ChangePassword(user, request.OldPassword, request.NewPassword);

				return Json(new BaseResponse(ResponseStatus.Success, "Password changed."));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error has occurred changing the password for user with email \"{user.Email}\".");

				return Json(new BaseResponse(ResponseStatus.Error, "An error has occurred processing the Change Password request."));
			}
		}

		/// <summary>
		/// 	Attempts to enable Two-Factor Authentication for the currently logged in user.
		/// </summary>
		/// <param name="request">
		///		The <see cref="EnableTwoFactorAuthenticationRequest"/>.
		/// </param>
		/// <returns>
		///		The <see cref="ActionResult"/> containing the <see cref="EnableTwoFactorAuthenticationResponse"/>, or
		/// 	<see cref="BaseResponse"/> in the event of an error.
		/// </returns>
		[HttpPost]
		public async Task<ActionResult> EnableTwoFactorAuthentication(
			[FromBody] EnableTwoFactorAuthenticationRequest request)
		{
			// Get the current user
			ApplicationUser user = await this.GetCurrentUserAsync(_dbContext);

			try
			{
				// Enable 2FA
				IEnumerable<string> recoveryCodes = await _userService.EnableTwoFactorAuthentication(user, request.VerificationCode);

				return Json(new EnableTwoFactorAuthenticationResponse(ResponseStatus.Success, "Two-Factor Authentication Enabled")
							{
								RecoveryCodes = recoveryCodes
							});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error has occurred enabling Two-Factor Authentication for user with email \"{user.Email}\".");

				return Json(new BaseResponse(ResponseStatus.Error, "An error has occurred processing the Enable Two-Factor Authentication request."));
			}
		}

		/// <summary>
		/// 	Attempts to reset Two-Factor Authentication for the currently logged in user.
		/// </summary>
		/// <returns>
		///		The <see cref="ActionResult"/> containing the <see cref="BaseResponse"/>.
		/// </returns>
		[HttpPost]
		public async Task<ActionResult> ResetTwoFactorAuthentication()
		{
			// Get the current user
			ApplicationUser user = await this.GetCurrentUserAsync(_dbContext);

			try
			{
				throw new NotImplementedException("Need to investigate reset vs Enable / Disable.");

				// Reset 2FA
				// Todo: Shouldn't i be getting codes here?
				await _userService.ResetTwoFactorAuthentication(user);

				return Json(new BaseResponse(ResponseStatus.Success, "Two-Factor Authentication reset."));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error has occurred resetting Two-Factor Authentication for user with email \"{user.Email}\".");

				return Json(new BaseResponse(ResponseStatus.Error, "An error has occurred processing the Reset Two-Factor Authentication request."));
			}
		}

		/// <summary>
		/// 	Attempts to disable Two-Factor Authentication for the currently logged in user.
		/// </summary>
		/// <returns>
		///		The <see cref="ActionResult"/> containing the <see cref="BaseResponse"/>.
		/// </returns>
		[HttpPost]
		public async Task<ActionResult> DisableTwoFactorAuthentication()
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

				return Json(new BaseResponse(ResponseStatus.Error, "An error has occurred processing the Disable Two-Factor Authentication request."));
			}
		}
	}
}
namespace OneSim.Api.Identity.Controllers
{
	using System;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Logging;

	using OneSim.Api.Data.Requests;
	using OneSim.Api.Data.Responses;
	using OneSim.Api.Identity.Data;
	using OneSim.Identity.Application;
	using OneSim.Identity.Application.Interfaces;
	using OneSim.Identity.Domain.Entities;

	using IUrlHelper = OneSim.Identity.Application.Interfaces.IUrlHelper;

	/// <summary>
	/// 	The Users <see cref="Controller"/>.
	/// </summary>
	[Authorize]
	public class UsersController : Controller
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
		private readonly ILogger<UsersController> _logger;

		/// <summary>
		/// 	The <see cref="IUrlHelper"/>.
		/// </summary>
		private readonly IUrlHelper _urlHelper;

		/// <summary>
		/// 	The <see cref="IEmailSender"/>.
		/// </summary>
		private readonly IEmailSender _emailSender;

		// Todo: Provide more feedback via the API.
		// Todo: Make the request / current user / admin checks more common

		/// <summary>
		///     Initializes a new instance of the <see cref="UsersController"/> class.
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
		public UsersController(
			UserService service,
			ApplicationIdentityDbContext dbContext,
			ILogger<UsersController> logger,
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
		/// 	Creates a new user.
		/// </summary>
		/// <param name="request">
		///		The <see cref="CreateUserRequest"/>.
		/// </param>
		/// <returns>
		///		The <see cref="ActionResult"/>.
		/// </returns>
		public async Task<ActionResult> CreateUser([FromBody] CreateUserRequest request)
		{
			try
			{
				// Check no conflicting users exist
				ApplicationUser existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
				if (existingUser != null)
				{
					return new JsonResult(new CreateUserResponse
										  {
											  UserCreated = false,
											  Message = "The given email address is already registered to an account."
										  });
				}

				existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);
				if (existingUser != null)
				{
					return new JsonResult(new CreateUserResponse
										  {
											  UserCreated = false,
											  Message = "The given username is already registered to an account."
										  });
				}

				// Create a new user
				ApplicationUser user = new ApplicationUser { Email = request.Email, UserName = request.UserName };

				// Register the user
				await _userService.CreateUser(user, request.Password, _urlHelper, "https", _emailSender);

				return new JsonResult(new CreateUserResponse
									  {
										  UserCreated = true
									  });
			}
			catch (Exception ex)
			{
				_logger.LogCritical($"Failed to create user. Exception:{Environment.NewLine}{ex}");

				return StatusCode(500, "An error has occurred processing the Create User request.");
			}
		}

		/// <summary>
		/// 	Deletes a user.
		/// </summary>
		/// <param name="request">
		///		The <see cref="DeleteUserRequest"/>.
		/// </param>
		/// <returns>
		///		The <see cref="ActionResult"/>.
		/// </returns>
		public async Task<ActionResult> DeleteUser([FromBody] DeleteUserRequest request)
		{
			try
			{
				// Find the user
				ApplicationUser user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);

				if (user == null) throw new Exception($"Unable to find user with username \"{request.UserName}\".");

				// Check the password is correct
				bool passwordMatches = await _userService.UserManager.CheckPasswordAsync(user, request.Password);
				if (passwordMatches)
				{
					await _userService.DeleteUser(user);

					return Ok();
				}
				else
				{
					// Todo: Log security stuff separately, and also keep in mind the amount of failed attempts
					// 	Might pose a security issue
					return Unauthorized();
				}
			}
			catch (Exception ex)
			{
				_logger.LogCritical($"Failed to delete user. Exception:{Environment.NewLine}{ex}");

				return StatusCode(500, "An error has occurred processing the Delete User request.");
			}
		}

		/// <summary>
		/// 	Sends a password reset email.
		/// </summary>
		/// <param name="request">
		///		The <see cref="SendPasswordResetEmailRequest"/>.
		/// </param>
		/// <returns>
		///		The <see cref="ActionResult"/>.
		/// </returns>
		public async Task<ActionResult> SendPasswordResetEmail([FromBody] SendPasswordResetEmailRequest request)
		{
			try
			{
				// Find the user
				ApplicationUser user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

				// Compare to the logged in user
				ApplicationUser currentUser = await this.GetCurrentUserAsync(_dbContext);

				// Todo: Allow admins to override this
				if (currentUser.Email != user.Email) return Unauthorized();

				// Send the email
				await _userService.SendPasswordResetEmail(_urlHelper, "https", _emailSender, user);

				return Ok();
			}
			catch (Exception ex)
			{
				_logger.LogCritical($"Failed to send password reset email. Exception:{Environment.NewLine}{ex}");

				return StatusCode(500, "An error has occurred processing the Send Password Reset Email request.");
			}
		}

		/// <summary>
		/// 	Resets the password.
		/// </summary>
		/// <param name="request">
		///		The <see cref="ResetPasswordRequest"/>.
		/// </param>
		/// <returns>
		///		The <see cref="ActionResult"/>.
		/// </returns>
		public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
		{
			try
			{
				// Find the user
				ApplicationUser user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

				// Compare to the logged in user
				ApplicationUser currentUser = await this.GetCurrentUserAsync(_dbContext);

				// Todo: Allow admins to override this
				if (currentUser.Email != user.Email) return Unauthorized();

				// Reset the password
				await _userService.ResetPassword(user, request.NewPassword, request.ResetToken);

				return Ok();
			}
			catch (Exception ex)
			{
				_logger.LogCritical($"Failed to reset password. Exception:{Environment.NewLine}{ex}");

				return StatusCode(500, "An error has occurred processing the Reset Password request.");
			}
		}

		/// <summary>
		/// 	Changes the password.
		/// </summary>
		/// <param name="request">
		///		The <see cref="ChangePasswordRequest"/>.
		/// </param>
		/// <returns>
		///		The <see cref="ActionResult"/>.
		/// </returns>
		public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
		{
			try
			{
				// Find the user
				ApplicationUser user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

				// Compare to the logged in user
				ApplicationUser currentUser = await this.GetCurrentUserAsync(_dbContext);

				// Todo: Allow admins to override this
				if (currentUser.Email != user.Email) return Unauthorized();

				// Change the password
				await _userService.ChangePassword(user, request.OldPassword, request.NewPassword);

				return Ok();
			}
			catch (Exception ex)
			{
				_logger.LogCritical($"Failed to change password. Exception:{Environment.NewLine}{ex}");

				return StatusCode(500, "An error has occurred processing the Change Password request.");
			}
		}

		/// <summary>
		/// 	Sends an email confirmation email.
		/// </summary>
		/// <param name="request">
		///		The <see cref="SendEmailConfirmationEmailRequest"/>.
		/// </param>
		/// <returns>
		///		The <see cref="ActionResult"/>.
		/// </returns>
		public async Task<ActionResult> SendEmailConfirmationEmail([FromBody] SendEmailConfirmationEmailRequest request)
		{
			try
			{
				// Find the user
				ApplicationUser user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

				// Compare to the logged in user
				ApplicationUser currentUser = await this.GetCurrentUserAsync(_dbContext);

				// Todo: Allow admins to override this
				if (currentUser.Email != user.Email) return Unauthorized();

				// Send the email confirmation email
				await _userService.SendEmailConfirmationEmail(_urlHelper, "https", _emailSender, user);

				return Ok();
			}
			catch (Exception ex)
			{
				_logger.LogCritical($"Failed to send email confirmation email. Exception:{Environment.NewLine}{ex}");

				return StatusCode(500, "An error has occurred processing the Send Email Confirmation Email request.");
			}
		}

		/// <summary>
		/// 	Confirms an email address.
		/// </summary>
		/// <param name="request">
		///		The <see cref="ConfirmEmailRequest"/>.
		/// </param>
		/// <returns>
		///		The <see cref="ActionResult"/>.
		/// </returns>
		public async Task<ActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
		{
			try
			{
				// Todo: Is the user check even required? Admins shouldn't be able to override this anyway
				// Find the user
				ApplicationUser user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

				// Compare to the logged in user
				ApplicationUser currentUser = await this.GetCurrentUserAsync(_dbContext);

				// Todo: Allow admins to override this
				if (currentUser.Email != user.Email) return Unauthorized();

				// Confirm email
				await _userService.ConfirmEmail(user, request.ConfirmationCode);

				return Ok();
			}
			catch (Exception ex)
			{
				_logger.LogCritical($"Failed to confirm email. Exception:{Environment.NewLine}{ex}");

				return StatusCode(500, "An error has occurred processing the Confirm Email request.");
			}
		}

		/// <summary>
		/// 	Enables Two-Factor Authentication.
		/// </summary>
		/// <param name="request">
		///		The <see cref="EnableTwoFactorAuthenticationRequest"/>.
		/// </param>
		/// <returns>
		///		The <see cref="ActionResult"/>.
		/// </returns>
		public async Task<ActionResult> EnableTwoFactorAuthentication(
			[FromBody] EnableTwoFactorAuthenticationRequest request)
		{
			try
			{
				// Todo: Is the user check even required? Admins shouldn't be able to override this anyway
				// Find the user
				ApplicationUser user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

				// Compare to the logged in user
				ApplicationUser currentUser = await this.GetCurrentUserAsync(_dbContext);

				// Todo: Allow admins to override this
				if (currentUser.Email != user.Email) return Unauthorized();

				// Enable 2FA
				await _userService.EnableTwoFactorAuthentication(user, request.VerificationCode);

				return Ok();
			}
			catch (Exception ex)
			{
				_logger.LogCritical($"Failed to enable Two-Factor Authentication. Exception:{Environment.NewLine}{ex}");

				return StatusCode(500, "An error has occurred processing the Enable Two-Factor Authentication request.");
			}
		}

		/// <summary>
		/// 	Resets Two-Factor Authentication/
		/// </summary>
		/// <param name="request">
		///		The <see cref="ResetTwoFactorAuthenticationRequest"/>.
		/// </param>
		/// <returns>
		///		The <see cref="ActionResult"/>.
		/// </returns>
		public async Task<ActionResult> ResetTwoFactorAuthentication(
			[FromBody] ResetTwoFactorAuthenticationRequest request)
		{
			try
			{
				// Todo: Is the user check even required? Admins shouldn't be able to override this anyway
				// Find the user
				ApplicationUser user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

				// Compare to the logged in user
				ApplicationUser currentUser = await this.GetCurrentUserAsync(_dbContext);

				// Todo: Allow admins to override this
				if (currentUser.Email != user.Email) return Unauthorized();

				// Reset 2FA
				await _userService.ResetTwoFactorAuthentication(user);

				return Ok();
			}
			catch (Exception ex)
			{
				_logger.LogCritical($"Failed to reset Two-Factor Authentication. Exception:{Environment.NewLine}{ex}");

				return StatusCode(500, "An error has occurred processing the Reset Two-Factor Authentication request.");
			}
		}

		/// <summary>
		/// 	Disables Two-Factor Authentication.
		/// </summary>
		/// <param name="request">
		///		The <see cref="ActionResult"/>.
		/// </param>
		/// <returns>
		///		The <see cref="ActionResult"/>.
		/// </returns>
		public async Task<ActionResult> DisableTwoFactorAuthentication(
			[FromBody] DisableTwoFactorAuthenticationRequest request)
		{
			try
			{
				// Find the user
				ApplicationUser user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

				// Compare to the logged in user
				ApplicationUser currentUser = await this.GetCurrentUserAsync(_dbContext);

				// Todo: Allow admins to override this
				if (currentUser.Email != user.Email) return Unauthorized();

				// Disable 2FA
				await _userService.DisableTwoFactorAuthentication(user);

				return Ok();
			}
			catch (Exception ex)
			{
				_logger.LogCritical($"Failed to disable Two-Factor Authentication. Exception:{Environment.NewLine}{ex}");

				return StatusCode(500,
								  "An error has occurred processing the Disable Two-Factor Authentication request.");
			}
		}
	}
}
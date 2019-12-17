namespace OneSim.Identity.Application
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Identity;
	using Microsoft.Extensions.Logging;

	using OneSim.Identity.Application.Exceptions;
	using OneSim.Identity.Application.Interfaces;
	using OneSim.Identity.Domain.Entities;
	using OneSim.Identity.Domain.Exceptions;

	/// <summary>
	/// 	The User Service.
	/// </summary>
	public class UserService
	{
		/// <summary>
		/// 	The <see cref="UserManager{TUser}"/> for the <see cref="ApplicationUser"/>.
		/// </summary>
		private UserManager<ApplicationUser> _userManager;

		/// <summary>
		///     The <see cref="SignInManager{TUser}"/> for the <see cref="ApplicationUser"/>.
		/// </summary>
		private SignInManager<ApplicationUser> _signInManager;

		/// <summary>
		/// 	The <see cref="ILogger"/>.
		/// </summary>
		private ILogger _logger;

		/// <summary>
		/// 	Initializes a new instance of the <see cref="UserService"/> class.
		/// </summary>
		/// <param name="userManager">
		///     The <see cref="UserManager{TUser}"/> for the <see cref="ApplicationUser"/>.
		/// </param>
		/// <param name="signInManager">
		///     The <see cref="SignInManager{TUser}"/> for the <see cref="ApplicationUser"/>.
		/// </param>
		/// <param name="logger">
		///		The <see cref="ILogger"/>.
		/// </param>
		public UserService(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			ILogger logger)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_logger = logger;
		}

		/// <summary>
		///		Sends a password reset email to the email address in the given <paramref name="user"/> so long as
		/// 	they have confirmed their email address.
		/// </summary>
		/// <param name="urlHelper">
		///		The <see cref="IUrlHelper"/>.
		/// </param>
		/// <param name="requestScheme">
		///		The Request Scheme.
		/// </param>
		/// <param name="emailSender">
		///		The <see cref="IEmailSender"/>.
		/// </param>
		/// <param name="user">
		///		The <see cref="ApplicationUser"/>.
		/// </param>
		/// <returns>
		///		The <see cref="Task"/>.
		/// </returns>
		public async Task SendPasswordResetEmail(
			IUrlHelper urlHelper,
			string requestScheme,
			IEmailSender emailSender,
			ApplicationUser user)
		{
			// Check the inputs
			if (urlHelper == null) throw new ArgumentNullException(nameof(urlHelper), "The URL Helper cannot be null.");
			if (string.IsNullOrEmpty(requestScheme)) throw new ArgumentNullException(nameof(requestScheme), "The Request Scheme cannot be null or empty.");
			if (emailSender == null) throw new ArgumentNullException(nameof(emailSender), "The Email Sender cannot be null.");
			if (user == null) throw new ArgumentNullException(nameof(user), "The User cannot be null.");

			// Can't reset a password for an unconfirmed email address
			if (!await _userManager.IsEmailConfirmedAsync(user))
				throw new EmailUnconfirmedException(user, $"Cannot send password reset email when email address is unconfirmed.");

			// Get the reset token
			string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

			// Get the callback URL
			string callbackUrl = urlHelper.ResetPasswordCallbackLink(user.Id, resetToken, requestScheme);

			// Send the password reset email
			// Todo: Use HTML email
			_logger.LogInformation($"{user.UserName} has requested a password reset.");
			await emailSender.SendEmailAsync(user.Email, "Reset Password", $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
		}

		/// <summary>
		/// 	Resets or sets the password for the given <paramref name="user"/>.
		/// </summary>
		/// <param name="user">
		///     The <see cref="ApplicationUser"/>.
		/// </param>
		/// <param name="newPassword">
		///     The new password.
		/// </param>
		/// <param name="resetToken">
		///     The password reset token.
		/// </param>
		/// <returns>
		/// 	The <see cref="Task"/>.
		/// </returns>
		public async Task ResetPassword(ApplicationUser user, string newPassword, string resetToken)
		{
			// Check the inputs
			if (user == null) throw new ArgumentNullException(nameof(user), "The User cannot be null.");
			if (string.IsNullOrEmpty(newPassword))
				throw new ArgumentNullException(nameof(newPassword), "The New Password cannot be null or empty.");

			// Determine whether to set or change the password
			IdentityResult result;
			bool hasPassword = await _userManager.HasPasswordAsync(user);
			if (hasPassword)
			{
				// Only need to check the token if we're resetting the password
				if (string.IsNullOrEmpty(resetToken)) throw new ArgumentNullException(nameof(resetToken), "The Password Reset Token cannot be null or empty.");

				// Reset the password
				result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
			}
			else
			{
				// Set the password
				result = await _userManager.AddPasswordAsync(user, newPassword);
			}

			// Log some stuff
			if (result.Succeeded)
			{
				// Otherwise, just log for a normal password reset
				_logger.LogInformation($"{user.UserName} has reset their password.");
			}
			else if (result.Errors.Any())
			{
				throw new IdentityException(result.Errors, $"One or more errors occurred when attempting to reset the password for {user.UserName}.");
			}
		}

		/// <summary>
		/// 	Changes the password for the given <paramref name="user"/>.
		/// </summary>
		/// <param name="user">
		///     The <see cref="ApplicationUser"/>.
		/// </param>
		/// <param name="oldPassword">
		///     The old password.
		/// </param>
		/// <param name="newPassword">
		///		The new password.
		/// </param>
		/// <returns>
		/// 	The <see cref="Task"/>.
		/// </returns>
		public async Task ChangePassword(ApplicationUser user, string oldPassword, string newPassword)
		{
			// Check the inputs
			if (user == null) throw new ArgumentNullException(nameof(user), "The User cannot be null.");
			if (string.IsNullOrEmpty(oldPassword)) throw new ArgumentNullException(nameof(oldPassword), "The Old Password cannot be null or empty.");
			if (string.IsNullOrEmpty(newPassword)) throw new ArgumentNullException(nameof(newPassword), "The New Password cannot be null or empty.");

			// Attempt to change the password
			IdentityResult result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
			if (!result.Succeeded)
			{
				// Something went wrong
				throw new IdentityException(result.Errors, $"One or more errors occurred when attempting to change the password for {user.UserName}.");
			}

			// Log
			_logger.LogInformation($"{user.UserName} has changed their password.");

			// Attempt to re-sign in
			await _signInManager.SignInAsync(user, true);
		}

		/// <summary>
		///		Sends an email confirmation email to the given <paramref name="user"/>.
		/// </summary>
		/// <param name="urlHelper">
		///		The <see cref="IUrlHelper"/>.
		/// </param>
		/// <param name="requestScheme">
		///		The Request Scheme.
		/// </param>
		/// <param name="emailSender">
		///		The <see cref="IEmailSender"/>.
		/// </param>
		/// <param name="user">
		///		The <see cref="ApplicationUser"/>.
		/// </param>
		/// <returns>
		///		The <see cref="Task"/>.
		/// </returns>
		public async Task SendEmailConfirmationEmail(
			IUrlHelper urlHelper,
			string requestScheme,
			IEmailSender emailSender,
			ApplicationUser user)
		{
			// Check the inputs
			if (urlHelper == null) throw new ArgumentNullException(nameof(urlHelper), "The URL Helper cannot be null.");
			if (string.IsNullOrEmpty(requestScheme)) throw new ArgumentNullException(nameof(requestScheme), "The Request Scheme cannot be null or empty.");
			if (emailSender == null) throw new ArgumentNullException(nameof(emailSender), "The Email Sender cannot be null.");
			if (user == null) throw new ArgumentNullException(nameof(user), "The User cannot be null.");

			// Get the confirmation code
			string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

			// Get the callback url
			string callbackUrl = urlHelper.EmailConfirmationLink(user.Id, code, requestScheme);

			// Send the confirmation email
			// Todo: Use HTML email
			await emailSender.SendEmailAsync(user.Email,
											 "Confirm Email",
											 $"Please confirm your email address by clicking here: <a href='{callbackUrl}'>link</a>");

			// Log
			_logger.LogInformation($"A verification email has been sent to {user.UserName} at \"{user.Email}\".");
		}

		/// <summary>
		/// 	Confirms the email address for the given <paramref name="user"/> using the given <paramref name="code"/>.
		/// </summary>
		/// <param name="user">
		///     The <see cref="ApplicationUser"/>.
		/// </param>
		/// <param name="code">
		///     The confirmation code.
		/// </param>
		/// <returns>
		/// 	The <see cref="Task"/>.
		/// </returns>
		public async Task ConfirmEmail(ApplicationUser user, string code)
		{
			// Check the inputs
			if (user == null) throw new ArgumentNullException(nameof(user), "The User cannot be null.");
			if (string.IsNullOrEmpty(code))
				throw new ArgumentNullException(nameof(code), "The Confirmation Code cannot be null or empty.");

			// Confirm the email address
			IdentityResult result = await _userManager.ConfirmEmailAsync(user, code);

			// Log some stuff
			if (result.Succeeded)
			{
				_logger.LogInformation($"{user.UserName} has confirmed their email.");
			}
			else if (result.Errors.Any())
			{
				throw new IdentityException(result.Errors, $"One or more errors occurred when attempting to confirm the email for for {user.UserName}.");
			}
		}

		/// <summary>
		///		Enables Two-Factor Authentication for the given <paramref name="user"/>.
		/// </summary>
		/// <param name="user">
		///		The <see cref="ApplicationUser"/>.
		/// </param>
		/// <param name="verificationCode">
		///		The verification code.
		/// </param>
		/// <returns>
		///		The <see cref="IEnumerable{T}"/> of <see cref="string"/>s representing the recovery codes.
		/// </returns>
		public async Task<IEnumerable<string>> EnableTwoFactorAuthentication(
			ApplicationUser user,
			string verificationCode)
		{
			// Check the inputs
			if (user == null) throw new ArgumentNullException(nameof(user), "The User cannot be null.");
			if (string.IsNullOrEmpty(verificationCode))
				throw new ArgumentNullException(nameof(verificationCode), "The Verification Code cannot be null or empty.");

			// Strip spaces and hyphens
			verificationCode = verificationCode.Replace(" ", string.Empty).Replace("-", string.Empty);

			// Check if the token is valid
			bool is2FaTokenValid = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

			if (!is2FaTokenValid) throw new InvalidTwoFactorAuthenticationCodeException(verificationCode, "Verification code is invalid.");

			// Attempt to enable 2FA
			await _userManager.SetTwoFactorEnabledAsync(user, true);

			// Log
			_logger.LogInformation($"{user.UserName} has enabled 2FA.");

			// Get recovery codes
			return await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
		}

		/// <summary>
		///		Resets the Two-Factor Authentication key for the given <paramref name="user"/>.
		/// </summary>
		/// <param name="user">
		///		The <see cref="ApplicationUser"/>.
		/// </param>
		/// <returns>
		///		The <see cref="Task"/>.
		/// </returns>
		public async Task ResetTwoFactorAuthentication(ApplicationUser user)
		{
			// Check the inputs
			if (user == null) throw new ArgumentNullException(nameof(user), "The User cannot be null.");

			// Disable 2FA
			await _userManager.SetTwoFactorEnabledAsync(user, false);

			// Reset the 2FA key
			await _userManager.ResetAuthenticatorKeyAsync(user);

			// Log
			_logger.LogInformation($"{user.UserName} has reset their 2FA key.");
		}

		/// <summary>
		///		Disables Two-Factor Authentication for the given <paramref name="user"/>.
		/// </summary>
		/// <param name="user">
		///		The <see cref="ApplicationUser"/>.
		/// </param>
		/// <returns>
		///		The <see cref="Task"/>.
		/// </returns>
		public async Task DisableTwoFactorAuthentication(ApplicationUser user)
		{
			// Check the inputs
			if (user == null) throw new ArgumentNullException(nameof(user), "The User cannot be null.");

			// Attempt to disable 2fa
			IdentityResult result = await _userManager.SetTwoFactorEnabledAsync(user, false);

			// Throw an exception if something went wrong
			if (!result.Succeeded) throw new IdentityException(result.Errors, $"One or more errors occured when attempting to disable 2FA for {user.UserName}.");

			// Log
			_logger.LogInformation($"{user.UserName} has disabled 2FA.");
		}

		/// <summary>
		///		Gets the Two-Factor Authentication recovery codes for the given <paramref name="user"/>.
		/// </summary>
		/// <param name="user">
		///		The <see cref="ApplicationUser"/>.
		/// </param>
		/// <returns>
		///		The <see cref="IEnumerable{T}"/> of <see cref="string"/>s representing the recovery codes.
		/// </returns>
		public async Task<IEnumerable<string>> GetRecoveryCodes(ApplicationUser user)
		{
			// Check the inputs
			if (user == null) throw new ArgumentNullException(nameof(user), "The User cannot be null.");

			// If 2FA is not enabled, then we shouldn't be here
			if (!user.TwoFactorEnabled)
				throw new Exception($"Cannot generate recovery codes for {user.UserName} because they do not have 2FA enabled.");

			// Generate the recovery codes
			IEnumerable<string> recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

			// Log
			_logger.LogInformation($"{user.UserName} has generated new 2FA recovery codes.");

			return recoveryCodes;
		}
	}
}
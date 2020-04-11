namespace OneSim.Identity.Application
{
	using System;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Authentication;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.Extensions.Logging;

	using OneSim.Identity.Application.Abstractions;
	using OneSim.Identity.Domain.Entities;

	/// <summary>
	/// 	The Authentication Service.
	/// </summary>
	public class AuthenticationService
	{
		/// <summary>
		///     Gets t <see cref="SignInManager{TUser}"/> for the <see cref="ApplicationUser"/>.
		/// </summary>
		public SignInManager<ApplicationUser> SignInManager { get; }

		/// <summary>
		/// 	The <see cref="ILogger{TCategoryName}"/>.
		/// </summary>
		private readonly ILogger<AuthenticationService> _logger;

		/// <summary>
		///     Initializes a new instance of the <see cref="AuthenticationService"/> class.
		/// </summary>
		/// <param name="signInManager">
		///     The <see cref="SignInManager{TUser}"/> for the <see cref="ApplicationUser"/>.
		/// </param>
		/// <param name="logger">
		///		The <see cref="ILogger{TCategoryName}"/>.
		/// </param>
		public AuthenticationService(
			SignInManager<ApplicationUser> signInManager,
			ILogger<AuthenticationService> logger)
		{
			SignInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager), "The SignInManager cannot be null.");
			_logger = logger ?? throw new ArgumentNullException(nameof(logger), "The Logger cannot be null.");
		}

		/// <summary>
		///		Attempts to log the given <paramref name="user"/> in.
		/// </summary>
		/// <param name="user">
		///		The <see cref="ApplicationUser"/>.
		/// </param>
		/// <param name="password">
		///		The un-encrypted password.
		/// </param>
		/// <returns>
		///		The <see cref="Microsoft.AspNetCore.Identity.SignInResult"/>.
		/// </returns>
		public async Task<SignInResult> LogIn(ApplicationUser user, string password)
		{
			// Check the inputs
			if (user == null) throw new ArgumentNullException(nameof(user), "The User cannot be null.");
			if (string.IsNullOrEmpty(password))
				throw new ArgumentNullException(nameof(password), "The password cannot be null or empty.");

			// This doesn't count login failures towards account lockout
			// To enable password failures to trigger account lockout, set lockoutOnFailure: true
			SignInResult result = await SignInManager.PasswordSignInAsync(user,
																		   password,
																		   true,
																		   false);

			// Log some data
			if (result.Succeeded)
			{
				_logger.LogInformation($"{user.UserName} logged in.");
			}
			else if (result.IsLockedOut)
			{
				_logger.LogWarning($"{user.UserName} has been locked out.");
			}
			else if (result.RequiresTwoFactor)
			{
				_logger.LogInformation($"{user.UserName} requires two-factor authentication.");
			}
			else
			{
				_logger.LogWarning($"Failed to log in {user.UserName}.");
			}

			return result;
		}

		/// <summary>
		/// 	Attempts to log the given <paramref name="user"/> in using a Two-Factor Authentication token.
		/// </summary>
		/// <param name="user">
		///		The <see cref="ApplicationUser"/>.
		/// </param>
		/// <param name="token">
		///		The Two-Factor Authentication token.
		/// </param>
		/// <param name="isPersistent">
		///		The value indicating whether or not to remember the user.
		/// </param>
		/// <param name="rememberClient">
		///		The value indicating whether or not to remember the client machine.
		/// </param>
		/// <returns>
		///		The <see cref="Microsoft.AspNetCore.Identity.SignInResult"/>.
		/// </returns>
		public async Task<SignInResult> TwoFactorAuthenticationLogIn(
			ApplicationUser user,
			string token,
			bool isPersistent = true,
			bool rememberClient = true)
		{
			// Check the inputs
			if (user == null) throw new ArgumentNullException(nameof(user), "The User cannot be null.");
			if (string.IsNullOrEmpty(token))
				throw new ArgumentNullException(nameof(token), "The token cannot be null or empty.");

			SignInResult result =
				await SignInManager.TwoFactorAuthenticatorSignInAsync(token, isPersistent, rememberClient);

			// Log some data
			if (result.Succeeded)
			{
				_logger.LogInformation($"{user.UserName} logged in.");
			}
			else if (result.IsLockedOut)
			{
				_logger.LogWarning($"{user.UserName} has been locked out.");
			}
			else if (result.RequiresTwoFactor)
			{
				_logger.LogError($"{user.UserName} requires two-factor authentication, but this is the 2FA method.");
			}
			else
			{
				_logger.LogWarning($"Failed to log in {user.UserName}.");
			}

			return result;
		}

		/// <summary>
		///		Logs the given <paramref name="user"/> in using a Two-Factor Authentication recovery code.
		/// </summary>
		/// <param name="user">
		///		The <see cref="ApplicationUser"/>.
		/// </param>
		/// <param name="recoveryCode">
		///		The recovery code.
		/// </param>
		/// <returns>
		///		The <see cref="SignInResult"/>.
		/// </returns>
		public async Task<SignInResult> RecoveryCodeLogIn(ApplicationUser user, string recoveryCode)
		{
			// Check the inputs
			if (user == null) throw new ArgumentNullException(nameof(user), "The User cannot be null.");
			if (string.IsNullOrEmpty(recoveryCode))
				throw new ArgumentNullException(nameof(recoveryCode), "The recovery code cannot be null or empty.");

			// Clean the recovery code
			recoveryCode = recoveryCode.Replace(" ", string.Empty);

			// Sign in with 2FA recovery code
			SignInResult result = await SignInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

			// Log some stuff
			if (result.Succeeded)
			{
				_logger.LogInformation($"{user.UserName} logged in with a two factor authentication recovery code.");
			}
			else if (result.IsLockedOut)
			{
				_logger.LogWarning($"{user.UserName} has been locked out.");
			}
			else
			{
				_logger.LogWarning($"{user.UserName} has entered an invalid recovery code.");
			}

			return result;
		}
	}
}
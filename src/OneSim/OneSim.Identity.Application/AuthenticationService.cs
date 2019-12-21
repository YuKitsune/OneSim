namespace OneSim.Identity.Application
{
	using System;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Identity;
	using Microsoft.Extensions.Logging;

	using OneSim.Identity.Application.Interfaces;
	using OneSim.Identity.Domain.Entities;

	/// <summary>
	/// 	The Authentication Service.
	/// </summary>
	public class AuthenticationService
	{
		/// <summary>
		///     The <see cref="SignInManager{TUser}"/> for the <see cref="ApplicationUser"/>.
		/// </summary>
		private readonly SignInManager<ApplicationUser> _signInManager;

		/// <summary>
		/// 	The <see cref="ILogger"/>.
		/// </summary>
		private readonly ILogger _logger;

		/// <summary>
		///     The <see cref="ITokenFactory"/>.
		/// </summary>
		private readonly ITokenFactory _tokenFactory;

		/// <summary>
		///     Initializes a new instance of the <see cref="AuthenticationService"/> class.
		/// </summary>
		/// <param name="signInManager">
		///     The <see cref="SignInManager{TUser}"/> for the <see cref="ApplicationUser"/>.
		/// </param>
		/// <param name="tokenFactory">
		///     The <see cref="ITokenFactory"/>.
		/// </param>
		/// <param name="logger">
		///		The <see cref="ILogger"/>.
		/// </param>
		public AuthenticationService(
			SignInManager<ApplicationUser> signInManager,
			ITokenFactory tokenFactory,
			ILogger logger)
		{
			_signInManager = signInManager;
			_logger = logger;
			_tokenFactory = tokenFactory;
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
			SignInResult result = await _signInManager.PasswordSignInAsync(user,
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
		public async Task<SignInResult> LogInWithRecoveryCode(ApplicationUser user, string recoveryCode)
		{
			// Check the inputs
			if (user == null) throw new ArgumentNullException(nameof(user), "The User cannot be null.");
			if (string.IsNullOrEmpty(recoveryCode))
				throw new ArgumentNullException(nameof(recoveryCode), "The recovery code cannot be null or empty.");

			// Clean the recovery code
			recoveryCode = recoveryCode.Replace(" ", string.Empty);

			// Sign in with 2FA recovery code
			SignInResult result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

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

		/// <summary>
		///     Gets the for the <paramref name="user"/>.
		/// </summary>
		/// <param name="user">
		///     The <see cref="ApplicationUser"/>.
		/// </param>
		/// <returns>
		///     The token.
		/// </returns>
		public string GetToken(ApplicationUser user) => _tokenFactory.GenerateToken(user);
	}
}
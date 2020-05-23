// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountController.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;

    using OneSim.Api.Models.Identity.Authentication;
    using OneSim.Identity.Application.Abstractions;
    using OneSim.Identity.Infrastructure;
    using OneSim.Identity.Persistence;

    /// <summary>
    ///     The <see cref="Controller"/> for authentication and account management.
    /// </summary>
    [Authorize]
    public class AccountController : Controller
    {
        /// <summary>
        ///     The <see cref="IdentityDbContext"/>.
        /// </summary>
        private readonly IdentityDbContext _dbContext;

        /// <summary>
        ///     The <see cref="IUserService{TUser}"/> for the <see cref="User"/>.
        /// </summary>
        private readonly IUserService<User> _userService;

        /// <summary>
        ///     The <see cref="IAuthenticationService{TUser}"/> for the <see cref="User"/>.
        /// </summary>
        private readonly IAuthenticationService<User> _authenticationService;

        /// <summary>
        ///     The <see cref="ITwoFactorAuthenticationService{TUser}"/> for the <see cref="User"/>.
        /// </summary>
        private readonly ITwoFactorAuthenticationService<User> _twoFactorAuthenticationService;

        /// <summary>
        ///     The <see cref="ITokenService"/>.
        /// </summary>
        private readonly ITokenService _tokenService;

        /// <summary>
        ///     The <see cref="ILogger"/>.
        /// </summary>
        private readonly ILogger<AccountController> _logger;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="dbContext">
        ///     The <see cref="IdentityDbContext"/>.
        /// </param>
        /// <param name="userService">
        ///     The <see cref="IUserService{TUser}"/>.
        /// </param>
        /// <param name="authenticationService">
        ///     The <see cref="IAuthenticationService{TUser}"/>.
        /// </param>
        /// <param name="twoFactorAuthenticationService">
        ///     The <see cref="ITwoFactorAuthenticationService{TUser}"/>.
        /// </param>
        /// <param name="tokenService">
        ///     The <see cref="ITokenService"/>.
        /// </param>
        /// <param name="logger">
        ///     The <see cref="ILogger{TCategoryName}"/>.
        /// </param>
        public AccountController(
            IdentityDbContext dbContext,
            IUserService<User> userService,
            IAuthenticationService<User> authenticationService,
            ITwoFactorAuthenticationService<User> twoFactorAuthenticationService,
            ITokenService tokenService,
            ILogger<AccountController> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _twoFactorAuthenticationService = twoFactorAuthenticationService ?? throw new ArgumentNullException(nameof(twoFactorAuthenticationService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Handles the request to register a new account.
        /// </summary>
        /// <param name="request">
        ///     The <see cref="RegisterRequest"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            // If the request is not valid, bad request
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                // Check no conflicting users exist
                User existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "The given email address is already registered to an account.");

                    return BadRequest(ModelState);
                }

                existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "The given username is already registered to an account.");

                    return BadRequest(ModelState);
                }

                // Create a new user
                User user = new User { Email = request.Email, UserName = request.UserName };

                // Register the user
                await _userService.RegisterUserAsync(user, request.Password);
                return Ok(new RegisterResponse { UserId = user.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has occurred registering an account with email \"{request.Email}\" and username \"{request.UserName}\".");
                ModelState.AddModelError(string.Empty, "An error has occurred while attempting to register your account.");

                return BadRequest();
            }
        }

        /// <summary>
        ///     Handles the Login request.
        /// </summary>
        /// <param name="request">
        ///     The <see cref="SignInRequest"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(SignInRequest request)
        {
            // If the request is not valid, bad request
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                // Get the user
                User user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

                // Check if the user and their credentials are valid
                if (user != null &&
                    await _userService.CheckPasswordAsync(user, request.Password))
                {
                    // Todo: If RememberMe is false, set token expiry to 24 hours, otherwise, 12 months

                    // Attempt to sign the user in
                    ISignInResult result = await _authenticationService.SignInAsync(user, request.Password);

                    // Figure out what to do
                    IActionResult actionResult = await GetActionResultFromSignInResultAsync(result);
                    if (actionResult != null) return actionResult;
                }

                // Show a generic error if invalid, can't be specific about what went wrong for security reasons
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has occurred logging into an account with email \"{request.Email}\".");
                ModelState.AddModelError(string.Empty, "An error has occurred while attempting to log in your account.");
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        ///     Handles the Two-Factor Authentication Login request.
        /// </summary>
        /// <param name="request">
        ///     The <see cref="TwoFactorSignInRequest"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithTwoFactorAuthentication(TwoFactorSignInRequest request)
        {
            // If the request is not valid, bad request
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Get the user
            User user = await _twoFactorAuthenticationService.GetTwoFactorAuthenticationUserAsync();
            try
            {
                // Attempt to log in
                ISignInResult result = await _twoFactorAuthenticationService.TwoFactorSignInAsync(
                                           user,
                                           request.TwoFactorCode,
                                           request.RememberMe,
                                           request.RememberMachine);

                // Figure out what to do
                IActionResult actionResult = await GetActionResultFromSignInResultAsync(result);
                if (actionResult != null) return actionResult;

                // 2FA code was wrong, try again
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has occurred logging into an account using 2FA with code \"{request.TwoFactorCode}\".");
                ModelState.AddModelError(string.Empty, "An error has occurred while attempting to log in your account.");
            }

            // Made it this far, something hasn't gone right
            return BadRequest(ModelState);
        }

        /// <summary>
        ///     Handles the 2FA recovery code login request.
        /// </summary>
        /// <param name="request">
        ///     The <see cref="RecoveryCodeSignInRequest"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithRecoveryCode(RecoveryCodeSignInRequest request)
        {
            // If the request is not valid, bad request
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                // Get the user
                User user = await _twoFactorAuthenticationService.GetTwoFactorAuthenticationUserAsync();
                if (user == null) throw new ApplicationException("Unable to load two-factor authentication user.");

                // Attempt to log in
                ISignInResult result =
                    await _twoFactorAuthenticationService.RecoveryCodeSignInAsync(user, request.RecoveryCode);

                // Figure out what to do
                IActionResult actionResult = await GetActionResultFromSignInResultAsync(result);
                if (actionResult != null) return actionResult;

                // 2FA recovery code was wrong, try again
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has occurred logging into an account using 2FA recovery with code \"{request.RecoveryCode}\".");
                ModelState.AddModelError(string.Empty, "An error has occurred while attempting to log in your account.");
            }

            // Made it this far, something hasn't gone right
            return BadRequest(ModelState);
        }

        /// <summary>
        ///     Handles the request to delete an account.
        /// </summary>
        /// <param name="request">
        ///     The <see cref="DeleteAccountRequest"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount(DeleteAccountRequest request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Handles the request to send a verification email.
        /// </summary>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet, ValidateAntiForgeryToken]
        public async Task<IActionResult> SendVerificationEmail()
        {
            // Get the current user
            User user = await _userService.GetUserAsync(User);

            try
            {
                // Send the email confirmation email
                await _userService.SendVerificationEmailAsync(user);
            }
            catch (Exception ex)
            {
                // Todo: Find another variable to log, or wrap in a null check, might be in the catch because User was null
                _logger.LogError(ex, $"An error has occurred sending a verification email to user with email \"{user.Email}\".");
                ModelState.AddModelError(string.Empty, "An error has occurred while attempting to send the verification email.");
            }

            return Ok();
        }

        /// <summary>
        ///     Handles the request to confirm an email address.
        /// </summary>
        /// <param name="confirmationCode">
        ///     The confirmation code contained in the email confirmation email.
        /// </param>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> VerifyEmail(string confirmationCode)
        {
            // Get the current user
            User user = await _userService.GetUserAsync(User);

            try
            {
                // Confirm email
                await _userService.VerifyEmailAsync(user, confirmationCode);
            }
            catch (Exception ex)
            {
                // Todo: Find another variable to log, or wrap in a null check, might be in the catch because User was null
                _logger.LogError(ex, $"An error has occurred when verifying the email \"{user.Email}\".");
                ModelState.AddModelError(string.Empty, "An error has occurred while attempting to verify your email.");
            }

            return Ok();
        }

        /// <summary>
        ///     Handles the request to send a password reset email.
        /// </summary>
        /// <param name="request">
        ///     The <see cref="ForgotPasswordRequest"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            // Get the user
            User user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user != null)
            {
                try
                {
                    // Send the email
                    await _userService.SendPasswordResetEmailAsync(user);
                }
                catch (Exception ex)
                {
                    // Todo: Find another variable to log, or wrap in a null check, might be in the catch because User was null
                    _logger.LogError(ex, $"An error has occurred when sending a Password Reset email to user with email \"{user.Email}\".");
                    ModelState.AddModelError(string.Empty, "An error has occurred while attempting to send the Password Reset email.");

                    return BadRequest(ModelState);
                }
            }
            else
            {
                _logger.LogWarning($"Password reset requested for user with email \"{request.Email}\", but no user with that email exists.");
            }

            // Can't say it failed, otherwise that poses a security threat
            return Ok();
        }

        /// <summary>
        ///     Handles the request to reset the users password.
        /// </summary>
        /// <param name="request">
        ///     The <see cref="ResetPasswordRequest"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            // If the request is not valid, bad request
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                // Get the user
                User user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

                // Todo: Custom domain exception
                if (user == null) throw new ApplicationException($"No user with email {request.Email} exists.");

                // Reset the password
                await _userService.ResetPasswordAsync(user, request.NewPassword, request.ResetToken);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has occurred when resetting the password for user with email \"{request.Email}\".");
                ModelState.AddModelError(string.Empty, "An error has occurred while attempting to reset your password.");

                return BadRequest(ModelState);
            }
        }

        /// <summary>
        ///     Handles the request to change the current users password.
        /// </summary>
        /// <param name="viewModel">
        ///     The <see cref="ChangePasswordRequest"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest viewModel)
        {
            // Get the current user
            User user = await _userService.GetUserAsync(User);

            try
            {
                // Change the password
                await _userService.ChangePasswordAsync(user, viewModel.OldPassword, viewModel.NewPassword);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has occurred when changing the password for user with email \"{user.Email}\".");
                ModelState.AddModelError(string.Empty, "An error has occurred while attempting to change your password.");

                return BadRequest(ModelState);
            }
        }

        /// <summary>
        ///     Returns the enable Two-Factor Authentication view.
        /// </summary>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> EnableTwoFactorAuthentication()
        {
            // Get the current user
            User user = await _userService.GetUserAsync(User);

            // Check if 2FA is enabled
            if (user.TwoFactorEnabled)
            {
                // Todo: Need to provide some user feedback somehow
                return BadRequest();
            }

            try
            {
                // Get the key and URI
                string sharedKey = await _twoFactorAuthenticationService.GetSharedKeyAsync(user);
                Uri uri = await _twoFactorAuthenticationService.GetAuthenticatorUriAsync(user, sharedKey);

                return Ok(
                    new TwoFactorAuthenticationVariables { SharedKey = sharedKey, AuthenticatorUri = uri.ToString() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has occurred when attempting to display the Enable 2FA view for user \"{user.Id}\".");
                ModelState.AddModelError(string.Empty, "An error has occurred while attempting to display the \"Enable Two-Factor Authentication\" page.");

                return BadRequest(ModelState);
            }
        }

        /// <summary>
        ///     Handles the request to enable Two-Factor Authentication.
        /// </summary>
        /// <param name="request">
        ///     The <see cref="EnableTwoFactorAuthenticationRequest"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableTwoFactorAuthentication(
            EnableTwoFactorAuthenticationRequest request)
        {
            // If the request is not valid, bad request
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Get the current user
            User user = await _userService.GetUserAsync(User);

            try
            {
                // Enable 2FA
                IEnumerable<string> recoveryCodes =
                    await _twoFactorAuthenticationService.EnableTwoFactorAuthenticationAsync(user, request.VerificationCode);

                return Ok(new EnableTwoFactorAuthenticationResponse(recoveryCodes.ToList()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has occurred when attempting to enable 2FA for user \"{user.Id}\".");
                ModelState.AddModelError(string.Empty, "An error has occurred while attempting to enable Two-Factor Authentication.");

                return BadRequest(ModelState);
            }
        }

        /// <summary>
        ///     Handles the request to reset Two-Factor Authentication.
        /// </summary>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> ResetTwoFactorAuthentication()
        {
            // Get the current user
            User user = await _userService.GetUserAsync(User);

            try
            {
                // Reset 2FA
                await _twoFactorAuthenticationService.ResetTwoFactorAuthenticationAsync(user);

                // Redirect to Enable 2FA
                return await EnableTwoFactorAuthentication();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has occurred when attempting to reset 2FA for user \"{user.Id}\".");
                ModelState.AddModelError(string.Empty, "An error has occurred while attempting to reset Two-Factor Authentication.");

                return BadRequest(ModelState);
            }
        }

        /// <summary>
        ///     Handles the request to disable Two-Factor Authentication.
        /// </summary>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> DisableTwoFactorAuthentication()
        {
            // Get the current user
            User user = await _userService.GetUserAsync(User);

            try
            {
                // Disable 2FA
                await _twoFactorAuthenticationService.DisableTwoFactorAuthenticationAsync(user);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has occurred when attempting to disable 2FA for user \"{user.Id}\".");
                ModelState.AddModelError(string.Empty, "An error has occurred while attempting to disable Two-Factor Authentication.");

                return BadRequest(ModelState);
            }
        }

        /// <summary>
        ///     Gets an <see cref="IActionResult"/> given an <see cref="ISignInResult"/> as an asynchronous operation.
        /// </summary>
        /// <param name="result">
        ///     The <see cref="ISignInResult"/> from a sign-in attempt.
        /// </param>
        /// <returns>
        ///     The <see cref="Task{TResult}"/> representing the asynchronous operation.
        ///     The <see cref="Task{TResult}.Result"/> contains the <see cref="IActionResult"/> to use. <c>null</c> will
        ///     be returned if there was some sort of problem.
        /// </returns>
        private async Task<IActionResult> GetActionResultFromSignInResultAsync(ISignInResult result)
        {
            // If the user is locked out, then give them the bad news
            // Todo: if (result.IsNotAllowed) return Forbid();
            if (result.IsLockedOut) return Ok(new SignInResponse { LockedOut = true });

            // If 2FA is required, then redirect to the 2FA login view
            if (result.RequiresTwoFactor)
                return Ok(new SignInResponse { TwoFactorAuthenticationRequired = true });

            // Everything cool, let's rock and roll
            if (result.Succeeded)
            {
                // Todo: Get expiry dates
                User user = await _userService.GetUserAsync(User);
                SecurityToken token = _tokenService.GetToken(user, DateTimeOffset.Now.AddHours(24));
                return Ok(new SignInResponse { Token = token.ToString() });
            }

            return null;
        }
    }
}

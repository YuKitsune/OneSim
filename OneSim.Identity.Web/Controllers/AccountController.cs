// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountController.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using IdentityModel;

    using IdentityServer4;
    using IdentityServer4.Models;
    using IdentityServer4.Services;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    using OneSim.Identity.Application.Abstractions;
    using OneSim.Identity.Infrastructure;
    using OneSim.Identity.Persistence;
    using OneSim.Identity.Web.ViewModels.Account;

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
        ///     The <see cref="IIdentityServerInteractionService"/>.
        /// </summary>
        private readonly IIdentityServerInteractionService _interactionService;

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
        /// <param name="identityService">
        ///     The <see cref="IIdentityServerInteractionService"/>.
        /// </param>
        /// <param name="logger">
        ///     The <see cref="ILogger{TCategoryName}"/>.
        /// </param>
        public AccountController(
            IdentityDbContext dbContext,
            IUserService<User> userService,
            IAuthenticationService<User> authenticationService,
            ITwoFactorAuthenticationService<User> twoFactorAuthenticationService,
            IIdentityServerInteractionService identityService,
            ILogger<AccountController> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _twoFactorAuthenticationService = twoFactorAuthenticationService ?? throw new ArgumentNullException(nameof(twoFactorAuthenticationService));
            _interactionService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Returns the index view.
        /// </summary>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet]
        public IActionResult Index() => View();

        /// <summary>
        ///     Returns the Login view.
        /// </summary>
        /// <param name="callbackUri">
        ///     The callback URI.
        /// </param>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> Login(string callbackUri)
        {
            // Get the authorization request
            AuthorizationRequest context = await _interactionService.GetAuthorizationContextAsync(callbackUri);

            if (context?.IdP != null) throw new NotSupportedException("External login is not supported.");

            return View(new LoginViewModel { CallbackUri = callbackUri, Email = context?.LoginHint });
        }

        /// <summary>
        ///     Handles the Login request.
        /// </summary>
        /// <param name="viewModel">
        ///     The <see cref="LoginViewModel"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            // If the model is valid, continue with the login
            if (ModelState.IsValid)
            {
                // Get the user
                User user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == viewModel.Email);

                // Check if the user and their credentials are valid
                if (user != null &&
                    await _userService.CheckPasswordAsync(user, viewModel.Password))
                {
                    // Todo: Get the token lifetime from the configuration file, otherwise default to 120 minutes
                    int tokenLifetime = 120;

                    AuthenticationProperties props = new AuthenticationProperties
                                                     {
                                                         ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(tokenLifetime),
                                                         AllowRefresh = true,
                                                         RedirectUri = viewModel.CallbackUri
                                                     };

                    // If requested to remember the login, then configure the permanent token
                    if (viewModel.RememberMe)
                    {
                        // Todo: Get token lifetime from configuration file
                        int permanentTokenLifetime = 365;

                        props.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(permanentTokenLifetime);
                        props.IsPersistent = true;
                    }

                    // Attempt to sign the user in
                    ISignInResult result = await _authenticationService.SignInAsync(user, viewModel.Password);

                    // Todo: Check if email confirmation is required

                    // If 2FA is required, then redirect to the 2FA login view
                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToAction(
                            nameof(LoginWithTwoFactorAuthentication),
                            new { viewModel.RememberMe, ReturnUrl = viewModel.CallbackUri });
                    }

                    // Determine where to redirect
                    IActionResult redirectResult = GetPostLoginRedirect(result, viewModel.CallbackUri);

                    return redirectResult;
                }

                // Throw a generic error if invalid, can't be specific about what went wrong for security reasons
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
            }

            // Something went wrong, clean the view model and go back
            return View(await BuildLoginViewModelAsync(viewModel));
        }

        /// <summary>
        ///     Returns the Two-Factor Authentication Login View.
        /// </summary>
        /// <param name="rememberMe">
        ///     The value indicating whether or not to remember the user.
        /// </param>
        /// <param name="callbackUri">
        ///     The URI to redirect to once login has completed.
        /// </param>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> LoginWithTwoFactorAuthentication(bool rememberMe, string callbackUri)
        {
            // Ensure the user has gone through the username & password screen first
            User user = await _twoFactorAuthenticationService.GetTwoFactorAuthenticationUserAsync();

            // Throw if we don't have a user
            // Todo: Custom domain exception
            if (user == null) throw new ApplicationException("Unable to load two-factor authentication user.");

            // Create the model and send back with the view
            return View(new TwoFactorLoginViewModel { RememberMe = rememberMe, CallbackUri = callbackUri });
        }

        /// <summary>
        ///     Handles the Two-Factor Authentication Login request.
        /// </summary>
        /// <param name="viewModel">
        ///     The <see cref="TwoFactorLoginViewModel"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithTwoFactorAuthentication(TwoFactorLoginViewModel viewModel)
        {
            // If invalid, re-display the form and try again
            if (!ModelState.IsValid) return View(viewModel);

            // Get the user
            User user = await _twoFactorAuthenticationService.GetTwoFactorAuthenticationUserAsync();
            try
            {
                // Attempt to log in
                ISignInResult result = await _twoFactorAuthenticationService.TwoFactorSignInAsync(
                                           user,
                                           viewModel.TwoFactorCode,
                                           viewModel.RememberMe,
                                           viewModel.RememberMachine);

                // Determine where to redirect
                IActionResult redirectResult = GetPostLoginRedirect(result, viewModel.CallbackUri);
                if (result != null) return redirectResult;

                // 2FA code was wrong, try again
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");

                return View(
                    new TwoFactorLoginViewModel
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

                return View(
                    new TwoFactorLoginViewModel
                    {
                        CallbackUri = viewModel.CallbackUri,
                        RememberMe = viewModel.RememberMe,
                        RememberMachine = viewModel.RememberMachine
                    });
            }
        }

        /// <summary>
        ///     Returns the Two-Factor Authentication recovery code login view.
        /// </summary>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode()
        {
            // Ensure the user has gone through the username & password screen first
            User user = await _twoFactorAuthenticationService.GetTwoFactorAuthenticationUserAsync();

            // Throw if we don't have a user
            // Todo: Custom domain exception
            if (user == null) throw new ApplicationException("Unable to load two-factor authentication user.");

            return View();
        }

        /// <summary>
        ///     Handles the 2FA recovery code login request.
        /// </summary>
        /// <param name="viewModel">
        ///     The <see cref="TwoFactorRecoveryCodeLoginViewModel"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithRecoveryCode(TwoFactorRecoveryCodeLoginViewModel viewModel)
        {
            // If invalid, re-display the form and try again
            if (!ModelState.IsValid) return View(viewModel);

            // Get the user
            User user = await _twoFactorAuthenticationService.GetTwoFactorAuthenticationUserAsync();

            if (user == null) throw new ApplicationException("Unable to load two-factor authentication user.");

            try
            {
                // Attempt to log in
                ISignInResult result =
                    await _twoFactorAuthenticationService.RecoveryCodeSignInAsync(user, viewModel.RecoveryCode);

                // Determine where to redirect
                IActionResult redirectResult = GetPostLoginRedirect(result, viewModel.CallbackUri);
                if (result != null) return redirectResult;

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
        ///     Returns the account registration view.
        /// </summary>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet, AllowAnonymous]
        public IActionResult Register() => View();

        /// <summary>
        ///     Handles the request to register a new account.
        /// </summary>
        /// <param name="viewModel">
        ///     The <see cref="RegisterViewModel"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            try
            {
                // Check no conflicting users exist
                User existingUserEmail = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == viewModel.Email);
                User existingUserUserName = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == viewModel.UserName);
                if (existingUserEmail != null)
                {
                    ModelState.AddModelError(string.Empty, "The given email address is already registered to an account.");

                    return View();
                }

                if (existingUserUserName != null)
                {
                    ModelState.AddModelError(string.Empty, "The given username is already registered to an account.");

                    return View();
                }

                // Create a new user
                User user = new User { Email = viewModel.Email, UserName = viewModel.UserName };

                // Register the user
                await _userService.RegisterUserAsync(user, viewModel.Password);

                // Todo: Redirect somewhere telling the user they need to verify their email.
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has occurred registering an account with email \"{viewModel.Email}\" and username \"{viewModel.UserName}\".");
                ModelState.AddModelError(string.Empty, $"An error has occurred registering the account.");

                return View();
            }
        }

        /// <summary>
        ///     Returns the account deletion view.
        /// </summary>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet]
        public IActionResult DeleteAccount() => View();

        /// <summary>
        ///     Handles the request to delete an account.
        /// </summary>
        /// <param name="viewModel">
        ///     The <see cref="DeleteAccountViewModel"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount(DeleteAccountViewModel viewModel)
        {
            throw new NotImplementedException();

            // Get the current user
            User user = await _userService.GetUserAsync(User);

            try
            {
                // Check the password is correct
                bool passwordMatches = await _userService.CheckPasswordAsync(user, viewModel.Password);
                if (passwordMatches)
                {
                    // Todo: await _userService.DeActivateUserAsync(user);
                    return View("AccountDeleted");
                }

                // Todo: Log security stuff separately, and also keep in mind the amount of failed attempts
                //  Might pose a security issue
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

                return RedirectToAction(
                    nameof(Index),
                    new IndexViewModel { Message = $"An Email Confirmation email has been sent to \"{user.Email}\"." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has occurred sending an Email Confirmation email to user with email \"{user.Email}\".");

                return RedirectToAction(
                    nameof(Index),
                    new IndexViewModel
                    {
                        Message = "An error occurred while attempting to send the Email Confirmation email.",
                        MessageIsError = true
                    });
            }
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

                return RedirectToAction(nameof(Index), new IndexViewModel { Message = "Email address verified." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has occurred verifying the email for user with email \"{user.Email}\".");

                return RedirectToAction(
                    nameof(Index),
                    new IndexViewModel
                    {
                        Message = "An error occurred while attempting to verify the email address.",
                        MessageIsError = true
                    });
            }
        }

        /// <summary>
        ///     Returns the forgot password view.
        /// </summary>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet, AllowAnonymous]
        public IActionResult ForgotPassword() => View();

        /// <summary>
        ///     Handles the request to send a password reset email.
        /// </summary>
        /// <param name="viewModel">
        ///     The <see cref="ForgotPasswordViewModel"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel viewModel)
        {
            // Get the user
            User user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == viewModel.Email);

            if (user != null)
            {
                try
                {
                    // Send the email
                    await _userService.SendPasswordResetEmailAsync(user);
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
        ///     Returns the password reset view.
        /// </summary>
        /// <param name="token">
        ///     The password reset token.
        /// </param>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet, AllowAnonymous]
        public IActionResult ResetPassword(string token) => View(new ResetPasswordViewModel { ResetToken = token });

        /// <summary>
        ///     Handles the request to reset the users password.
        /// </summary>
        /// <param name="viewModel">
        ///     The <see cref="ResetPasswordViewModel"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel viewModel)
        {
            // Get the user
            User user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == viewModel.Email);

            try
            {
                // Reset the password
                await _userService.ResetPasswordAsync(user, viewModel.NewPassword, viewModel.ResetToken);

                return RedirectToAction(
                    "Login",
                    "Account",
                    new LoginViewModel { Message = "Password has been reset." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has occurred resetting the password for user with email \"{user.Email}\".");
                ModelState.AddModelError(string.Empty, "An error occurred while attempting to reset the password.");

                return View(new ResetPasswordViewModel { Email = viewModel.Email, ResetToken = viewModel.ResetToken });
            }
        }

        /// <summary>
        ///     Returns the change password view.
        /// </summary>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet]
        public IActionResult ChangePassword() => View();

        /// <summary>
        ///     Handles the request to change the current users password.
        /// </summary>
        /// <param name="viewModel">
        ///     The <see cref="ChangePasswordViewModel"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel viewModel)
        {
            // Get the current user
            User user = await _userService.GetUserAsync(User);

            try
            {
                // Change the password
                await _userService.ChangePasswordAsync(user, viewModel.OldPassword, viewModel.NewPassword);

                return RedirectToAction(nameof(Index), new IndexViewModel { Message = "Password has been changed." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has occurred changing the password for user with email \"{user.Email}\".");

                return RedirectToAction(
                    nameof(Index),
                    new IndexViewModel
                    {
                        Message = "An error occurred while attempting to change password.",
                        MessageIsError = true
                    });
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
                return RedirectToAction(
                    nameof(Index),
                    new IndexViewModel { Message = "Two-Factor Authentication is already enabled." });
            }

            try
            {
                // Get the key and URI
                string sharedKey = await _twoFactorAuthenticationService.GetSharedKeyAsync(user);
                Uri uri = await _twoFactorAuthenticationService.GetAuthenticatorUriAsync(user, sharedKey);

                return View(
                    new EnableTwoFactorAuthenticationViewModel
                    {
                        SharedKey = sharedKey, AuthenticatorUri = uri.ToString()
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has occurred enabling Two-Factor Authentication for user with email \"{user.Email}\".");

                return RedirectToAction(
                    nameof(Index),
                    new IndexViewModel
                    {
                        Message = "An error occurred while attempting to enable Two-Factor Authentication.",
                        MessageIsError = true
                    });
            }
        }

        /// <summary>
        ///     Handles the request to enable Two-Factor Authentication.
        /// </summary>
        /// <param name="viewModel">
        ///     The <see cref="EnableTwoFactorAuthenticationViewModel"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableTwoFactorAuthentication(
            EnableTwoFactorAuthenticationViewModel viewModel)
        {
            // Get the current user
            User user = await _userService.GetUserAsync(User);

            try
            {
                // Enable 2FA
                IEnumerable<string> recoveryCodes =
                    await _twoFactorAuthenticationService.EnableTwoFactorAuthenticationAsync(user, viewModel.VerificationCode);

                return View("RecoveryCodes", new RecoveryCodesViewModel { RecoveryCodes = recoveryCodes });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has occurred enabling Two-Factor Authentication for user with email \"{user.Email}\".");
                ModelState.AddModelError(string.Empty, "An error occurred while attempting to enable Two-Factor Authentication.");

                return View(
                    new EnableTwoFactorAuthenticationViewModel
                    {
                        SharedKey = viewModel.SharedKey, AuthenticatorUri = viewModel.AuthenticatorUri
                    });
            }
        }

        /// <summary>
        ///     Returns the Two-Factor Authentication reset warning view.
        /// </summary>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet]
        public IActionResult ResetTwoFactorAuthenticationWarning() => View();

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
        ///     Returns the Two-Factor Authentication disable warning view.
        /// </summary>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet]
        public IActionResult DisableTwoFactorAuthenticationWarning() => View();

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

                return RedirectToAction(
                    nameof(Index),
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

        /// <summary>
        ///     Returns the Logout view.
        /// </summary>
        /// <param name="logoutId">
        ///     The logout ID.
        /// </param>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
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
        ///     Handles the Logout request.
        /// </summary>
        /// <param name="viewModel">
        ///     The <see cref="LogoutViewModel"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutViewModel viewModel)
        {
            // Get the identity provider
            string identityProvider = User?.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;

            // Check if the identity provider is an external provider
            if (!string.IsNullOrEmpty(identityProvider) &&
                identityProvider != IdentityServerConstants.LocalIdentityProvider)
            {
                // If there's no current logout context, we need to create one
                // this captures necessary info from the current logged in user
                // before we sign out and redirect away to the external IdP for sign out
                viewModel.LogoutId ??= await _interactionService.CreateLogoutContextAsync();

                // Todo: Update this URL
                string url = "/Authentication/Logout?logoutId=" + viewModel.LogoutId;

                try
                {
                    // Hack: try/catch to handle social providers that throw
                    await HttpContext.SignOutAsync(
                        identityProvider,
                        new AuthenticationProperties { RedirectUri = url });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"LOGOUT ERROR: {ex.Message}");
                }
            }

            // Delete authentication cookie
            await _authenticationService.SignOutAsync();

            // Get context information (client name, post logout redirect URI and iframe for federated sign out)
            LogoutRequest logout = await _interactionService.GetLogoutContextAsync(viewModel.LogoutId);

            // Show the Logged Out page
            return View("LoggedOut", logout);
        }

        /// <summary>
        ///     Handles the Logout request from a device.
        /// </summary>
        /// <param name="redirectUrl">
        ///     The URL to redirect to after logging out.
        /// </param>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> DeviceLogout(string redirectUrl)
        {
            // Delete authentication cookie
            await HttpContext.SignOutAsync();

            // Set this so UI rendering sees an anonymous user
            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            return Redirect(redirectUrl);
        }

        /// <summary>
        ///     Returns the redirecting view.
        /// </summary>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet]
        public IActionResult Redirecting() => View();

        /// <summary>
        ///     Returns the lockout view.
        /// </summary>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet, AllowAnonymous]
        public IActionResult Lockout() => View();

        /// <summary>
        ///     Builds a clean <see cref="LoginViewModel"/> (with no password)
        ///     given a dirty <see cref="LoginViewModel"/> (with password).
        /// </summary>
        /// <param name="viewModel">
        ///     The dirty <see cref="LoginViewModel"/>.
        /// </param>
        /// <returns>
        ///     The clean <see cref="LoginViewModel"/>.
        /// </returns>
        private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginViewModel viewModel)
        {
            AuthorizationRequest context = await _interactionService.GetAuthorizationContextAsync(viewModel.CallbackUri);
            LoginViewModel vm = new LoginViewModel { CallbackUri = viewModel.CallbackUri, Email = context?.LoginHint };
            vm.Email = viewModel.Email;
            vm.RememberMe = viewModel.RememberMe;

            return vm;
        }

        /// <summary>
        ///     Gets the required <see cref="RedirectResult"/> given the <see cref="ISignInResult"/> assuming the
        ///     <paramref name="result"/> is the <see cref="ISignInResult"/> of a login attempt.
        /// </summary>
        /// <param name="result">
        ///     The <see cref="ISignInResult"/>.
        /// </param>
        /// <param name="callbackUri">
        ///     The callback URI if any.
        /// </param>
        /// <returns>
        ///     The required <see cref="RedirectResult"/>.
        ///     If no <see cref="RedirectResult"/> could be determined, then a redirect to the Login action is returned.
        /// </returns>
        private IActionResult GetPostLoginRedirect(ISignInResult result, string callbackUri)
        {
            // If successful, make sure the callback URI is still valid,
            // and if so, redirect back to authorize endpoint
            if (result.Succeeded)
            {
                if (_interactionService.IsValidReturnUrl(callbackUri))
                {
                    return Redirect(callbackUri);
                }

                return RedirectToAction("Index", "Account");
            }

            // If locked out, then redirect to the lockout view
            if (result.IsLockedOut) return RedirectToAction("Lockout", "Account");

            return RedirectToAction("Login");
        }
    }
}

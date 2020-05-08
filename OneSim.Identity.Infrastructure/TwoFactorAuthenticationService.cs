// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TwoFactorAuthenticationService.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.Encodings.Web;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;

    using OneSim.Identity.Application.Abstractions;
    using OneSim.Identity.Infrastructure.Exceptions;
    using OneSim.Identity.Infrastructure.Extensions;

    /// <summary>
    ///     The implementation of the <see cref="ITwoFactorAuthenticationService{TUser}"/> for the <see cref="User"/>
    ///     using the ASP.NET Core <see cref="SignInManager{TUser}"/> and <see cref="UserManager{TUser}"/>.
    /// </summary>
    public class TwoFactorAuthenticationService : ITwoFactorAuthenticationService<User>
    {
        /// <summary>
        ///     The Two-Factor Authenticator URI format.
        /// </summary>
        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        /// <summary>
        ///     The <see cref="SignInManager{TUser}"/> for the <see cref="User"/>.
        /// </summary>
        private readonly SignInManager<User> _signInManager;

        /// <summary>
        ///     The <see cref="UserManager{TUser}"/> for the <see cref="User"/>.
        /// </summary>
        private readonly UserManager<User> _userManager;

        /// <summary>
        ///     The <see cref="ILogger"/> for the <see cref="TwoFactorAuthenticationService"/>.
        /// </summary>
        private readonly ILogger<TwoFactorAuthenticationService> _logger;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TwoFactorAuthenticationService"/> class.
        /// </summary>
        /// <param name="signInManager">
        ///     The <see cref="SignInManager{TUser}"/> for the <see cref="User"/>.
        /// </param>
        /// <param name="userManager">
        ///     The <see cref="UserManager{TUser}"/> for the <see cref="User"/>.
        /// </param>
        /// <param name="logger">
        ///     The <see cref="ILogger{TCategoryName}"/> for the <see cref="AuthenticationService"/>.
        /// </param>
        public TwoFactorAuthenticationService(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            ILogger<TwoFactorAuthenticationService> logger)
        {
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Attempts to sign the <see cref="User"/> in with a Two-Factor Authentication code as an
        ///     asynchronous operation.
        /// </summary>
        /// <param name="user">
        ///     The <see cref="User"/> to sign-in.
        /// </param>
        /// <param name="code">
        ///     The Two-Factor Authentication code.
        /// </param>
        /// <param name="isPersistent">
        ///     Whether the sign-in cookie should persist after the browser is closed.
        /// </param>
        /// <param name="rememberClient">
        ///     Whether the current browser should be remembered, suppressing all further two factor authentication
        ///     prompts.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken"/> to observe while waiting for the <see cref="Task{T}"/> to complete.
        /// </param>
        /// <returns>
        ///     The <see cref="Task{T}"/> representing the asynchronous operation.
        ///     The <see cref="Task{T}.Result"/> contains a <see cref="ISignInResult"/> for the sign-in attempt.
        /// </returns>
        public async Task<ISignInResult> TwoFactorSignInAsync(
            User user,
            string code,
            bool isPersistent = false,
            bool rememberClient = false,
            CancellationToken cancellationToken = default)
        {
            // Check the parameters
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(code)) throw new ArgumentNullException(nameof(code));

            // Attempt to sign-in
            cancellationToken.ThrowIfCancellationRequested();
            SignInResult result =
                await _signInManager.TwoFactorAuthenticatorSignInAsync(code, isPersistent, rememberClient);

            // Log
            _logger.LogResult(user, result);

            return new InternalSignInResult(result);
        }

        /// <summary>
        ///     Attempts to sign the <see cref="User"/> in with a Two-Factor Authentication recovery code.
        /// </summary>
        /// <param name="user">
        ///     The <see cref="User"/> to sign-in.
        /// </param>
        /// <param name="code">
        ///     The Two-Factor Authentication recovery code.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken"/> to observe while waiting for the <see cref="Task{T}"/> to complete.
        /// </param>
        /// <returns>
        ///     The <see cref="Task{T}"/> representing the asynchronous operation.
        ///     The <see cref="Task{T}.Result"/> contains a <see cref="ISignInResult"/> for the sign-in attempt.
        /// </returns>
        public async Task<ISignInResult> RecoveryCodeSignInAsync(
            User user,
            string code,
            CancellationToken cancellationToken = default)
        {
            // Check the parameters
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(code)) throw new ArgumentNullException(nameof(code));

            // Clean the recovery code
            code = code.Replace(" ", string.Empty);

            // Sign in with 2FA recovery code
            cancellationToken.ThrowIfCancellationRequested();
            SignInResult result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(code);

            // Log
            _logger.LogResult(user, result);

            return new InternalSignInResult(result);
        }

        /// <summary>
        ///     Gets the <see cref="User"/> for the current Two-Factor Authentication login attempt as an asynchronous
        ///     operation.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/> representing the asynchronous operation.
        ///     The <see cref="Task{T}.Result"/> contains the <see cref="User"/>.
        /// </returns>
        public async Task<User> GetTwoFactorAuthenticationUserAsync() => await _signInManager.GetTwoFactorAuthenticationUserAsync();

        /// <summary>
        ///     Gets the shared Two-Factor Authentication key for the <see cref="User"/> as an asynchronous
        ///     operation.
        /// </summary>
        /// <param name="user">
        ///     The <see cref="User"/> whose Two-Factor Authentication key to get.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken"/> to observe while waiting for the <see cref="Task{T}"/> to complete.
        /// </param>
        /// <returns>
        ///     The <see cref="Task{T}"/> representing the asynchronous operation.
        ///     The <see cref="Task{T}.Result"/> contains the Two-Factor Authentication key in the form of a
        ///     <see cref="string"/>.
        /// </returns>
        public async Task<string> GetSharedKeyAsync(User user, CancellationToken cancellationToken = default)
        {
            // Check the parameters
            if (user == null) throw new ArgumentNullException(nameof(user));

            // Get the key
            cancellationToken.ThrowIfCancellationRequested();
            string unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);

            return unformattedKey;
        }

        /// <summary>
        ///     Gets the Two-Factor Authenticator <see cref="Uri"/> as an asynchronous operation as an asynchronous
        ///     operation.
        /// </summary>
        /// <param name="user">
        ///     The <see cref="User"/> whose Two-Factor Authenticator URI to get.
        /// </param>
        /// <param name="authenticatorKey">
        ///     The Two-Factor Authentication key.
        /// </param>
        /// <returns>
        ///     The <see cref="Task{T}"/> representing the asynchronous operation.
        ///     The <see cref="Task{T}.Result"/> contains the Two-Factor Authenticator <see cref="Uri"/>.
        /// </returns>
        public async Task<Uri> GetAuthenticatorUriAsync(User user, string authenticatorKey)
        {
            // If the key is empty, reset it and try again
            // ReSharper disable once InvertIf
            //  Means less return statements
            if (string.IsNullOrEmpty(authenticatorKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            // Return the key and URI
            return GenerateQrCodeUri(user.Email, authenticatorKey);
        }

        /// <summary>
        ///     Enables Two-Factor Authentication for the <see cref="User"/>.
        /// </summary>
        /// <param name="user">
        ///     The <see cref="User"/> to enable Two-Factor Authentication for.
        /// </param>
        /// <param name="verificationCode">
        ///     The Two-Factor Authentication code presented to the <see cref="User"/> for verification.
        ///     <para/>
        ///     This is to verify that the Two-Factor Authentication method has been successfully setup.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken"/> to observe while waiting for the <see cref="Task{T}"/> to complete.
        /// </param>
        /// <returns>
        ///     The <see cref="Task{T}"/> representing the asynchronous operation.
        ///     The <see cref="Task{T}.Result"/> contains a <see cref="IReadOnlyCollection{T}"/> of
        ///     <see cref="string"/>s representing the recovery codes.
        /// </returns>
        public async Task<IReadOnlyCollection<string>> EnableTwoFactorAuthenticationAsync(
            User user,
            string verificationCode,
            CancellationToken cancellationToken = default)
        {
            // Check the parameters
            if (user == null) throw new ArgumentNullException(nameof(user), "The User cannot be null.");

            if (string.IsNullOrEmpty(verificationCode))
            {
                throw new ArgumentNullException(
                    nameof(verificationCode),
                    "The Verification Code cannot be null or empty.");
            }

            // Strip spaces and hyphens
            verificationCode = verificationCode
                              .Replace(" ", string.Empty)
                              .Replace("-", string.Empty);

            // Check if the token is valid
            bool is2FaTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                                       user,
                                       _userManager.Options.Tokens.AuthenticatorTokenProvider,
                                       verificationCode);

            if (!is2FaTokenValid)
            {
                throw new InvalidTwoFactorAuthenticationCodeException(
                    verificationCode,
                    "Verification code is invalid.");
            }

            // Attempt to enable 2FA
            cancellationToken.ThrowIfCancellationRequested();
            await _userManager.SetTwoFactorEnabledAsync(user, true);

            // Log
            _logger.LogInformation($"{user.UserName} has enabled 2FA.");

            // Get recovery codes
            IEnumerable<string> recoveryCodes = await GetNewRecoveryCodesAsync(user, cancellationToken);

            return recoveryCodes.ToList().AsReadOnly();
        }

        /// <summary>
        ///     Resets the Two-Factor Authentication key for the <see cref="User"/>.
        /// </summary>
        /// <param name="user">
        ///     The <see cref="User"/> whose Two-Factor Authentication key to reset.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken"/> to observe while waiting for the <see cref="Task{T}"/> to complete.
        /// </param>
        /// <returns>
        ///     The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        public async Task ResetTwoFactorAuthenticationAsync(User user, CancellationToken cancellationToken = default)
        {
            // Check the parameters
            if (user == null) throw new ArgumentNullException(nameof(user), "The User cannot be null.");

            // Disable 2FA
            cancellationToken.ThrowIfCancellationRequested();
            await _userManager.SetTwoFactorEnabledAsync(user, false);

            // Reset the 2FA key
            await _userManager.ResetAuthenticatorKeyAsync(user);

            // Log
            _logger.LogInformation($"{user.UserName} has reset their 2FA key.");
        }

        /// <summary>
        ///     Disables Two-Factor Authentication for the <see cref="User"/>.
        /// </summary>
        /// <param name="user">
        ///     The <see cref="User"/> whose Two-Factor Authentication to disable.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken"/> to observe while waiting for the <see cref="Task{T}"/> to complete.
        /// </param>
        /// <returns>
        ///     The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        public async Task DisableTwoFactorAuthenticationAsync(User user, CancellationToken cancellationToken = default)
        {
            // Check the parameters
            if (user == null) throw new ArgumentNullException(nameof(user), "The User cannot be null.");

            // Attempt to disable 2FA
            cancellationToken.ThrowIfCancellationRequested();
            IdentityResult result = await _userManager.SetTwoFactorEnabledAsync(user, false);

            // Throw an exception if something went wrong
            if (!result.Succeeded)
            {
                throw new IdentityException(
                    result.Errors,
                    $"One or more errors occured when attempting to disable 2FA for {user.UserName}.");
            }

            // Log
            _logger.LogInformation($"{user.UserName} has disabled 2FA.");
        }

        /// <summary>
        ///     Gets a new set of Two-Factor Authentication recovery codes for the <see cref="User"/>.
        /// </summary>
        /// <param name="user">
        ///     The <see cref="User"/> whose Two-Factor Authentication recovery codes to generate.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken"/> to observe while waiting for the <see cref="Task{T}"/> to complete.
        /// </param>
        /// <returns>
        ///     The <see cref="Task{T}"/> representing the asynchronous operation.
        ///     The <see cref="Task{T}.Result"/> contains a <see cref="IReadOnlyCollection{T}"/> of
        ///     <see cref="string"/>s representing the recovery codes.
        /// </returns>
        public async Task<IReadOnlyCollection<string>> GetNewRecoveryCodesAsync(
            User user,
            CancellationToken cancellationToken = default)
        {
            // Check the parameters
            if (user == null) throw new ArgumentNullException(nameof(user), "The User cannot be null.");

            // If 2FA is not enabled, then we shouldn't be here
            if (!user.TwoFactorEnabled)
                throw new Exception($"Cannot generate recovery codes for {user.UserName} because they do not have 2FA enabled.");

            // Generate the recovery codes
            cancellationToken.ThrowIfCancellationRequested();
            IEnumerable<string> recoveryCodes =
                await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

            // Log
            _logger.LogInformation($"{user.UserName} has generated new 2FA recovery codes.");

            return recoveryCodes.ToList().AsReadOnly();
        }

        /// <summary>
        ///     Formats the given shared Two-Factor Authentication key.
        /// </summary>
        /// <param name="unformattedKey">
        ///     The un-formatted Two-Factor Authentication key.
        /// </param>
        /// <returns>
        ///     The formatted Two-Factor Authentication key.
        /// </returns>
        public static string FormatKey(string unformattedKey)
        {
            StringBuilder result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }

            if (currentPosition < unformattedKey.Length) result.Append(unformattedKey.Substring(currentPosition));

            return result.ToString().ToLowerInvariant();
        }

        /// <summary>
        ///     Generates the Two-Factor Authenticator <see cref="Uri"/>.
        /// </summary>
        /// <param name="email">
        ///     The email address of the <see cref="User"/>.
        /// </param>
        /// <param name="unformattedKey">
        ///     The un-formatted shared Two-Factor Authenticator key.
        /// </param>
        /// <returns>
        ///     The Two-Factor Authenticator <see cref="Uri"/>.
        /// </returns>
        private static Uri GenerateQrCodeUri(string email, string unformattedKey)
        {
            UrlEncoder urlEncoder = UrlEncoder.Default;

            return new Uri(
                string.Format(
                    AuthenticatorUriFormat,
                    urlEncoder.Encode("Skybound"),
                    urlEncoder.Encode(email),
                    unformattedKey));
        }
    }
}

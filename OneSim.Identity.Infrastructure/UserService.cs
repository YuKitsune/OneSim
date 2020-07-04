// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserService.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Infrastructure
{
    using System;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    using OneSim.Common.Application.Abstractions;
    using OneSim.Identity.Application.Abstractions;
    using OneSim.Identity.Application.Exceptions;
    using OneSim.Identity.Infrastructure.Extensions;

    using Strato.Persistence.Abstractions;

    /// <summary>
    ///     The implementation of the <see cref="IUserService{TUser}"/> for the <see cref="User"/> using the
    ///     ASP.NET Core <see cref="UserManager{TUser}"/>.
    /// </summary>
    public class UserService : IUserService<User>
    {
        /// <summary>
        ///     The <see cref="UserManager{TUser}"/> for the <see cref="User"/>.
        /// </summary>
        private readonly UserManager<User> _userManager;

        /// <summary>
        ///     The <see cref="IIdentityDbContext{TUser}"/> for the <see cref="User"/>.
        /// </summary>
        private readonly IIdentityDbContext<User> _dbContext;

        /// <summary>
        ///     The <see cref="IEmailSender"/>.
        /// </summary>
        private readonly IEmailSender _emailSender;

        /// <summary>
        ///     The <see cref="ILogger"/> for the <see cref="UserService"/>.
        /// </summary>
        private readonly ILogger<UserService> _logger;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="userManager">
        ///     The <see cref="UserManager{TUser}"/> for the <see cref="User"/>.
        /// </param>
        /// <param name="dbContext">
        ///     The <see cref="IIdentityDbContext{TUser}"/> for the <see cref="User"/>.
        /// </param>
        /// <param name="emailSender">
        ///     The <see cref="IEmailSender"/>.
        /// </param>
        /// <param name="logger">
        ///     The <see cref="ILogger"/> for the <see cref="UserService"/>.
        /// </param>
        public UserService(
            UserManager<User> userManager,
            IIdentityDbContext<User> dbContext,
            IEmailSender emailSender,
            ILogger<UserService> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Gets the <see cref="User"/> from the given <see cref="ClaimsPrincipal"/> as an asynchronous operation.
        /// </summary>
        /// <param name="claimsPrincipal">
        ///     The <see cref="ClaimsPrincipal"/> representing the user.
        /// </param>
        /// <returns>
        ///     The <see cref="Task"/> representing the asynchronous operation.
        ///     The <see cref="Task{T}.Result"/> contains the <see cref="User"/>.
        /// </returns>
        public async Task<User> GetUserAsync(ClaimsPrincipal claimsPrincipal) =>
            await _userManager.GetUserAsync(claimsPrincipal);

        /// <summary>
        ///     Gets a value indicating whether or not the given <paramref name="password"/> is the correct password for
        ///     the <paramref name="user"/> as an asynchronous operation..
        /// </summary>
        /// <param name="user">
        ///     The <see cref="User"/>.
        /// </param>
        /// <param name="password">
        ///     The password to check.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="password"/> is correct, <c>false</c> otherwise.
        /// </returns>
        public async Task<bool> CheckPasswordAsync(User user, string password) =>
            await _userManager.CheckPasswordAsync(user, password);

        /// <summary>
        ///     Registers a new <see cref="User"/> with a password as an asynchronous operation.
        /// </summary>
        /// <param name="user">
        ///     The new <see cref="User"/> to register.
        /// </param>
        /// <param name="password">
        ///     The plain-text password for the <paramref name="user"/>.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken"/> to observe while waiting for the <see cref="Task{T}"/> to complete.
        /// </param>
        /// <returns>
        ///     The <see cref="Task{T}"/> representing the asynchronous operation.
        ///     The <see cref="Task{T}.Result"/> contains the newly registered <see cref="User"/>.
        /// </returns>
        public async Task<User> RegisterUserAsync(
            User user,
            string password,
            CancellationToken cancellationToken = default)
        {
            // Check the parameters
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));

            // Create the user with a password
            cancellationToken.ThrowIfCancellationRequested();
            IdentityResult result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded) _logger.LogInformation($"A new user ({user.UserName}) has registered.");

            result.ThrowIfAnyErrors();

            // Get the user
            User newUser = await _dbContext.Users.FirstOrDefaultAsync(
                               u => u.UserName == user.UserName,
                               cancellationToken);
            if (newUser == null)
            {
                throw new Exception("Failed to fetch user after registering.");
            }

            return newUser;
        }

        /// <summary>
        ///     Sends an email to the <see cref="User"/> allowing them to reset their password.
        /// </summary>
        /// <param name="user">
        ///     The <see cref="User"/> to send the email to.
        /// </param>
        /// <param name="ignoreUnverifiedEmail">
        ///     Whether or not to allow the email to send if the <see cref="User"/>s email address has not been
        ///     verified.
        ///     <para/>
        ///     When set to <c>false</c> (which is the default value), an <see cref="UnverifiedEmailException"/> will be
        ///     thrown if the <paramref name="user"/> has not verified their email address. Otherwise, the email will be
        ///     sent to the unverified email address.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken"/> to observe while waiting for the <see cref="Task{T}"/> to complete.
        /// </param>
        /// <returns>
        ///     The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        public async Task SendPasswordResetEmailAsync(
            User user,
            bool ignoreUnverifiedEmail = false,
            CancellationToken cancellationToken = default)
        {
            // Check the parameters
            if (user == null) throw new ArgumentNullException(nameof(user), "The User cannot be null.");

            // Can't reset a password for an unverified email address
            if (!ignoreUnverifiedEmail &&
                !await _userManager.IsEmailConfirmedAsync(user))
            {
                throw new UnverifiedEmailException(
                    user.Id,
                    user.Email,
                    "Cannot send a Password Reset Email to an unverified email address.");
            }

            // Start a new transaction
            // Todo: Need to test the UserManager properly honours the transaction
            using ITransaction transaction = await _dbContext.BeginTransactionAsync(cancellationToken);

            // Get the reset token
            string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Todo: Get the callback URL
            // string callbackUrl = urlHelper.ResetPasswordCallbackLink(user.Id, resetToken, requestScheme);

            // Send the password reset email
            // Todo: Use HTML email
            _logger.LogInformation($"{user.UserName} has requested a password reset.");
            await _emailSender.SendAsync(
                user.Email,
                "Reset Password",
                $"Reset token: {resetToken}",
                cancellationToken);

            // Commit all changes
            await transaction.CommitAsync(cancellationToken);
        }

        /// <summary>
        ///     Resets the <see cref="User"/>s password to the <paramref name="newPassword"/>.
        /// </summary>
        /// <param name="user">
        ///     The <see cref="User"/> whose password to reset.
        /// </param>
        /// <param name="newPassword">
        ///     The new plain-text password.
        /// </param>
        /// <param name="resetToken">
        ///     The token which was sent in the password reset email.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken"/> to observe while waiting for the <see cref="Task{T}"/> to complete.
        /// </param>
        /// <returns>
        ///     The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        public async Task ResetPasswordAsync(
            User user,
            string newPassword,
            string resetToken,
            CancellationToken cancellationToken = default)
        {
            // Check the parameters
            if (user == null) throw new ArgumentNullException(nameof(user), "The User cannot be null.");

            if (string.IsNullOrEmpty(newPassword))
            {
                throw new ArgumentNullException(nameof(newPassword), "The New Password cannot be null or empty.");
            }

            // Determine whether to set or change the password
            cancellationToken.ThrowIfCancellationRequested();
            IdentityResult result;
            bool hasPassword = await _userManager.HasPasswordAsync(user);
            if (hasPassword)
            {
                // Only need to check the token if we're resetting the password
                if (string.IsNullOrEmpty(resetToken))
                {
                    throw new ArgumentNullException(
                        nameof(resetToken),
                        "The Password Reset Token cannot be null or empty.");
                }

                // Reset the password
                result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
            }
            else
            {
                // Set the password
                result = await _userManager.AddPasswordAsync(user, newPassword);
            }

            // Log some stuff
            if (result.Succeeded) _logger.LogInformation($"{user.UserName} has reset their password.");

            result.ThrowIfAnyErrors();
        }

        /// <summary>
        ///     Updates the <see cref="User"/>s password to the <paramref name="newPassword"/>.
        /// </summary>
        /// <param name="user">
        ///     The <see cref="User"/> whose password to change.
        /// </param>
        /// <param name="oldPassword">
        ///     The <see cref="User"/>s old plain-text password.
        /// </param>
        /// <param name="newPassword">
        ///     The <see cref="User"/>s new plain-text password.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken"/> to observe while waiting for the <see cref="Task{T}"/> to complete.
        /// </param>
        /// <returns>
        ///     The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        public async Task ChangePasswordAsync(
            User user,
            string oldPassword,
            string newPassword,
            CancellationToken cancellationToken = default)
        {
            // Check the parameters
            if (user == null) throw new ArgumentNullException(nameof(user), "The User cannot be null.");

            if (string.IsNullOrEmpty(oldPassword))
            {
                throw new ArgumentNullException(nameof(oldPassword), "The Old Password cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(newPassword))
            {
                throw new ArgumentNullException(nameof(newPassword), "The New Password cannot be null or empty.");
            }

            // Attempt to change the password
            cancellationToken.ThrowIfCancellationRequested();
            IdentityResult result =
                await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);

            // Throw if something went wrong
            result.ThrowIfAnyErrors();

            // Log
            _logger.LogInformation($"{user.UserName} has changed their password.");

            // Todo: Implement auto sign in after password change
            //  this works in production, but unit testing this is a challenge
        }

        /// <summary>
        ///     Sends an email to the <see cref="User"/> allowing them to verify the email address.
        /// </summary>
        /// <param name="user">
        ///     The <see cref="User"/> whose email to verify.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken"/> to observe while waiting for the <see cref="Task{T}"/> to complete.
        /// </param>
        /// <returns>
        ///     The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        public async Task SendVerificationEmailAsync(User user, CancellationToken cancellationToken = default)
        {
            // Check the parameters
            if (user == null) throw new ArgumentNullException(nameof(user), "The User cannot be null.");

            // Start a new transaction
            // Todo: Need to test the UserManager properly honours the transaction
            using (ITransaction transaction = _dbContext.BeginTransaction())
            {
                // Get the confirmation code
                string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                // Todo: Get the callback url
                // string callbackUrl = urlHelper.EmailConfirmationLink(user.Id, code, requestScheme);

                // Send the confirmation email
                // Todo: Use HTML email
                await _emailSender.SendAsync(
                    user.Email,
                    "Confirm Email",
                    $"Confirmation code: {code}",
                    cancellationToken);

                // Commit all changes
                await transaction.CommitAsync(cancellationToken);
            }

            // Log
            _logger.LogInformation($"A verification email has been sent to {user.UserName} at \"{user.Email}\".");
        }

        /// <summary>
        ///     Verifies the <see cref="User"/>s email address.
        /// </summary>
        /// <param name="user">
        ///     The <see cref="User"/> whose email to verify.
        /// </param>
        /// <param name="verificationCode">
        ///     The verification code which was sent in the verification email.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken"/> to observe while waiting for the <see cref="Task{T}"/> to complete.
        /// </param>
        /// <returns>
        ///     The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        public async Task VerifyEmailAsync(
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
                    "The Confirmation Code cannot be null or empty.");
            }

            // Confirm the email address
            cancellationToken.ThrowIfCancellationRequested();
            IdentityResult result = await _userManager.ConfirmEmailAsync(user, verificationCode);

            // Log some stuff
            if (result.Succeeded) _logger.LogInformation($"{user.UserName} has confirmed their email.");

            result.ThrowIfAnyErrors();
        }
    }
}

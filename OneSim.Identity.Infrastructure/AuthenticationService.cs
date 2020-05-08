// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationService.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Infrastructure
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;

    using OneSim.Identity.Application.Abstractions;
    using OneSim.Identity.Infrastructure.Extensions;

    /// <summary>
    ///     The implementation of the <see cref="IAuthenticationService{TUser}"/> for the <see cref="User"/> using the
    ///     ASP.NET Core <see cref="SignInManager{TUser}"/>.
    /// </summary>
    public class AuthenticationService : IAuthenticationService<User>
    {
        /// <summary>
        ///     The <see cref="SignInManager{TUser}"/> for the <see cref="User"/>.
        /// </summary>
        private readonly SignInManager<User> _signInManager;

        /// <summary>
        ///     The <see cref="ILogger"/> for the <see cref="AuthenticationService"/>.
        /// </summary>
        private readonly ILogger<AuthenticationService> _logger;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AuthenticationService"/> class.
        /// </summary>
        /// <param name="signInManager">
        ///     The <see cref="SignInManager{TUser}"/> for the <see cref="User"/>.
        /// </param>
        /// <param name="logger">
        ///     The <see cref="ILogger{TCategoryName}"/> for the <see cref="AuthenticationService"/>.
        /// </param>
        public AuthenticationService(SignInManager<User> signInManager, ILogger<AuthenticationService> logger)
        {
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Attempts to sign the <see cref="User"/> in with their password as an asynchronous operation.
        /// </summary>
        /// <param name="user">
        ///     The <see cref="User"/> to sign-in.
        /// </param>
        /// <param name="password">
        ///     The plain-text password.
        /// </param>
        /// <param name="isPersistent">
        ///     Whether the sign-in cookie should persist after the browser is closed.
        /// </param>
        /// <param name="lockoutOnFailure">
        ///     Whether the user account should be locked if the sign in fails after too many attempts.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken"/> to observe while waiting for the <see cref="Task{T}"/> to complete.
        /// </param>
        /// <returns>
        ///     The <see cref="Task{T}"/> representing the asynchronous operation.
        ///     The <see cref="Task{T}.Result"/> contains an <see cref="ISignInResult"/> for the sign-in attempt.
        /// </returns>
        public async Task<ISignInResult> SignInAsync(
            User user,
            string password,
            bool isPersistent = false,
            bool lockoutOnFailure = true,
            CancellationToken cancellationToken = default)
        {
            // Check the parameters
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));

            // Attempt to sign-in
            cancellationToken.ThrowIfCancellationRequested();
            SignInResult result = await _signInManager.PasswordSignInAsync(
                                      user,
                                      password,
                                      isPersistent,
                                      lockoutOnFailure);

            // Log
            _logger.LogResult(user, result);

            return new InternalSignInResult(result);
        }

        /// <summary>
        ///     Attempts to sign the <see cref="User"/> out.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        public async Task SignOutAsync() => await _signInManager.SignOutAsync();
    }
}

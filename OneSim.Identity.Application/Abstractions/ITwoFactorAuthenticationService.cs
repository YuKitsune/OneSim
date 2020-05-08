// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITwoFactorAuthenticationService.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Application.Abstractions
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using OneSim.Identity.Domain;

    /// <summary>
    ///     The interface representing a service capable of providing two-factor authentication mechanisms.
    /// </summary>
    /// <typeparam name="TUser">
    ///     The type of <see cref="IUser"/>.
    /// </typeparam>
    public interface ITwoFactorAuthenticationService<TUser>
        where TUser : IUser
    {
        /// <summary>
        ///     Attempts to sign the <typeparamref name="TUser"/> in with a Two-Factor Authentication code as an
        ///     asynchronous operation.
        /// </summary>
        /// <param name="user">
        ///     The <typeparamref name="TUser"/> to sign-in.
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
        Task<ISignInResult> TwoFactorSignInAsync(
            TUser user,
            string code,
            bool isPersistent = false,
            bool rememberClient = false,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Attempts to sign the <typeparamref name="TUser"/> in with a Two-Factor Authentication recovery code as
        ///     an asynchronous operation.
        /// </summary>
        /// <param name="user">
        ///     The <typeparamref name="TUser"/> to sign-in.
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
        Task<ISignInResult> RecoveryCodeSignInAsync(
            TUser user,
            string code,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Gets the <typeparamref name="TUser"/> for the current Two-Factor Authentication login attempt as an
        ///     asynchronous operation.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task{T}"/> representing the asynchronous operation.
        ///     The <see cref="Task{T}.Result"/> contains the <typeparamref name="TUser"/>.
        /// </returns>
        Task<TUser> GetTwoFactorAuthenticationUserAsync();

        /// <summary>
        ///     Gets the shared Two-Factor Authentication key for the <typeparamref name="TUser"/> as an asynchronous
        ///     operation.
        /// </summary>
        /// <param name="user">
        ///     The <typeparamref name="TUser"/> whose Two-Factor Authentication key to get.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken"/> to observe while waiting for the <see cref="Task{T}"/> to complete.
        /// </param>
        /// <returns>
        ///     The <see cref="Task{T}"/> representing the asynchronous operation.
        ///     The <see cref="Task{T}.Result"/> contains the Two-Factor Authentication key in the form of a
        ///     <see cref="string"/>.
        /// </returns>
        Task<string> GetSharedKeyAsync(TUser user, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Gets the Two-Factor Authenticator <see cref="Uri"/> as an asynchronous operation as an asynchronous
        ///     operation.
        /// </summary>
        /// <param name="user">
        ///     The <typeparamref name="TUser"/> whose Two-Factor Authenticator URI to get.
        /// </param>
        /// <param name="authenticatorKey">
        ///     The Two-Factor Authentication key.
        /// </param>
        /// <returns>
        ///     The <see cref="Task{T}"/> representing the asynchronous operation.
        ///     The <see cref="Task{T}.Result"/> contains the Two-Factor Authenticator <see cref="Uri"/>.
        /// </returns>
        Task<Uri> GetAuthenticatorUriAsync(TUser user, string authenticatorKey);

        /// <summary>
        ///     Enables Two-Factor Authentication for the <typeparamref name="TUser"/> as an asynchronous operation.
        /// </summary>
        /// <param name="user">
        ///     The <typeparamref name="TUser"/> to enable Two-Factor Authentication for.
        /// </param>
        /// <param name="verificationCode">
        ///     The Two-Factor Authentication code presented to the <typeparamref name="TUser"/> for verification.
        ///     <para/>
        ///     This is to verify that the Two-Factor Authentication method has been successfully setup.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken"/> to observe while waiting for the <see cref="Task{T}"/> to complete.
        /// </param>
        /// <returns>
        ///     The <see cref="Task{T}"/> representing the asynchronous operation.
        ///     The <see cref="Task{T}.Result"/> contains an <see cref="IReadOnlyCollection{T}"/> of
        ///     <see cref="string"/>s representing the recovery codes.
        /// </returns>
        Task<IReadOnlyCollection<string>> EnableTwoFactorAuthenticationAsync(
            TUser user,
            string verificationCode,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Resets the Two-Factor Authentication key for the <typeparamref name="TUser"/> as an asynchronous
        ///     operation.
        /// </summary>
        /// <param name="user">
        ///     The <typeparamref name="TUser"/> whose Two-Factor Authentication key to reset.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken"/> to observe while waiting for the <see cref="Task{T}"/> to complete.
        /// </param>
        /// <returns>
        ///     The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        Task ResetTwoFactorAuthenticationAsync(TUser user, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Disables Two-Factor Authentication for the <typeparamref name="TUser"/> as an asynchronous operation.
        /// </summary>
        /// <param name="user">
        ///     The <typeparamref name="TUser"/> whose Two-Factor Authentication to disable.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken"/> to observe while waiting for the <see cref="Task{T}"/> to complete.
        /// </param>
        /// <returns>
        ///     The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        Task DisableTwoFactorAuthenticationAsync(TUser user, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Gets a new set of Two-Factor Authentication recovery codes for the <typeparamref name="TUser"/> as an
        ///     asynchronous operation.
        /// </summary>
        /// <param name="user">
        ///     The <typeparamref name="TUser"/> whose Two-Factor Authentication recovery codes to generate.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken"/> to observe while waiting for the <see cref="Task{T}"/> to complete.
        /// </param>
        /// <returns>
        ///     The <see cref="Task{T}"/> representing the asynchronous operation.
        ///     The <see cref="Task{T}.Result"/> contains a <see cref="IReadOnlyCollection{T}"/> of
        ///     <see cref="string"/>s representing the recovery codes.
        /// </returns>
        Task<IReadOnlyCollection<string>> GetNewRecoveryCodesAsync(
            TUser user,
            CancellationToken cancellationToken = default);
    }
}

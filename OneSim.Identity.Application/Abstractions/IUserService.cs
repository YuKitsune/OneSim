// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUserService.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Application.Abstractions
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;

    using OneSim.Identity.Application.Exceptions;
    using OneSim.Identity.Domain;

    /// <summary>
    ///     The interface representing a service capable of managing <see cref="IUser"/>s.
    /// </summary>
    /// <typeparam name="TUser">
    ///     The type of <see cref="IUser"/>.
    /// </typeparam>
    public interface IUserService<TUser>
        where TUser : IUser
    {
        /// <summary>
        ///     Gets the <typeparamref name="TUser"/> from the given <see cref="ClaimsPrincipal"/> as an asynchronous
        ///     operation.
        /// </summary>
        /// <param name="claimsPrincipal">
        ///     The <see cref="ClaimsPrincipal"/> representing the user.
        /// </param>
        /// <returns>
        ///     The <see cref="Task{T}"/> representing the asynchronous operation.
        ///     The <see cref="Task{T}.Result"/> contains the <typeparamref name="TUser"/>.
        /// </returns>
        Task<TUser> GetUserAsync(ClaimsPrincipal claimsPrincipal);

        /// <summary>
        ///     Gets a value indicating whether or not the given <paramref name="password"/> is the correct password for
        ///     the <paramref name="user"/> as an asynchronous operation.
        /// </summary>
        /// <param name="user">
        ///     The <typeparamref name="TUser"/>.
        /// </param>
        /// <param name="password">
        ///     The password to check.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="password"/> is correct, <c>false</c> otherwise.
        /// </returns>
        Task<bool> CheckPasswordAsync(TUser user, string password);

        /// <summary>
        ///     Registers a new <typeparamref name="TUser"/> with a password as an asynchronous operation.
        /// </summary>
        /// <param name="user">
        ///     The new <typeparamref name="TUser"/> to register.
        /// </param>
        /// <param name="password">
        ///     The plain-text password for the <paramref name="user"/>.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken"/> to observe while waiting for the <see cref="Task{T}"/> to complete.
        /// </param>
        /// <returns>
        ///     The <see cref="Task{T}"/> representing the asynchronous operation.
        ///     The <see cref="Task{T}.Result"/> contains the newly registered <typeparamref name="TUser"/>.
        /// </returns>
        Task<TUser> RegisterUserAsync(TUser user, string password, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Sends an email to the <typeparamref name="TUser"/> allowing them to reset their password as an
        ///     asynchronous operation.
        /// </summary>
        /// <param name="user">
        ///     The <typeparamref name="TUser"/> to send the email to.
        /// </param>
        /// <param name="ignoreUnverifiedEmail">
        ///     Whether or not to allow the email to send if the <typeparamref name="TUser"/>s email address has not
        ///     been verified.
        ///     <para/>
        ///     When set to <c>false</c> (which is the default value), an <see cref="UnverifiedEmailException"/> will be
        ///     thrown if the <paramref name="user"/> has not verified their email address. Otherwise, the email will be
        ///     sent to the unverified email address. Be cautious when using this, as sending a password reset email to
        ///     an unverified email address poses a potential security risk.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken"/> to observe while waiting for the <see cref="Task{T}"/> to complete.
        /// </param>
        /// <returns>
        ///     The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        Task SendPasswordResetEmailAsync(
            TUser user,
            bool ignoreUnverifiedEmail = false,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Resets the <typeparamref name="TUser"/>s password to the <paramref name="newPassword"/> as an
        ///     asynchronous operation.
        /// </summary>
        /// <param name="user">
        ///     The <typeparamref name="TUser"/> whose password to reset.
        /// </param>
        /// <param name="newPassword">
        ///     The new plain-text password.
        /// </param>
        /// <param name="resetToken">
        ///     The password reset token which was sent in the password reset email.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken"/> to observe while waiting for the <see cref="Task{T}"/> to complete.
        /// </param>
        /// <returns>
        ///     The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        Task ResetPasswordAsync(
            TUser user,
            string newPassword,
            string resetToken,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Updates the <typeparamref name="TUser"/>s password to the <paramref name="newPassword"/> as an
        ///     asynchronous operation.
        /// </summary>
        /// <param name="user">
        ///     The <typeparamref name="TUser"/> whose password to change.
        /// </param>
        /// <param name="oldPassword">
        ///     The <typeparamref name="TUser"/>s old plain-text password.
        /// </param>
        /// <param name="newPassword">
        ///     The <typeparamref name="TUser"/>s new plain-text password.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken"/> to observe while waiting for the <see cref="Task{T}"/> to complete.
        /// </param>
        /// <returns>
        ///     The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        Task ChangePasswordAsync(
            TUser user,
            string oldPassword,
            string newPassword,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Sends an email to the <typeparamref name="TUser"/> allowing them to verify the email address as an
        ///     asynchronous operation.
        /// </summary>
        /// <param name="user">
        ///     The <typeparamref name="TUser"/> whose email to verify.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken"/> to observe while waiting for the <see cref="Task{T}"/> to complete.
        /// </param>
        /// <returns>
        ///     The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        Task SendVerificationEmailAsync(TUser user, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Verifies the <typeparamref name="TUser"/>s email address as an asynchronous operation.
        /// </summary>
        /// <param name="user">
        ///     The <typeparamref name="TUser"/> whose email to verify.
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
        Task VerifyEmailAsync(TUser user, string verificationCode, CancellationToken cancellationToken = default);
    }
}

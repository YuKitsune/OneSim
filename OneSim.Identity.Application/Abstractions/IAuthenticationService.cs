// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuthenticationService.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Application.Abstractions
{
    using System.Threading;
    using System.Threading.Tasks;

    using OneSim.Identity.Domain;

    /// <summary>
    ///     The interface representing a service providing authentication mechanisms for <see cref="IUser"/>s.
    /// </summary>
    /// <typeparam name="TUser">
    ///     The type of <see cref="IUser"/>.
    /// </typeparam>
    public interface IAuthenticationService<TUser>
        where TUser : IUser
    {
        /// <summary>
        ///     Attempts to sign the <typeparamref name="TUser"/> in with their password as an asynchronous operation.
        /// </summary>
        /// <param name="user">
        ///     The <typeparamref name="TUser"/> to sign-in.
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
        Task<ISignInResult> SignInAsync(
            TUser user,
            string password,
            bool isPersistent = false,
            bool lockoutOnFailure = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Attempts to sign the <typeparamref name="TUser"/> out as an asynchronous operation.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        Task SignOutAsync();
    }
}

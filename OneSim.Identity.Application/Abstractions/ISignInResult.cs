// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISignInResult.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Application.Abstractions
{
    /// <summary>
    ///     The interface representing a result from an attempt to sign-in.
    /// </summary>
    public interface ISignInResult
    {
        /// <summary>
        ///     Gets a value indicating whether or not the sign-in was successful.
        /// </summary>
        bool Succeeded { get; }

        /// <summary>
        ///     Gets a value indicating whether or not the user attempting to sign-in is locked out.
        /// </summary>
        bool IsLockedOut { get; }

        /// <summary>
        ///     Gets a value indicating whether or not the user attempting to sign-in is not allowed to sign-in.
        /// </summary>
        bool IsNotAllowed { get; }

        /// <summary>
        ///     Gets a value indicating whether or not the user attempting to sign-in requires two factor authentication.
        /// </summary>
        bool RequiresTwoFactor { get; }
    }
}

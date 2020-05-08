// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InternalSignInResult.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Infrastructure
{
    using System;

    using Microsoft.AspNetCore.Identity;

    using OneSim.Identity.Application.Abstractions;

    /// <summary>
    ///     The internal <see cref="SignInResult"/> wrapper.
    /// </summary>
    internal sealed class InternalSignInResult : ISignInResult
    {
        /// <summary>
        ///     The <see cref="SignInResult"/>.
        /// </summary>
        private readonly SignInResult _result;

        /// <summary>
        ///     Gets a value indicating whether or not the sign-in was successful.
        /// </summary>
        public bool Succeeded => _result.Succeeded;

        /// <summary>
        ///     Gets a value indicating whether or not the user attempting to sign-in is locked out.
        /// </summary>
        public bool IsLockedOut => _result.IsLockedOut;

        /// <summary>
        ///     Gets a value indicating whether or not the user attempting to sign-in is not allowed to sign-in.
        /// </summary>
        public bool IsNotAllowed => _result.IsNotAllowed;

        /// <summary>
        ///     Gets a value indicating whether or not the user attempting to sign-in requires two factor authentication.
        /// </summary>
        public bool RequiresTwoFactor => _result.RequiresTwoFactor;

        /// <summary>
        ///     Initializes a new instance of the <see cref="InternalSignInResult"/> class.
        /// </summary>
        /// <param name="result">
        ///     The <see cref="SignInResult"/>.
        /// </param>
        public InternalSignInResult(SignInResult result) =>
            _result = result ?? throw new ArgumentNullException(nameof(result));
    }
}

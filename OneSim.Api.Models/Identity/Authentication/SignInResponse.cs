// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SignInResponse.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Api.Models.Identity.Authentication
{
    /// <summary>
    ///     The class representing a Sign-In response.
    /// </summary>
    public class SignInResponse
    {
        /// <summary>
        ///     Gets or sets a value indicating whether or not Two-Factor Authentication is required.
        /// </summary>
        public bool TwoFactorAuthenticationRequired { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the user has been locked out of their account.
        /// </summary>
        public bool LockedOut { get; set; }

        /// <summary>
        ///     Gets or sets the token to use for authentication future requests.
        /// </summary>
        public string Token { get; set; }
    }
}

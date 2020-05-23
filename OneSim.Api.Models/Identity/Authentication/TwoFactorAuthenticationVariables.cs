// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TwoFactorAuthenticationVariables.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Api.Models.Identity.Authentication
{
    // Todo: Rename this because it's shit.

    /// <summary>
    ///     The class which contains the variables required to enable Two-Factor Authentication.
    /// </summary>
    public class TwoFactorAuthenticationVariables
    {
        /// <summary>
        ///     Gets or sets the shared key.
        /// </summary>
        public string SharedKey { get; set; }

        /// <summary>
        ///     Gets or sets the authenticator URI.
        /// </summary>
        public string AuthenticatorUri { get; set; }
    }
}

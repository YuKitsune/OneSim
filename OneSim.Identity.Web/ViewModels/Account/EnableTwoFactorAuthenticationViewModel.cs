// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnableTwoFactorAuthenticationViewModel.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Web.ViewModels.Account
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Mvc.ModelBinding;

    /// <summary>
    ///     The View Model to use when enabling Two-Factor Authentication.
    /// </summary>
    public class EnableTwoFactorAuthenticationViewModel
    {
        /// <summary>
        ///     Gets or sets the verification code.
        /// </summary>
        [Required, DataType(DataType.Text)]
        public string VerificationCode { get; set; }

        /// <summary>
        ///     Gets or sets the shared key.
        /// </summary>
        [BindNever]
        public string SharedKey { get; set; }

        /// <summary>
        ///     Gets or sets the authenticator URI.
        /// </summary>
        [BindNever]
        public string AuthenticatorUri { get; set; }
    }
}

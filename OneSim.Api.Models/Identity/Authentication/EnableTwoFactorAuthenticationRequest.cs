// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnableTwoFactorAuthenticationRequest.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Api.Models.Identity.Authentication
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     The class representing a request to enable Two-Factor Authentication.
    /// </summary>
    public class EnableTwoFactorAuthenticationRequest : TwoFactorAuthenticationVariables
    {
        /// <summary>
        ///     Gets or sets the verification code.
        /// </summary>
        [Required, DataType(DataType.Text)]
        public string VerificationCode { get; set; }
    }
}

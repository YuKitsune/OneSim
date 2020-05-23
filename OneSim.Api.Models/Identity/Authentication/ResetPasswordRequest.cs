// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResetPasswordRequest.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Api.Models.Identity.Authentication
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     The class representing a Password Reset request.
    /// </summary>
    public class ResetPasswordRequest : AdjustPasswordRequest
    {
        /// <summary>
        ///     Gets or sets the email address.
        /// </summary>
        [Required, EmailAddress]
        public string Email { get; set; }

        /// <summary>
        ///     Gets or sets the password reset token.
        /// </summary>
        [Required]
        public string ResetToken { get; set; }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ForgotPasswordRequest.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Api.Models.Identity.Authentication
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     The class representing a Password Reset Email request.
    /// </summary>
    public class ForgotPasswordRequest
    {
        /// <summary>
        ///     Gets or sets the email address.
        /// </summary>
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SignInRequest.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Api.Models.Identity.Authentication
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     The class representing a Sign-In Request.
    /// </summary>
    public class SignInRequest
    {
        /// <summary>
        ///     Gets or sets the email address.
        /// </summary>
        [Required, EmailAddress]
        public string Email { get; set; }

        /// <summary>
        ///     Gets or sets the password.
        /// </summary>
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not to set the "remember me" token.
        /// </summary>
        public bool RememberMe { get; set; }
    }
}

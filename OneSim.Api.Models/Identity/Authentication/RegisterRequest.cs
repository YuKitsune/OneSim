// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegisterRequest.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Api.Models.Identity.Authentication
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     The class representing a Registration request.
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        ///     Gets or sets the email address.
        /// </summary>
        [Required, EmailAddress]
        public string Email { get; set; }

        /// <summary>
        ///     Gets or sets the username.
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        ///     Gets or sets the password.
        /// </summary>
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

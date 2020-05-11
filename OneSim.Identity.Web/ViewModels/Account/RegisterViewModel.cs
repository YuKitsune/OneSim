// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegisterViewModel.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Web.ViewModels.Account
{
    using System.ComponentModel.DataAnnotations;

    // Todo: Refine fields

    /// <summary>
    ///     The View Model to use when registering a new user.
    /// </summary>
    public class RegisterViewModel
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

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginViewModel.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Web.ViewModels.Account
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     The Login View Model.
    /// </summary>
    public class LoginViewModel
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
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        /// <summary>
        ///     Gets or sets the URL to direct the user back to after login.
        /// </summary>
        public string CallbackUri { get; set; }

        /// <summary>
        ///     Gets or sets the message.
        /// </summary>
        public string Message { get; set; }
    }
}

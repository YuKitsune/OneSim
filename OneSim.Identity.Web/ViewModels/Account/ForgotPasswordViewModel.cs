// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ForgotPasswordViewModel.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Web.ViewModels.Account
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     The View Model to use for sending a password reset email.
    /// </summary>
    public class ForgotPasswordViewModel
    {
        /// <summary>
        ///     Gets or sets the email address.
        /// </summary>
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}

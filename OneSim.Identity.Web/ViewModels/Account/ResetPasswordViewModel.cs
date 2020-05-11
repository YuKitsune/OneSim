// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResetPasswordViewModel.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Web.ViewModels.Account
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     The View Model to use when resetting a password.
    /// </summary>
    public class ResetPasswordViewModel : PasswordAdjustmentViewModel
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

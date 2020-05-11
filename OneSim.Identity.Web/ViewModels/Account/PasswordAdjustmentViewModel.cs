// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PasswordAdjustmentViewModel.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Web.ViewModels.Account
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     The base View Model for creating adjustments to passwords.
    /// </summary>
    public abstract class PasswordAdjustmentViewModel
    {
        /// <summary>
        ///     Gets or sets the new password.
        /// </summary>
        [Required, DataType(DataType.Password)]
        public string NewPassword { get; set; }

        /// <summary>
        ///     Gets or sets the confirmed password.
        /// </summary>
        [DataType(DataType.Password),
         Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}

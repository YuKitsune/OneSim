// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangePasswordViewModel.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Web.ViewModels.Account
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     The View Model to use when changing a password.
    /// </summary>
    public class ChangePasswordViewModel : PasswordAdjustmentViewModel
    {
        /// <summary>
        ///     Gets or sets the old password.
        /// </summary>
        [Required, DataType(DataType.Password)]
        public string OldPassword { get; set; }
    }
}

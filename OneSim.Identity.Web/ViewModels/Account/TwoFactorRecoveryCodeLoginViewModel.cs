// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TwoFactorRecoveryCodeLoginViewModel.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Web.ViewModels.Account
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     The Two-Factor Authentication Recovery Code login View Model.
    /// </summary>
    public class TwoFactorRecoveryCodeLoginViewModel
    {
        /// <summary>
        ///     Gets or sets the recovery code.
        /// </summary>
        [Required,
         DataType(DataType.Text),
         Display(Name = "Recovery Code")]
        public string RecoveryCode { get; set; }

        /// <summary>
        ///     Gets or sets the URL to direct the user back to after login.
        /// </summary>
        public string CallbackUri { get; set; }
    }
}

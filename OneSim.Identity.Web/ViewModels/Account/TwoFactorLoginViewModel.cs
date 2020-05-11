// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TwoFactorLoginViewModel.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Web.ViewModels.Account
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     The Two-Factor Authentication Login View Model.
    /// </summary>
    public class TwoFactorLoginViewModel
    {
        /// <summary>
        ///     Gets or sets the two-factor authentication code.
        /// </summary>
        [Required,
         StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6),
         DataType(DataType.Text),
         Display(Name = "Authenticator code")]
        public string TwoFactorCode { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not to remember the machine.
        /// </summary>
        [Display(Name = "Remember this machine")]
        public bool RememberMachine { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not to store a remember me token.
        /// </summary>
        public bool RememberMe { get; set; }

        /// <summary>
        ///     Gets or sets the URL to direct the user back to after login.
        /// </summary>
        public string CallbackUri { get; set; }
    }
}

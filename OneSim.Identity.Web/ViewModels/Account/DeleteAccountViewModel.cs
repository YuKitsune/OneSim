// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteAccountViewModel.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Web.ViewModels.Account
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     The View Model to use when deleting an account.
    /// </summary>
    public class DeleteAccountViewModel
    {
        /// <summary>
        ///     Gets or sets the password.
        /// </summary>
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

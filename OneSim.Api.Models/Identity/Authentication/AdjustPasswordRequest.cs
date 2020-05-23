// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdjustPasswordRequest.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Api.Models.Identity.Authentication
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     The class representing a generic request to adjust a password.
    /// </summary>
    public abstract class AdjustPasswordRequest
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

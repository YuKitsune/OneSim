// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangePasswordRequest.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Api.Models.Identity.Authentication
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     The class representing a Change Password request.
    /// </summary>
    public class ChangePasswordRequest : AdjustPasswordRequest
    {
        /// <summary>
        ///     Gets or sets the old password.
        /// </summary>
        [Required, DataType(DataType.Password)]
        public string OldPassword { get; set; }
    }
}

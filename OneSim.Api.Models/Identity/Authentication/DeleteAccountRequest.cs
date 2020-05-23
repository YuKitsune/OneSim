// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteAccountRequest.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Api.Models.Identity.Authentication
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     The class representing an Account Deletion request.
    /// </summary>
    public class DeleteAccountRequest
    {
        /// <summary>
        ///     Gets or sets the password.
        /// </summary>
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

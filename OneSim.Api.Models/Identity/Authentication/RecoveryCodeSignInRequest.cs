// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecoveryCodeSignInRequest.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Api.Models.Identity.Authentication
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     The class representing a Two-Factor Authentication recovery code sign-in request.
    /// </summary>
    public class RecoveryCodeSignInRequest
    {
        /// <summary>
        ///     Gets or sets the recovery code.
        /// </summary>
        [Required,
         DataType(DataType.Text)]
        public string RecoveryCode { get; set; }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteAccountResponse.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Api.Models.Identity.Authentication
{
    /// <summary>
    ///     The class representing an Account Deletion response.
    /// </summary>
    public class DeleteAccountResponse
    {
        // Todo: Need more info than this.

        /// <summary>
        ///     Gets or sets a value indicating whether or not the account was deleted.
        /// </summary>
        public bool Deleted { get; set; }
    }
}

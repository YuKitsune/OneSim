// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnableTwoFactorAuthenticationResponse.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Api.Models.Identity.Authentication
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    ///     The class representing a response from enabling Two-Factor Authentication.
    /// </summary>
    public class EnableTwoFactorAuthenticationResponse
    {
        /// <summary>
        ///     Gets the <see cref="ICollection{T}"/> of <see cref="string"/>s representing Two-Factor Authentication
        ///     recovery codes.
        /// </summary>
        public ICollection<string> RecoveryCodes { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EnableTwoFactorAuthenticationResponse"/> class.
        /// </summary>
        /// <param name="recoveryCodes">
        ///     The <see cref="ICollection{T}"/> of <see cref="string"/>s representing Two-Factor Authentication
        ///     recovery codes.
        /// </param>
        [JsonConstructor]
        public EnableTwoFactorAuthenticationResponse(ICollection<string> recoveryCodes = null) =>
            RecoveryCodes = recoveryCodes ?? new List<string>();
    }
}

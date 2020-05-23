// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JwtSettings.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Api.Configuration
{
    /// <summary>
    ///     The JSON Web Token settings.
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        ///     Gets or sets the JWT secret.
        /// </summary>
        public string Secret { get; set; }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITokenService.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Application.Abstractions
{
    using System;

    using Microsoft.IdentityModel.Tokens;

    using OneSim.Identity.Domain;

    /// <summary>
    ///     The interface representing a service capable of creating tokens for users.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        ///     Gets a new <see cref="SecurityToken"/> for the given <see cref="IUser"/>.
        /// </summary>
        /// <param name="user">
        ///     The <see cref="IUser"/> who the JWT will be issued to.
        /// </param>
        /// <param name="expiryDate">
        ///     The <see cref="DateTimeOffset"/> at which the JWT should expire.
        /// </param>
        /// <returns>
        ///     The <see cref="SecurityToken"/>.
        /// </returns>
        SecurityToken GetToken(IUser user, DateTimeOffset expiryDate);
    }
}

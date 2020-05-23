// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonWebTokenService.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Infrastructure
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    using Microsoft.IdentityModel.Tokens;

    using OneSim.Identity.Application.Abstractions;
    using OneSim.Identity.Domain;

    /// <summary>
    ///     The JWT (JSON Web Token) implementation of the <see cref="ITokenService"/>.
    /// </summary>
    public class JsonWebTokenService : ITokenService
    {
        /// <summary>
        ///     The JWT secret.
        /// </summary>
        private readonly string _secret;

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsonWebTokenService"/> class.
        /// </summary>
        /// <param name="secret">
        ///     The JWT secret.
        /// </param>
        public JsonWebTokenService(string secret)
        {
            if (string.IsNullOrEmpty(secret)) throw new ArgumentNullException(nameof(secret));

            _secret = secret;
        }

        /// <summary>
        ///     Gets a new JSON Web Token for the given <see cref="IUser"/>.
        /// </summary>
        /// <param name="user">
        ///     The <see cref="IUser"/> who the JWT will be issued to.
        /// </param>
        /// <param name="expiryDate">
        ///     The <see cref="DateTimeOffset"/> at which the JWT should expire.
        /// </param>
        /// <returns>
        ///     The JWT in the form of a <see cref="SecurityToken"/>.
        /// </returns>
        public SecurityToken GetToken(IUser user, DateTimeOffset expiryDate)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_secret);
            SecurityTokenDescriptor tokenDescriptor =
                new SecurityTokenDescriptor
                {
                    Subject =
                        new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.Id) }),
                    Expires = expiryDate.DateTime,
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };
            return tokenHandler.CreateToken(tokenDescriptor);
        }
    }
}

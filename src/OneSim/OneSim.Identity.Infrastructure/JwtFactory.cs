using System;

namespace OneSim.Identity.Infrastructure
{
    using OneSim.Identity.Application.Interfaces;
    using OneSim.Identity.Domain.Entities;

    /// <summary>
    ///     The JSON Web Token Factory.
    /// </summary>
    public class JwtFactory : ITokenFactory
    {
        /// <summary>
        ///     Generates a JSON Web Token for the provided <see cref="ApplicationUser"/>.
        /// </summary>
        /// <param name="user">
        ///     The <see cref="ApplicationUser"/>.
        /// </param>
        /// <returns>
        ///     The JSON Web Token in the form of a string.
        /// </returns>
        public string GenerateToken(ApplicationUser user)
        {
            throw new NotImplementedException();
        }
    }
}
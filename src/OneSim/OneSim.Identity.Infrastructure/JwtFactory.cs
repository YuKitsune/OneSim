namespace OneSim.Identity.Infrastructure
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;

    using OneSim.Identity.Application.Abstractions;
    using OneSim.Identity.Domain;
    using OneSim.Identity.Domain.Entities;

    /// <summary>
    ///     The JSON Web Token Factory.
    /// </summary>
    public class JwtFactory : ITokenFactory
    {
        /// <summary>
        ///     Gets the <see cref="TokenSettings"/>.
        /// </summary>
        public TokenSettings Settings { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JwtFactory"/>.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="TokenSettings"/>.
        /// </param>
        public JwtFactory(TokenSettings settings) => Settings = settings ?? throw new ArgumentNullException(nameof(settings), "The TokenSettings cannot be null.");

        /// <summary>
        ///     Initializes a new instance of the <see cref="JwtFactory"/>.
        /// </summary>
        /// <param name="configuration">
        ///     The <see cref="IConfiguration"/> containing the <see cref="TokenSettings"/>.
        /// </param>
        public JwtFactory(IConfiguration configuration)
        {
            TokenSettings settings = configuration.GetSection("TokenSettings").Get<TokenSettings>();
            Settings = settings ??
                       throw new ArgumentNullException(nameof(settings), "Couldn't find the \"TokenSettings\" section in the configuration.");
        }

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
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] secret = Encoding.ASCII.GetBytes(Settings.Secret);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
                                                      {
                                                          Subject = new ClaimsIdentity(new[]
                                                                                       {
                                                                                           // Todo: Check this is correct
                                                                                           new Claim(ClaimTypes.Name, user.Id)
                                                                                       }),

                                                          // Todo: See if we can remove the expiry date from the JWT. Or maybe have it user defined?
                                                          Expires = DateTime.UtcNow.AddMonths(12),
                                                          SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256Signature)
                                                      };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            string tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }
    }
}
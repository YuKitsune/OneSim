namespace OneSim.Identity.Application
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;
    using OneSim.Identity.Application.Interfaces;
    using OneSim.Identity.Domain;
    using OneSim.Identity.Domain.Entities;
    using OneSim.Identity.Domain.Exceptions;

    public class AuthenticationService
    {
        /// <summary>
        ///     The <see cref="IIdentityDbContext"/>.
        /// </summary>
        private IIdentityDbContext _dbContext;

        /// <summary>
        ///     The <see cref="SignInManager{TUser}"/> for the <see cref="ApplicationUser"/>.
        /// </summary>
        private SignInManager<ApplicationUser> _signInManager;

        /// <summary>
        ///     The <see cref="TokenSettings"/>.
        /// </summary>
        private TokenSettings _tokenSettings;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AuthenticationService"/> class.
        /// </summary>
        /// <param name="dbContext">
        ///     The <see cref="IIdentityDbContext"/>.
        /// </param>
        /// <param name="signInManager">
        ///     The <see cref="SignInManager{TUser}"/> for the <see cref="ApplicationUser"/>.
        /// </param>
        /// <param name="tokenSettings">
        ///     The <see cref="TokenSettings"/>.
        /// </param>
        public AuthenticationService(IIdentityDbContext dbContext, SignInManager<ApplicationUser> signInManager, TokenSettings tokenSettings)
        {
            _dbContext = dbContext;
            _signInManager = signInManager;
            _tokenSettings = tokenSettings;
        }
        
        /// <summary>
        ///     Gets the <see cref="SecurityToken"/> for the user.
        /// </summary>
        /// <param name="username">
        ///     The <see cref="ApplicationUser.UserName"/>.
        /// </param>
        /// <param name="password">
        ///     The un-encrypted password for the <see cref="ApplicationUser"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="SecurityToken"/>.
        /// </returns>
        public async Task<SecurityToken> GetToken(string username, string password)
        {
            // Get the user
            ApplicationUser user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == username);

            return await GetToken(user, password);
        }
        
        /// <summary>
        ///     Gets the <see cref="SecurityToken"/> for the user.
        /// </summary>
        /// <param name="user">
        ///     The <see cref="ApplicationUser"/>.
        /// </param>
        /// <param name="password">
        ///     The un-encrypted password for the <see cref="ApplicationUser"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="SecurityToken"/>.
        /// </returns>
        public async Task<SecurityToken> GetToken(ApplicationUser user, string password)
        {
            // Sign the user in
            SignInResult result = await _signInManager.PasswordSignInAsync(user, password, true, false);

            // Todo: Implement 2FA
            if (result.RequiresTwoFactor)
            {
                throw new NotImplementedException("Two Factor Authentication (2FA) has not been implemented yet.");
            }

            // Throw if login failed
            if (!result.Succeeded) throw new AuthenticationFailedException($"Failed to authenticate user with ID \"{user.Id}\".");

            // Authentication successful, generate JSON Web Token (JWT)
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_tokenSettings.Secret);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                // Todo: Refine these settings
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Id)
                }),
                /*Expires = DateTime.UtcNow.AddDays(7),*/
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            // Return token
            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return securityToken;
        }
    }
}
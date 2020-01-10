namespace OneSim.Auth.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using IdentityModel;

    using IdentityServer4.Models;
    using IdentityServer4.Services;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.IdentityModel.JsonWebTokens;

    using OneSim.Identity.Domain.Entities;

    /// <summary>
    ///     The implementation of <see cref="IProfileService"/> allowing IdentityServer to connect to the OneSim user
    ///     and profile store.
    /// </summary>
    public class ProfileService : IProfileService
    {
        /// <summary>
        ///     The <see cref="UserManager{TUser}"/>.
        /// </summary>
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProfileService"/> class.
        /// </summary>
        /// <param name="userManager">
        ///     The <see cref="UserManager{TUser}"/> for the <see cref="ApplicationUser"/>.
        /// </param>
        public ProfileService(UserManager<ApplicationUser> userManager) => _userManager = userManager ??
                                                                                          throw new
                                                                                              ArgumentNullException(nameof(userManager),
                                                                                                                    "The User Manager cannot be null");

        /// <summary>
        ///     This method is called whenever claims about the user are requested
        ///     (e.g. during token creation or via the userinfo endpoint)
        /// </summary>
        /// <param name="context">
        ///     The <see cref="ProfileDataRequestContext"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            // Get the subject from the context
            ClaimsPrincipal subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));

            // Try to get the claim
            Claim subjectClaim = subject.Claims.FirstOrDefault(x => x.Type == "sub");

            if (subjectClaim == null) throw new Exception("Unable to find the subject ID claim.");

            // Get the subject ID
            string subjectId = subjectClaim.Value;

            // Get the ApplicationUser from the ID
            ApplicationUser user = await _userManager.FindByIdAsync(subjectId);

            if (user == null) throw new ArgumentException("Invalid subject identifier");

            // Get the claims for the user
            IEnumerable<Claim> claims = GetClaimsFromUser(user);
            context.IssuedClaims = claims.ToList();
        }

        /// <summary>
        ///     This method gets called whenever identity server needs to determine if the user is valid or active
        ///     (e.g. if the user's account has been deactivated since they logged in).
        ///     (e.g. during token issuance or validation).
        /// </summary>
        /// <param name="context">
        ///     The <see cref="IsActiveContext"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="Task{TResult}"/>.
        /// </returns>
        public async Task IsActiveAsync(IsActiveContext context)
        {
            // Get the subject from the context
            ClaimsPrincipal subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));

            // Try to get the claim
            Claim subjectClaim = subject.Claims.FirstOrDefault(x => x.Type == "sub");

            if (subjectClaim == null) throw new Exception("Unable to find the subject ID claim.");

            // Get the subject ID
            string subjectId = subjectClaim.Value;

            // Get the ApplicationUser from the ID
            ApplicationUser user = await _userManager.FindByIdAsync(subjectId);

            // Set IsActive to false for the moment, need to check the user is good
            context.IsActive = false;

            if (user != null)
            {
                if (_userManager.SupportsUserSecurityStamp)
                {
                    // Get the security stamp
                    string securityStamp = subject.Claims.Where(c => c.Type == "security_stamp")
                                                  .Select(c => c.Value)
                                                  .SingleOrDefault();
                    if (securityStamp != null)
                    {
                        // Get the security stamp from the database
                        string dbSecurityStamp = await _userManager.GetSecurityStampAsync(user);

                        // not active if the stamps don't match
                        if (dbSecurityStamp != securityStamp) return;
                    }
                }

                // Check if the user is locked out
                context.IsActive =
                    !user.LockoutEnabled ||
                    !user.LockoutEnd.HasValue ||
                    user.LockoutEnd <= DateTime.Now;
            }
        }

        /// <summary>
        ///     Gets the <see cref="IEnumerable{T}"/> of <see cref="Claim"/>s for the given <see cref="ApplicationUser"/>.
        /// </summary>
        /// <param name="user">
        ///     The <see cref="ApplicationUser"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable{T}"/> of <see cref="Claim"/>s.
        /// </returns>
        private IEnumerable<Claim> GetClaimsFromUser(ApplicationUser user)
        {
            // Get our basic username and ID claims
            List<Claim> claims = new List<Claim>
                                 {
                                     new Claim(JwtClaimTypes.Subject, user.Id),
                                     new Claim(JwtClaimTypes.PreferredUserName, user.UserName),
                                     new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
                                 };

            // Add the email claims if available
            if (_userManager.SupportsUserEmail)
            {
                claims.AddRange(new[]
                                {
                                    new Claim(JwtClaimTypes.Email, user.Email),
                                    new Claim(JwtClaimTypes.EmailVerified,
                                              user.EmailConfirmed ? "true" : "false",
                                              ClaimValueTypes.Boolean)
                                });
            }

            // Add phone number claims if available
            if (_userManager.SupportsUserPhoneNumber &&
                !string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                claims.AddRange(new[]
                                {
                                    new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber),
                                    new Claim(JwtClaimTypes.PhoneNumberVerified,
                                              user.PhoneNumberConfirmed ? "true" : "false",
                                              ClaimValueTypes.Boolean)
                                });
            }

            return claims;
        }
    }
}
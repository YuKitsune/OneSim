// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Config.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Web
{
    using System.Collections.Generic;

    using IdentityServer4;
    using IdentityServer4.Models;

    /// <summary>
    ///     The Identity Server configuration helper.
    /// </summary>
    public static class Config
    {
        /// <summary>
        ///     Gets the Identity Resources.
        /// </summary>
        public static IEnumerable<IdentityResource> Ids =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        /// <summary>
        ///     Gets the APIs.
        /// </summary>
        public static IEnumerable<ApiResource> Apis =>
            new List<ApiResource>
            {
                new ApiResource("traffic", "Online Traffic API Service")
            };

        /// <summary>
        ///     Gets the Clients.
        /// </summary>
        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "desktop",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris = { "http://127.0.0.1:42069" },
                    PostLogoutRedirectUris = { "http://127.0.0.1:42069" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "traffic"
                    },

                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,

                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    RefreshTokenExpiration = TokenExpiration.Sliding
                },

                // interactive ASP.NET Core MVC client
                new Client
                {
                    ClientId = "mvc",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,
                    RequireConsent = false,
                    RequirePkce = true,

                    // where to redirect to after login
                    RedirectUris = { "http://localhost:5002/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },

                    AllowedScopes = new List<string>
                                    {
                                        IdentityServerConstants.StandardScopes.OpenId,
                                        IdentityServerConstants.StandardScopes.Profile,
                                        "traffic"
                                    },

                    AllowOfflineAccess = true
                },

                // Map visualization for the traffic API
                new Client
                {
                    ClientId = "map",

                    // Todo: Make a better secret
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,
                    RequireConsent = false,
                    RequirePkce = true,

                    // where to redirect to after login
                    RedirectUris =
                    {
                        "https://localhost:6011/",
                        "https://localhost:6011/OidcSignInCallback",
                        "https://localhost:6011/OidcSignOutCallback",
                    },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "https://localhost:6011/OidcSignOutCallback" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "traffic"
                    },

                    AllowOfflineAccess = true
                }
            };
    }
}

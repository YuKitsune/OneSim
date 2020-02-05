namespace OneSim.Identity.Web
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using IdentityServer4;
	using IdentityServer4.EntityFramework.DbContexts;
	using IdentityServer4.EntityFramework.Mappers;
	using IdentityServer4.Models;

	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.DependencyInjection;

	using OneSim.Identity.Application;
	using OneSim.Identity.Domain.Entities;
	using OneSim.Identity.Persistence;

	/// <summary>
	/// 	The Data Seeder.
	/// </summary>
	public class DataSeeder
	{
		/// <summary>
		/// 	Ensures the data is seeded.
		/// </summary>
		/// <param name="serviceProvider">
		///		The <see cref="IServiceProvider"/>.
		/// </param>
		/// <returns>
		///		The <see cref="Task"/>
		/// </returns>
		public static async Task SeedDataAsync(IServiceProvider serviceProvider)
		{
			Console.WriteLine("Seeding database...");
			await SeedDataAsync(serviceProvider.GetRequiredService<ApplicationIdentityDbContext>(),
								serviceProvider.GetRequiredService<UserService>());
			await SeedDataAsync(serviceProvider.GetRequiredService<ConfigurationDbContext>());
			Console.WriteLine("Done seeding database.");
		}

		/// <summary>
		/// 	Seeds the data for the <see cref="ApplicationIdentityDbContext"/>.
		/// </summary>
		/// <param name="dbContext">
		///		The <see cref="ApplicationIdentityDbContext"/>.
		/// </param>
		/// <param name="userService">
		///		The <see cref="UserService"/>.
		/// </param>
		/// <returns>
		///		The <see cref="Task"/>
		/// </returns>
		private static async Task SeedDataAsync(
			ApplicationIdentityDbContext dbContext,
			UserService userService)
		{
			await dbContext.Database.MigrateAsync();
			ApplicationUser seedUser = new ApplicationUser
									   {
										   UserName = "SeedySally101",
										   Email = "eoinmoth@yahoo.ie",
										   Type = UserType.Administrator
									   };
			ApplicationUser existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == seedUser.Email);

			// If the doesn't exist, then create a new one
			if (existingUser == null)
			{
				await userService.RegisterUser(seedUser, "Password123456789!@#$%^&*(");
			}
		}

		/// <summary>
		/// 	Seeds the data for the <see cref="ConfigurationDbContext"/>.
		/// </summary>
		/// <param name="context">
		///		The <see cref="ConfigurationDbContext"/>.
		/// </param>
		/// <returns>
		///		The <see cref="Task"/>
		/// </returns>
		private static async Task SeedDataAsync(ConfigurationDbContext context)
		{
			await context.Database.MigrateAsync();
			if (!context.Clients.Any())
			{
				Console.WriteLine("Seeding Clients.");
				List<Client> clients = new List<Client>
									   {
										   new Client
										   {
											   ClientId = "mvc",
											   ClientName = "MVC Web Client",

											   // Todo: Generate better ones
											   ClientSecrets = { new Secret("secret".Sha256()) },

											   // Todo: Store URLs for each client somewhere
											   ClientUri = "",
											   AllowedGrantTypes = GrantTypes.Hybrid,
											   AllowAccessTokensViaBrowser = false,
											   RequireConsent = false,
											   AllowOfflineAccess = true,
											   AlwaysIncludeUserClaimsInIdToken = true,
											   RedirectUris =
											   {
												   // Todo: Store redirect URIs
												   ""
											   },
											   PostLogoutRedirectUris =
											   {
												   // Todo: Store redirect URIs
												   ""
											   },
											   AllowedScopes =
											   {
												   IdentityServerConstants.StandardScopes.OpenId,
												   IdentityServerConstants.StandardScopes.Profile,
												   "identity"
											   },
											   AccessTokenLifetime = (int) TimeSpan.FromDays(1).TotalSeconds,
											   IdentityTokenLifetime = (int) TimeSpan.FromDays(1).TotalSeconds
										   },
										   new Client
										   {
											   ClientId = "map",
											   ClientName = "Map Web Client",

											   // Todo: Generate better ones
											   ClientSecrets = { new Secret("secret".Sha256()) },

											   // Todo: Store URLs for each client somewhere
											   ClientUri = "",
											   AllowedGrantTypes = GrantTypes.Hybrid,
											   AllowAccessTokensViaBrowser = false,
											   RequireConsent = false,
											   AllowOfflineAccess = true,
											   AlwaysIncludeUserClaimsInIdToken = true,
											   RedirectUris =
											   {
												   // Todo: Store redirect URIs
												   ""
											   },
											   PostLogoutRedirectUris =
											   {
												   // Todo: Store redirect URIs
												   ""
											   },
											   AllowedScopes =
											   {
												   IdentityServerConstants.StandardScopes.OpenId,
												   "traffic"
											   },
											   AccessTokenLifetime = (int) TimeSpan.FromDays(1).TotalSeconds,
											   IdentityTokenLifetime = (int) TimeSpan.FromDays(1).TotalSeconds
										   },
										   new Client
										   {
											   ClientId = "desktop",
											   ClientName = "Desktop Application Client",

											   // Todo: Generate better ones
											   ClientSecrets = { new Secret("secret".Sha256()) },

											   // Todo: Store URLs for each client somewhere
											   ClientUri = "",
											   AllowedGrantTypes = GrantTypes.Hybrid,
											   AllowAccessTokensViaBrowser = false,
											   RequireConsent = false,
											   AllowOfflineAccess = true,
											   AlwaysIncludeUserClaimsInIdToken = true,
											   RedirectUris =
											   {
												   "http://127.0.0.1:6969"
											   },
											   PostLogoutRedirectUris =
											   {
												   // Todo: Store redirect URIs
												   ""
											   },
											   AllowedScopes =
											   {
												   IdentityServerConstants.StandardScopes.OpenId,
												   IdentityServerConstants.StandardScopes.Profile,
												   "identity",
												   "traffic"
											   },
											   AccessTokenLifetime = (int) TimeSpan.FromDays(1).TotalSeconds,
											   IdentityTokenLifetime = (int) TimeSpan.FromDays(1).TotalSeconds
										   }
									   };
				await context.Clients.AddRangeAsync(clients.Select(c => c.ToEntity()));
				await context.SaveChangesAsync();
			}
			else
			{
				Console.WriteLine("Clients already populated.");
			}

			if (!context.IdentityResources.Any())
			{
				Console.WriteLine("Populating Identity Resources.");

				List<IdentityResource> resources = new List<IdentityResource>
												   {
													   new IdentityResources.OpenId(),
													   new IdentityResources.Profile(),
												   };
				await context.IdentityResources.AddRangeAsync(resources.Select(r => r.ToEntity()));
				await context.SaveChangesAsync();
			}
			else
			{
				Console.WriteLine("Identity Resources already populated.");
			}

			if (!context.ApiResources.Any())
			{
				Console.WriteLine("Populating API Resources.");
				List<ApiResource> apiResources = new List<ApiResource>
												 {
													 new ApiResource("identity", "Identity Service"),
													 new ApiResource("traffic", "Online Traffic Service")
												 };
				await context.ApiResources.AddRangeAsync(apiResources.Select(r => r.ToEntity()));
				await context.SaveChangesAsync();
			}
			else
			{
				Console.WriteLine("Api Resources already populated");
			}
		}
	}
}
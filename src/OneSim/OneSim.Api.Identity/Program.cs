namespace OneSim.Api.Identity
{
    using System;
    using System.Linq;

    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;

    using OneSim.Api.Identity.Data;
    using OneSim.Identity.Application;
    using OneSim.Identity.Application.Abstractions;
    using OneSim.Identity.Domain.Entities;

    /// <summary>
    ///     The Program.
    /// </summary>
    public class Program
    {
        /// <summary>
        ///     The application entry point.
        /// </summary>
        /// <param name="args">
        ///     The arguments.
        /// </param>
        public static void Main(string[] args)
        {
            // Create the host
            IWebHost host = CreateWebHostBuilder(args).Build();

            // Seed the data
            using (IServiceScope scope = host.Services.CreateScope())
            {
                // Get the service provider
                IServiceProvider services = scope.ServiceProvider;

                // Check if the seed user exists
                ApplicationIdentityDbContext dbContext = services.GetService<ApplicationIdentityDbContext>();
                ApplicationUser seedUser = new ApplicationUser
                                           {
                                               UserName = "SeedySally101", Email = "eoinmoth@yahoo.ie"
                                           };
                ApplicationUser existingUser = dbContext.Users.FirstOrDefault(u => u.Email == seedUser.Email);

                // If the doesn't exist, then create a new one
                if (existingUser == null)
                {
                    UserService userService = services.GetService<UserService>();
                    IUrlHelper urlHelper = services.GetService<IUrlHelper>();
                    userService.CreateUser(seedUser, "Password123456789!@#$%^&*(", urlHelper, "https").GetAwaiter().GetResult();
                }
            }

            // Run
            host.Run();
        }

        /// <summary>
        ///     Creates the <see cref="IWebHostBuilder"/>.
        /// </summary>
        /// <param name="args">
        ///     The arguments.
        /// </param>
        /// <returns>
        ///     The <see cref="IWebHostBuilder"/>.
        /// </returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
    }
}
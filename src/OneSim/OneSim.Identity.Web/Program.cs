namespace OneSim.Identity.Web
{
    using System;
    using System.Linq;

    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;

    using OneSim.Identity.Application;
    using OneSim.Identity.Application.Abstractions;
    using OneSim.Identity.Domain.Entities;
    using OneSim.Identity.Persistence;

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
                DataSeeder.SeedDataAsync(services).GetAwaiter().GetResult();
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
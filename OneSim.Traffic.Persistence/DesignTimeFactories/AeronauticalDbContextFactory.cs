// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AeronauticalDbContextFactory.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Persistence.DesignTimeFactories
{
    using System.IO;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    ///     The <see cref="AeronauticalDbContext"/>'s <see cref="IDesignTimeDbContextFactory{TContext}"/>.
    /// </summary>
    internal class AeronauticalDbContextFactory : IDesignTimeDbContextFactory<AeronauticalDbContext>
    {
        /// <summary>
        ///     Creates the <see cref="AeronauticalDbContext"/>.
        /// </summary>
        /// <param name="args">
        ///     The arguments.
        /// </param>
        /// <returns>
        ///     The <see cref="AeronauticalDbContext"/>.
        /// </returns>
        public AeronauticalDbContext CreateDbContext(string[] args)
        {
            // Build config
            IConfiguration config = new ConfigurationBuilder()
                                   .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
                                   .AddJsonFile("appsettings.json")
                                   .Build();

            // Create options builder
            DbContextOptionsBuilder<AeronauticalDbContext> optionsBuilder = new DbContextOptionsBuilder<AeronauticalDbContext>();
            optionsBuilder.UseNpgsql(config.GetConnectionString("AeronauticalDataConnection"));

            return new AeronauticalDbContext(optionsBuilder.Options);
        }
    }
}

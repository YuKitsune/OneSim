// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdentityDbContextFactory.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Persistence.DesignTimeFactories
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;

    /// <summary>
    ///     The <see cref="IdentityDbContext"/>s <see cref="IDesignTimeDbContextFactory{TContext}"/>.
    /// </summary>
    public class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
    {
        /// <summary>
        ///     Creates a new <see cref="IdentityDbContext"/> instance.
        /// </summary>
        /// <param name="args">
        ///     The arguments.
        /// </param>
        /// <returns>
        ///     The new <see cref="IdentityDbContext"/>.
        /// </returns>
        public IdentityDbContext CreateDbContext(string[] args)
        {
            // Create options builder
            DbContextOptionsBuilder<IdentityDbContext> optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();

            // It's just a local test DB so this is okay, thought it'd be nice to move it somewhere else
            // It'd actually be better if this class didn't have to exist
            optionsBuilder.UseNpgsql("Host=localhost; Database=onesim_identity; Username=onesim_identity_server; Password=OneSim.app2020123!@#");

            return new IdentityDbContext(optionsBuilder.Options);
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdentityDbContext.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Persistence
{
    using Microsoft.EntityFrameworkCore;

    using OneSim.Identity.Application.Abstractions;
    using OneSim.Identity.Infrastructure;

    using Strato.Persistence.EntityFrameworkCore;

    /// <summary>
    ///     The <see cref="IIdentityDbContext{TUser}"/> implementation for the <see cref="User"/>.
    /// </summary>
    public class IdentityDbContext : TransactionalIdentityDbContext, IIdentityDbContext<User>
    {
        /// <summary>
        ///     Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="User"/>s.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="IdentityDbContext"/> class.
        /// </summary>
        /// <param name="dbContextOptions">
        ///     The <see cref="DbContextOptions{TContext}"/> for the <see cref="IdentityDbContext"/>.
        /// </param>
        public IdentityDbContext(DbContextOptions<IdentityDbContext> dbContextOptions)
            : base(dbContextOptions)
        {
            // Does nothing fun, just calls the base constructor
        }
    }
}

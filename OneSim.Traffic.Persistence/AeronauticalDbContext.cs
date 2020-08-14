// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AeronauticalDbContext.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Persistence
{
    using Microsoft.EntityFrameworkCore;

    using OneSim.Traffic.Application.Abstractions;
    using OneSim.Traffic.Domain.Entities;
    using OneSim.Traffic.Domain.Entities.Aeronautical;

    /// <summary>
    ///     The database containing aeronautical information.
    /// </summary>
    public class AeronauticalDbContext : DbContext, IAeronauticalDbContext
    {
        /// <summary>
        ///     Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="SectorSet"/>s.
        /// </summary>
        public DbSet<SectorSet> SectorSets { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="Fix"/>es.
        /// </summary>
        public DbSet<Fix> Fixes { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="Navaid"/>s.
        /// </summary>
        public DbSet<Navaid> Navaids { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="Airport"/>s.
        /// </summary>
        public DbSet<Airport> Airports { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="Runway"/>s.
        /// </summary>
        public DbSet<Runway> Runways { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="TerminalRoute"/>s.
        /// </summary>
        public DbSet<TerminalRoute> TerminalRoutes { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="Airway"/>s.
        /// </summary>
        public DbSet<Airway> Airways { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="ControllerPosition"/>s.
        /// </summary>
        public DbSet<ControllerPosition> ControllerPositions { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="Sector"/>s.
        /// </summary>
        public DbSet<Sector> Sectors { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="ControllerPriority"/>s.
        /// </summary>
        public DbSet<ControllerPriority> ControllerPriorities { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AeronauticalDbContext"/> class.
        /// </summary>
        /// <param name="options">
        ///     The <see cref="DbContextOptions{TContext}"/>.
        /// </param>
        public AeronauticalDbContext(DbContextOptions<AeronauticalDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        ///     Method called when configuring models.
        /// </summary>
        /// <param name="modelBuilder">
        ///     The <see cref="ModelBuilder"/>.
        /// </param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Fix>().OwnsOne(f => f.Location);
            modelBuilder.Entity<Navaid>().OwnsOne(f => f.Location);
            modelBuilder.Entity<Airport>().OwnsOne(f => f.Location);
        }
    }
}

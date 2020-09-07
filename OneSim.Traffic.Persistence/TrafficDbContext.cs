// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrafficDbContext.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    using OneSim.Traffic.Application.Abstractions;
    using OneSim.Traffic.Domain.Entities;
    using OneSim.Traffic.Domain.ValueObjects;

    /// <summary>
    ///     The database containing the most up-to-date information on traffic connected to a specific Online Flight
    ///     Simulation Network.
    /// </summary>
    public class TrafficDbContext : DbContext, ITrafficDbContext
    {
        /// <summary>
        ///     Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="AirTrafficController"/>s.
        /// </summary>
        public DbSet<AirTrafficController> Controllers { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="Pilot"/>s.
        /// </summary>
        public DbSet<Pilot> Pilots { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="FlightNotification"/>s.
        /// </summary>
        public DbSet<FlightNotification> FlightNotifications { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="FlightPlan"/>s.
        /// </summary>
        public DbSet<FlightPlan> FlightPlans { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="Server"/>s.
        /// </summary>
        public DbSet<Server> Servers { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TrafficDbContext"/> class.
        /// </summary>
        /// <param name="options">
        ///     The <see cref="DbContextOptions{TContext}"/>.
        /// </param>
        public TrafficDbContext(DbContextOptions<TrafficDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        ///     Method used to further configure the model that was discovered by convention from the entity types
        ///     exposed in <see cref="DbSet{TEntity}" /> properties on your derived context. The resulting model may be cached
        ///     and re-used for subsequent instances of your derived context.
        /// </summary>
        /// <param name="modelBuilder">
        ///     The builder being used to construct the model for this context. Databases (and other extensions) typically
        ///     define extension methods on this object that allow you to configure aspects of the model that are specific
        ///     to a given database.
        /// </param>
        /// <remarks>
        ///     If a model is explicitly set on the options for this context (via <see cref="DbContextOptionsBuilder.UseModel(IModel)" />)
        ///     then this method will not be run.
        /// </remarks>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Need to specify to store Squawk Codes as strings
            modelBuilder.Entity<Pilot>()
                        .Property(p => p.Squawk)
                        .HasConversion(
                             s => s.ToString(),
                             s => (SquawkCode)s);
            modelBuilder.Entity<Pilot>().OwnsOne(
                p => p.Location,
                a =>
                {
                    a.Property(p => p.Latitude);
                    a.Property(p => p.Longitude);
                    a.Property(p => p.Altitude);
                });
            modelBuilder.Entity<Pilot>().OwnsMany(
                p => p.History,
                a =>
                {
                    a.Property(p => p.Latitude);
                    a.Property(p => p.Longitude);
                    a.Property(p => p.Altitude);
                });
        }
    }
}

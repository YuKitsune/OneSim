// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITrafficDbContext.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Application.Abstractions
{
    using Microsoft.EntityFrameworkCore;

    using OneSim.Traffic.Domain.Entities;

    using Strato.Persistence.Abstractions;

    /// <summary>
    ///     The interface representing a database containing the most up-to-date traffic data for a specific traffic
    ///     data source.
    /// </summary>
    public interface ITrafficDbContext : IDbContext
    {
        /// <summary>
        ///     Gets or sets the <see cref="AirTrafficController"/>s <see cref="DbSet{TEntity}"/>.
        /// </summary>
        DbSet<AirTrafficController> Controllers { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="Pilot"/>s <see cref="DbSet{TEntity}"/>.
        /// </summary>
        DbSet<Pilot> Pilots { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="FlightNotification"/>s <see cref="DbSet{TEntity}"/>.
        /// </summary>
        DbSet<FlightNotification> FlightNotifications { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="FlightPlan"/>s <see cref="DbSet{TEntity}"/>.
        /// </summary>
        DbSet<FlightPlan> FlightPlans { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="Server"/>s <see cref="DbSet{TEntity}"/>.
        /// </summary>
        DbSet<Server> Servers { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="Point3d"/>s <see cref="DbSet{TEntity}"/>.
        /// </summary>
        DbSet<Point3d> Points { get; set; }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAeronauticalDbContext.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Application.Abstractions
{
    using Microsoft.EntityFrameworkCore;
    using OneSim.Traffic.Domain.Entities.Ais;
    using Strato.Persistence.Abstractions;

    /// <summary>
    ///     The interface representing the database containing aeronautical information.
    /// </summary>
    public interface IAeronauticalDbContext : IDbContext
    {
        /// <summary>
        ///     Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="Fix"/>es.
        /// </summary>
        DbSet<Fix> Fixes { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="Navaid"/>s.
        /// </summary>
        DbSet<Navaid> Navaids { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="Airport"/>s.
        /// </summary>
        DbSet<Airport> Airports { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="Runway"/>s.
        /// </summary>
        DbSet<Runway> Runways { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="TerminalRoute"/>s.
        /// </summary>
        DbSet<TerminalRoute> TerminalRoutes { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="Airway"/>s.
        /// </summary>
        DbSet<Airway> Airways { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="ControllerPosition"/>s.
        /// </summary>
        DbSet<ControllerPosition> ControllerPositions { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="Sector"/>s.
        /// </summary>
        DbSet<Sector> Sectors { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="ControllerPriority"/>s.
        /// </summary>
        DbSet<ControllerPriority> ControllerPriorities { get; set; }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHistoricalDbContext.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Application.Abstractions
{
    using Microsoft.EntityFrameworkCore;

    using OneSim.Traffic.Domain.Entities;

    using Strato.Persistence.Abstractions;

    /// <summary>
    ///     The interface representing a database containing previously used traffic data for archival purposes.
    /// </summary>
    public interface IHistoricalDbContext : IDbContext
    {
        /// <summary>
        ///     Gets or sets the <see cref="TrafficDataArchiveEntry"/>s <see cref="DbSet{TEntity}"/>.
        /// </summary>
        DbSet<TrafficDataArchiveEntry> TrafficData { get; set; }
    }
}

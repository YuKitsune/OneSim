// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MockHelpers.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Tests.Mocks
{
    using System;

    using Microsoft.EntityFrameworkCore;

    using OneSim.Traffic.Persistence;

    /// <summary>
    ///     The mock helpers.
    /// </summary>
    public static class MockHelpers
    {
        /// <summary>
        ///     Gets a new <see cref="TrafficDbContext"/>.
        /// </summary>
        /// <returns>
        ///     A mocked <see cref="TrafficDbContext"/>.
        /// </returns>
        public static TrafficDbContext GetTrafficDbContext() =>
            new TrafficDbContext(
                new DbContextOptionsBuilder<TrafficDbContext>()
                   .UseInMemoryDatabase(Guid.NewGuid().ToString())
                   .Options);

        /// <summary>
        ///     Gets a new <see cref="HistoricalDbContext"/>.
        /// </summary>
        /// <returns>
        ///     A mocked <see cref="HistoricalDbContext"/>.
        /// </returns>
        public static HistoricalDbContext GetHistoricalDbContext() =>
            new HistoricalDbContext(
                new DbContextOptionsBuilder<HistoricalDbContext>()
                   .UseInMemoryDatabase(Guid.NewGuid().ToString())
                   .Options);
    }
}

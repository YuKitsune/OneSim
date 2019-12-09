namespace OneSim.Identity.Tests.Utils
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using OneSim.Identity.Application.Interfaces;

    /// <summary>
    ///     The helpers.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        ///     Gets a new <see cref="IIdentityDbContext"/>.
        /// </summary>
        /// <returns>
        ///    A new <see cref="IIdentityDbContext"/>.
        /// </returns>
        public static IIdentityDbContext GetDbContext() => new MockDbContext(new DbContextOptionsBuilder<MockDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options);
    }
}
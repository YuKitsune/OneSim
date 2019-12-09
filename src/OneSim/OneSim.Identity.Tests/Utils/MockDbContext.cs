namespace OneSim.Identity.Tests.Utils
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using OneSim.Identity.Application.Interfaces;
    using OneSim.Identity.Domain.Entities;

    /// <summary>
    ///     The Mock <see cref="IIdentityDbContext"/>.
    /// </summary>
    public class MockDbContext : IdentityDbContext<ApplicationUser>, IIdentityDbContext
    {
        public MockDbContext()
        {
        }

        public MockDbContext(DbContextOptions<MockDbContext> options)
            : base(options)
        {
        }
    }
}
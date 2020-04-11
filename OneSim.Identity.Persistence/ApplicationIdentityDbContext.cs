namespace OneSim.Identity.Persistence
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    using OneSim.Identity.Application.Abstractions;
    using OneSim.Identity.Domain.Entities;

    /// <summary>
    ///     The Application <see cref="IdentityDbContext"/>.
    /// </summary>
    public class ApplicationIdentityDbContext : IdentityDbContext<ApplicationUser>, IIdentityDbContext
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ApplicationIdentityDbContext"/> class.
        /// </summary>
        /// <param name="options">
        ///     The <see cref="DbContextOptions{TContext}"/>.
        /// </param>
        public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options)
            : base(options)
        {
        }
    }
}
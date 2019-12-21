using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OneSim.Api.Identity.Data
{
    using OneSim.Identity.Domain.Entities;

    /// <summary>
    ///     The Application <see cref="IdentityDbContext"/>.
    /// </summary>
    public class ApplicationIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options)
            : base(options)
        {
        }
    }
}
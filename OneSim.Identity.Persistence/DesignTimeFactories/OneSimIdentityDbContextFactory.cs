namespace OneSim.Identity.Persistence.DesignTimeFactories
{
	using System.IO;

	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Design;
	using Microsoft.Extensions.Configuration;

	/// <summary>
	/// 	The <see cref="ApplicationIdentityDbContext"/>'s <see cref="IDesignTimeDbContextFactory{TContext}"/>.
	/// </summary>
	internal class ApplicationIdentityDbContextFactory : IDesignTimeDbContextFactory<ApplicationIdentityDbContext>
	{
		/// <summary>
		/// 	Creates the <see cref="ApplicationIdentityDbContext"/>.
		/// </summary>
		/// <param name="args">
		///		The arguments.
		/// </param>
		/// <returns>
		///		The <see cref="ApplicationIdentityDbContext"/>.
		/// </returns>
		public ApplicationIdentityDbContext CreateDbContext(string[] args)
		{
			// Build config
			IConfiguration config = new ConfigurationBuilder()
								   .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
								   .AddJsonFile("appsettings.json")
								   .Build();

			// Create options builder
			DbContextOptionsBuilder<ApplicationIdentityDbContext> optionsBuilder = new DbContextOptionsBuilder<ApplicationIdentityDbContext>();
			optionsBuilder.UseNpgsql(config.GetConnectionString("IdentityConnection"));

			return new ApplicationIdentityDbContext(optionsBuilder.Options);
		}
	}
}
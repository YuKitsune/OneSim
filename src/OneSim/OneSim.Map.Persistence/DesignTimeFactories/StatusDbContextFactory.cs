namespace OneSim.Map.Persistence.DesignTimeFactories
{
	using System.IO;

	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Design;
	using Microsoft.Extensions.Configuration;

	/// <summary>
	/// 	The <see cref="StatusDbContext"/>'s <see cref="IDesignTimeDbContextFactory{TContext}"/>.
	/// </summary>
	internal class StatusDbContextFactory : IDesignTimeDbContextFactory<StatusDbContext>
	{
		/// <summary>
		/// 	Creates the <see cref="StatusDbContext"/>.
		/// </summary>
		/// <param name="args">
		///		The arguments.
		/// </param>
		/// <returns>
		///		The <see cref="StatusDbContext"/>.
		/// </returns>
		public StatusDbContext CreateDbContext(string[] args)
		{
			// Build config
			IConfiguration config = new ConfigurationBuilder()
								   .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
								   .AddJsonFile("appsettings.json")
								   .Build();

			// Create options builder
			DbContextOptionsBuilder<StatusDbContext> optionsBuilder = new DbContextOptionsBuilder<StatusDbContext>();
			optionsBuilder.UseNpgsql(config.GetConnectionString("StatusConnection"));

			return new StatusDbContext(optionsBuilder.Options);
		}
	}
}
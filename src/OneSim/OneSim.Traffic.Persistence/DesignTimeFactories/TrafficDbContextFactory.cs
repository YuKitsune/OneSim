namespace OneSim.Traffic.Persistence.DesignTimeFactories
{
	using System.IO;

	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Design;
	using Microsoft.Extensions.Configuration;

	/// <summary>
	/// 	The <see cref="TrafficDbContext"/>'s <see cref="IDesignTimeDbContextFactory{TContext}"/>.
	/// </summary>
	internal class TrafficDbContextFactory : IDesignTimeDbContextFactory<TrafficDbContext>
	{
		/// <summary>
		/// 	Creates the <see cref="TrafficDbContext"/>.
		/// </summary>
		/// <param name="args">
		///		The arguments.
		/// </param>
		/// <returns>
		///		The <see cref="TrafficDbContext"/>.
		/// </returns>
		public TrafficDbContext CreateDbContext(string[] args)
		{
			// Build config
			IConfiguration config = new ConfigurationBuilder()
								   .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
								   .AddJsonFile("appsettings.json")
								   .Build();

			// Create options builder
			DbContextOptionsBuilder<TrafficDbContext> optionsBuilder = new DbContextOptionsBuilder<TrafficDbContext>();
			optionsBuilder.UseNpgsql(config.GetConnectionString("TrafficDataConnection"));

			return new TrafficDbContext(optionsBuilder.Options);
		}
	}
}
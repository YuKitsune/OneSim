namespace OneSim.Map.Persistence.DesignTimeFactories
{
	using System.IO;

	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Design;
	using Microsoft.Extensions.Configuration;

	/// <summary>
	/// 	The <see cref="HistoricalDbContext"/>'s <see cref="IDesignTimeDbContextFactory{TContext}"/>.
	/// </summary>
	internal class HistoricalDbContextFactory : IDesignTimeDbContextFactory<HistoricalDbContext>
	{
		/// <summary>
		/// 	Creates the <see cref="HistoricalDbContext"/>.
		/// </summary>
		/// <param name="args">
		///		The arguments.
		/// </param>
		/// <returns>
		///		The <see cref="HistoricalDbContext"/>.
		/// </returns>
		public HistoricalDbContext CreateDbContext(string[] args)
		{
			// Build config
			IConfiguration config = new ConfigurationBuilder()
								   .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
								   .AddJsonFile("appsettings.json")
								   .Build();

			// Create options builder
			DbContextOptionsBuilder<HistoricalDbContext> optionsBuilder = new DbContextOptionsBuilder<HistoricalDbContext>();
			optionsBuilder.UseNpgsql(config.GetConnectionString("HistoricalConnection"));

			return new HistoricalDbContext(optionsBuilder.Options);
		}
	}
}
namespace OneSim.Identity.Persistence.DesignTimeFactories
{
	using System.IO;

	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Design;
	using Microsoft.Extensions.Configuration;

	/// <summary>
	/// 	The <see cref="KeysDbContext"/>'s <see cref="IDesignTimeDbContextFactory{TContext}"/>.
	/// </summary>
	internal class KeysDbContextFactory : IDesignTimeDbContextFactory<KeysDbContext>
	{
		/// <summary>
		/// 	Creates the <see cref="KeysDbContext"/>.
		/// </summary>
		/// <param name="args">
		///		The arguments.
		/// </param>
		/// <returns>
		///		The <see cref="KeysDbContext"/>.
		/// </returns>
		public KeysDbContext CreateDbContext(string[] args)
		{
			// Build config
			IConfiguration config = new ConfigurationBuilder()
								   .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
								   .AddJsonFile("appsettings.json")
								   .Build();

			// Create options builder
			DbContextOptionsBuilder<KeysDbContext> optionsBuilder = new DbContextOptionsBuilder<KeysDbContext>();
			optionsBuilder.UseNpgsql(config.GetConnectionString("KeysConnection"));

			return new KeysDbContext(optionsBuilder.Options);
		}
	}
}
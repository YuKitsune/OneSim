namespace OneSim.Map.Persistence
{
	using Microsoft.EntityFrameworkCore;

	using OneSim.Map.Application.Abstractions;
	using OneSim.Map.Domain.Entities;

	/// <summary>
	/// 	The database containing previously used traffic data for archival purposes.
	/// </summary>
	public class HistoricalDbContext : DbContext, IHistoricalDbContext
	{
		/// <summary>
		/// 	Gets or sets the <see cref="TrafficDataArchiveEntry"/>s <see cref="DbSet{TEntity}"/>.
		/// </summary>
		public DbSet<TrafficDataArchiveEntry> TrafficData { get; set; }

		/// <summary>
		///     Initializes a new instance of the <see cref="HistoricalDbContext"/> class.
		/// </summary>
		/// <param name="options">
		///     The <see cref="DbContextOptions{TContext}"/>.
		/// </param>
		public HistoricalDbContext(DbContextOptions<HistoricalDbContext> options)
			: base(options)
		{
		}
	}
}
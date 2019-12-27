namespace OneSim.Map.Persistence
{
	using Microsoft.EntityFrameworkCore;

	using OneSim.Map.Application.Abstractions;
	using OneSim.Map.Domain.Entities;

	/// <summary>
	/// 	The Historical Status Database Context containing previously used status files.
	/// </summary>
	public class HistoricalDbContext : DbContext, IHistoricalDbContext
	{
		/// <summary>
		/// 	Gets or sets the <see cref="StatusFileArchiveEntry"/>s <see cref="DbSet{TEntity}"/>.
		/// </summary>
		public DbSet<StatusFileArchiveEntry> StatusFiles { get; set; }

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
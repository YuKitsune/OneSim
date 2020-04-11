namespace OneSim.Traffic.Application.Abstractions
{
	using System.Threading;
	using System.Threading.Tasks;

	using Microsoft.EntityFrameworkCore;

	using OneSim.Traffic.Domain.Entities;

	/// <summary>
	/// 	The interface representing a database containing previously used traffic data for archival purposes.
	/// </summary>
	public interface IHistoricalDbContext
	{
		/// <summary>
		/// 	Gets or sets the <see cref="TrafficDataArchiveEntry"/>s <see cref="DbSet{TEntity}"/>.
		/// </summary>
		DbSet<TrafficDataArchiveEntry> TrafficData { get; set; }

		/// <summary>
		/// 	Persists the changes to the database.
		/// </summary>
		/// <returns>
		///		The number of entries persisted to the database.
		/// </returns>
		int SaveChanges();

		/// <summary>
		/// 	Persists the changes to the database as an asynchronous operation.
		/// </summary>
		/// <param name="cancellationToken">
		///		The <see cref="CancellationToken"/>.
		/// </param>
		/// <returns>
		///		The number of entries persisted to the database.
		/// </returns>
		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
	}
}
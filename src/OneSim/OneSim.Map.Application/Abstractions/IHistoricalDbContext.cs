namespace OneSim.Map.Application.Abstractions
{
	using System.Threading.Tasks;

	using Microsoft.EntityFrameworkCore;

	using OneSim.Map.Domain.Entities;

	/// <summary>
	/// 	The Historical Status Database Context containing previously used status files.
	/// </summary>
	public interface IHistoricalDbContext
	{
		/// <summary>
		/// 	Gets or sets the <see cref="StatusFileArchiveEntry"/>s <see cref="DbSet{TEntity}"/>.
		/// </summary>
		DbSet<StatusFileArchiveEntry> StatusFiles { get; set; }

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
		/// <returns>
		///		The number of entries persisted to the database.
		/// </returns>
		Task<int> SaveChangesAsync();
	}
}
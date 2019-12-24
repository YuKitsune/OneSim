namespace OneSim.Identity.Application.Abstractions
{
	using Microsoft.EntityFrameworkCore;

	using OneSim.Identity.Domain.Entities;

	/// <summary>
	/// 	The Identity Database Context.
	/// </summary>
	public interface IIdentityDbContext
	{
		/// <summary>
		/// 	Gets or sets the <see cref="ApplicationUser"/>s <see cref="DbSet{TEntity}"/>.
		/// </summary>
		DbSet<ApplicationUser> Users { get; set; }

		/// <summary>
		/// 	Persists the changes to the database.
		/// </summary>
		/// <returns>
		///		The number of entries persisted to the database.
		/// </returns>
		int SaveChanges();
	}
}
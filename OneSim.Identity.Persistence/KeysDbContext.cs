namespace OneSim.Identity.Persistence
{
	using Microsoft.EntityFrameworkCore;

	using OneSim.Identity.Application.Abstractions;
	using OneSim.Identity.Domain.Entities;

	/// <summary>
	/// 	The <see cref="DbContext"/> implementing <see cref="IKeysDbContext"/>.
	/// </summary>
	public class KeysDbContext : DbContext, IKeysDbContext
	{
		/// <summary>
		/// 	Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="Key"/>s.
		/// </summary>
		public DbSet<Key> Keys { get; set; }

		/// <summary>
		///     Initializes a new instance of the <see cref="KeysDbContext"/> class.
		/// </summary>
		/// <param name="options">
		///     The <see cref="DbContextOptions{TContext}"/>.
		/// </param>
		public KeysDbContext(DbContextOptions<KeysDbContext> options)
			: base(options)
		{
		}
	}
}
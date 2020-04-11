namespace OneSim.Identity.Application.Abstractions
{
	using Microsoft.EntityFrameworkCore;

	using OneSim.Identity.Domain.Entities;

	/// <summary>
	/// 	The interface representing a Database Context containing security keys.
	/// </summary>
	public interface IKeysDbContext
	{
		/// <summary>
		/// 	Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="Key"/>s.
		/// </summary>
		DbSet<Key> Keys { get; set; }
	}
}
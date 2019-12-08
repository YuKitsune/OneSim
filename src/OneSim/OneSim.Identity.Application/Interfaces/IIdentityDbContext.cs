namespace OneSim.Identity.Application.Interfaces
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
	}
}
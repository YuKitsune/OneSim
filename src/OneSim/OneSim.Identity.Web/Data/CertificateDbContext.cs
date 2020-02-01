namespace OneSim.Identity.Web.Data
{
	using Microsoft.EntityFrameworkCore;

	/// <summary>
	/// 	The <see cref="DbContext"/> for storing <see cref="Certificate"/>s.
	/// </summary>
	public class CertificateDbContext : DbContext
	{
		/// <summary>
		/// 	Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="Certificate"/>s.
		/// </summary>
		public DbSet<Certificate> Certificates { get; set; }
	}
}
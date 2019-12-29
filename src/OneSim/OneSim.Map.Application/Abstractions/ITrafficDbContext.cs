namespace OneSim.Map.Application.Abstractions
{
	using System.Threading;
	using System.Threading.Tasks;

	using Microsoft.EntityFrameworkCore;

	using OneSim.Map.Domain.Entities;

	/// <summary>
	/// 	The interface representing a database containing the most up-to-date traffic data for a specific traffic
	/// 	data source.
	/// </summary>
	public interface ITrafficDbContext
	{
		/// <summary>
		/// 	Gets or sets the <see cref="AirTrafficController"/>s <see cref="DbSet{TEntity}"/>.
		/// </summary>
		DbSet<AirTrafficController> Controllers { get; set; }

		/// <summary>
		/// 	Gets or sets the <see cref="Pilot"/>s <see cref="DbSet{TEntity}"/>.
		/// </summary>
		DbSet<Pilot> Pilots { get; set; }

		/// <summary>
		/// 	Gets or sets the <see cref="FlightNotification"/>s <see cref="DbSet{TEntity}"/>.
		/// </summary>
		DbSet<FlightNotification> FlightNotifications { get; set; }

		/// <summary>
		/// 	Gets or sets the <see cref="FlightPlan"/>s <see cref="DbSet{TEntity}"/>.
		/// </summary>
		DbSet<FlightPlan> FlightPlans { get; set; }

		/// <summary>
		/// 	Gets or sets the <see cref="Server"/>s <see cref="DbSet{TEntity}"/>.
		/// </summary>
		DbSet<Server> Servers { get; set; }

		/// <summary>
		/// 	Gets or sets the <see cref="Point3d"/>s <see cref="DbSet{TEntity}"/>.
		/// </summary>
		DbSet<Point3d> Points { get; set; }

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
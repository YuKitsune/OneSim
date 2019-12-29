namespace OneSim.Map.Persistence
{
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata;

	using OneSim.Map.Application.Abstractions;
	using OneSim.Map.Domain.Entities;
	using OneSim.Map.Domain.ValueObjects;

	/// <summary>
	/// 	The Status <see cref="DbContext"/>
	/// </summary>
	public class TrafficDbContext : DbContext, ITrafficDbContext
	{
		/// <summary>
		/// 	The <see cref="DbSet{TEntity}"/> of <see cref="AirTrafficController"/>s.
		/// </summary>
		public DbSet<AirTrafficController> Controllers { get; set; }

		/// <summary>
		/// 	The <see cref="DbSet{TEntity}"/> of <see cref="Pilot"/>s.
		/// </summary>
		public DbSet<Pilot> Pilots { get; set; }

		/// <summary>
		/// 	The <see cref="DbSet{TEntity}"/> of <see cref="FlightNotification"/>s.
		/// </summary>
		public DbSet<FlightNotification> FlightNotifications { get; set; }

		/// <summary>
		/// 	The <see cref="DbSet{TEntity}"/> of <see cref="FlightPlan"/>s.
		/// </summary>
		public DbSet<FlightPlan> FlightPlans { get; set; }

		/// <summary>
		/// 	The <see cref="DbSet{TEntity}"/> of <see cref="Point3d"/>s.
		/// </summary>
		public DbSet<Point3d> Points { get; set; }

		/// <summary>
		/// 	The <see cref="DbSet{TEntity}"/> of <see cref="Server"/>s.
		/// </summary>
		public DbSet<Server> Servers { get; set; }

		/// <summary>
		///     Initializes a new instance of the <see cref="TrafficDbContext"/> class.
		/// </summary>
		/// <param name="options">
		///     The <see cref="DbContextOptions{TContext}"/>.
		/// </param>
		public TrafficDbContext(DbContextOptions<TrafficDbContext> options)
			: base(options)
		{
		}

		/// <summary>
		/// 	Method used to further configure the model that was discovered by convention from the entity types
		///     exposed in <see cref="DbSet{TEntity}" /> properties on your derived context. The resulting model may be cached
		///     and re-used for subsequent instances of your derived context.
		/// </summary>
		/// <param name="modelBuilder">
		///     The builder being used to construct the model for this context. Databases (and other extensions) typically
		///     define extension methods on this object that allow you to configure aspects of the model that are specific
		///     to a given database.
		/// </param>
		/// <remarks>
		///     If a model is explicitly set on the options for this context (via <see cref="DbContextOptionsBuilder.UseModel(IModel)" />)
		///     then this method will not be run.
		/// </remarks>
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Need to specify to store Squawk Codes as strings
			modelBuilder.Entity<Pilot>()
						.Property(p => p.Squawk)
						.HasConversion(s => s.ToString(),
									   s => (SquawkCode) s);
		}
	}
}
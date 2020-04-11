namespace OneSim.Traffic.Domain.Entities
{
	/// <summary>
	/// 	The Administrative Rating.
	/// </summary>
	public enum AdministrativeRating
	{
		/// <summary>
		/// 	Suspended.
		/// </summary>
		Suspended = 0,

		/// <summary>
		/// 	Observer.
		/// </summary>
		Observer = 1,

		/// <summary>
		/// 	User.
		/// </summary>
		User = 2,

		/// <summary>
		/// 	Supervisor.
		/// </summary>
		Supervisor = 11,

		/// <summary>
		/// 	Administrator
		/// </summary>
		Administrator = 12
	}
}
namespace OneSim.Traffic.Domain.Entities
{
	using System;

	/// <summary>
	/// 	The Air Traffic Controllers Rating.
	/// </summary>
	public enum ControllerRating
	{
		/// <summary>
		///     Inactive.
		/// </summary>
		Inactive = 0,

		/// <summary>
		///     Observer.
		/// </summary>
		Observer = 1,

		/// <summary>
		///     Student 1.
		/// </summary>
		Student1 = 2,

		/// <summary>
		///     Student 2.
		/// </summary>
		Student2 = 3,

		/// <summary>
		///     Student 3.
		/// </summary>
		Student3 = 4,

		/// <summary>
		///     Controller 1.
		/// </summary>
		Controller1 = 5,

		/// <summary>
		///     Controller 2.
		/// </summary>
		/// <remarks>
		///		Has since made obsolete.
		/// </remarks>
		[Obsolete]
		Controller2 = 6,

		/// <summary>
		///     Senior Controller.
		/// </summary>
		Controller3 = 7,

		/// <summary>
		///     Instructor 1.
		/// </summary>
		Instructor1 = 8,

		/// <summary>
		///     Instructor 2.
		/// </summary>
		/// <remarks>
		///		Has since made obsolete.
		/// </remarks>
		[Obsolete]
		Instructor2 = 9,

		/// <summary>
		///     Instructor 3.
		/// </summary>
		Instructor3 = 10,

		/// <summary>
		///     Supervisor.
		/// </summary>
		Supervisor = 11,

		/// <summary>
		///     Administrator
		/// </summary>
		Administrator = 12
	}
}
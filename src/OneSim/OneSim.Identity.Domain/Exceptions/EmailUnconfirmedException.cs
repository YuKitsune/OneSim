namespace OneSim.Identity.Domain.Exceptions
{
	using System;

	using OneSim.Identity.Domain.Entities;

	/// <summary>
	/// 	The Email un-confirmed <see cref="Exception"/>.
	/// </summary>
	public class EmailUnconfirmedException : Exception
	{
		/// <summary>
		/// 	Gets the <see cref="ApplicationUser"/> who's email is un-confirmed.
		/// </summary>
		public ApplicationUser User { get; }

		/// <summary>
		/// 	Initializes a new instance of the <see cref="EmailUnconfirmedException"/>.
		/// </summary>
		/// <param name="user">
		///		The <see cref="ApplicationUser"/> who's email is un-confirmed.
		/// </param>
		/// <param name="message">
		///		The message.
		/// </param>
		/// <param name="innerException">
		///		The inner <see cref="Exception"/> if any.
		/// </param>
		public EmailUnconfirmedException(ApplicationUser user, string message = "", Exception innerException = null) :
			base(message, innerException) => User = user;
	}
}
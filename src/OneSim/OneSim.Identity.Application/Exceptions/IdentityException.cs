namespace OneSim.Identity.Application.Exceptions
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Microsoft.AspNetCore.Identity;

	/// <summary>
	/// 	The Identity <see cref="Exception"/>.
	/// </summary>
	public class IdentityException : Exception
	{
		/// <summary>
		/// 	Gets the <see cref="IEnumerable{T}"/> of <see cref="IdentityError"/>s.
		/// </summary>
		public IEnumerable<IdentityError> Errors { get; }

		/// <summary>
		/// 	Initializes a new instance of the <see cref="IdentityException"/> class.
		/// </summary>
		/// <param name="errors">
		///		The <see cref="IdentityError"/>s.
		/// </param>
		/// <param name="message">
		///		The message.
		/// </param>
		/// <param name="innerException">
		///		The inner <see cref="Exception"/>.
		/// </param>
		public IdentityException(
			IEnumerable<IdentityError> errors,
			string message = "",
			Exception innerException = null) : base(message, innerException) => Errors = errors.ToArray();
	}
}
namespace OneSim.Identity.Domain.Exceptions
{
	using System;

	/// <summary>
	/// 	The User Not Found <see cref="Exception"/>.
	/// </summary>
	public class UserNotFoundException : Exception
	{
		/// <summary>
		/// 	Initializes a new instance of the <see cref="UserNotFoundException"/>.
		/// </summary>
		/// <param name="message">
		///		The message.
		/// </param>
		/// <param name="innerException">
		///		The inner <see cref="Exception"/> if any.
		/// </param>
		public UserNotFoundException(string message, Exception innerException = null) : base(message, innerException)
		{
		}
	}
}
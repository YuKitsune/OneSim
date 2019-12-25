namespace OneSim.Api.Data.Responses
{
	using System;

	/// <summary>
	/// 	The Base Response.
	/// </summary>
	public class BaseResponse
	{
		/// <summary>
		/// 	Gets the message.
		/// </summary>
		public string Message { get; }

		/// <summary>
		///		Gets the <see cref="ResponseStatus"/>.
		/// </summary>
		public ResponseStatus Status { get; }

		/// <summary>
		/// 	Initializes a new instance of the <see cref="BaseResponse"/> class.
		/// </summary>
		/// <param name="status">
		///		The <see cref="ResponseStatus"/>.
		/// </param>
		/// <param name="message">
		///		The message.
		/// </param>
		public BaseResponse(ResponseStatus status, string message)
		{
			if (string.IsNullOrEmpty(message)) throw new ArgumentNullException(nameof(message), "The message cannot be null or empty.");

			Message = message;
			Status = status;
		}
	}
}
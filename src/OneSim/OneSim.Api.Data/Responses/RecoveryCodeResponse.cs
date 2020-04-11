namespace OneSim.Api.Data.Responses
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	/// <summary>
	/// 	The Enable Two-Factor Authentication Recovery Code(s) <see cref="BaseResponse"/>.
	/// </summary>
	public class RecoveryCodeResponse : BaseResponse
	{
		/// <summary>
		/// 	Gets or sets the <see cref="IEnumerable{T}"/> of <see cref="string"/> representing the Two-Factor Authentication
		/// 	recovery codes.
		/// </summary>
		public IEnumerable<string> RecoveryCodes { get; set; }

		/// <summary>
		/// 	Initializes a new instance of the <see cref="RecoveryCodeResponse"/> class.
		/// </summary>
		/// <param name="status">
		///		The <see cref="ResponseStatus"/>.
		/// </param>
		/// <param name="message">
		///		The message.
		/// </param>
		public RecoveryCodeResponse(ResponseStatus status, string message) : base(status, message)
		{
		}
	}
}
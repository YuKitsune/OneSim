namespace OneSim.Api.Data.Responses
{
	using System.Collections.Generic;

	/// <summary>
	/// 	The Enable Two-Factor Authentication <see cref="BaseResponse"/>.
	/// </summary>
	public class EnableTwoFactorAuthenticationResponse : BaseResponse
	{
		/// <summary>
		/// 	Gets or sets the <see cref="IEnumerable{T}"/> of <see cref="string"/> representing the Two-Factor Authentication
		/// 	recovery codes.
		/// </summary>
		public IEnumerable<string> RecoveryCodes { get; set; }

		/// <summary>
		/// 	Initializes a new instance of the <see cref="EnableTwoFactorAuthenticationResponse"/> class.
		/// </summary>
		/// <param name="status">
		///		The <see cref="ResponseStatus"/>.
		/// </param>
		/// <param name="message">
		///		The message.
		/// </param>
		public EnableTwoFactorAuthenticationResponse(ResponseStatus status, string message) : base(status, message)
		{
		}
	}
}
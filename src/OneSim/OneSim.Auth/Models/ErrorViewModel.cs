namespace OneSim.Auth.Models
{
	using IdentityServer4.Models;

	/// <summary>
	/// 	The Error View Model.
	/// </summary>
	public class ErrorViewModel
	{
		/// <summary>
		/// 	Gets or sets the Request ID.
		/// </summary>
		public string RequestId { get; set; }

		/// <summary>
		/// 	Gets a value indicating whether or not to show the <see cref="RequestId"/>.
		/// </summary>
		public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

		/// <summary>
		/// 	Gets or sets the <see cref="ErrorMessage"/>.
		/// </summary>
		public ErrorMessage Error { get; set; }
	}
}
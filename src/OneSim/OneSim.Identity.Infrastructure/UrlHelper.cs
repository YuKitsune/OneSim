namespace OneSim.Identity.Infrastructure
{
	using Microsoft.Extensions.Configuration;

	using OneSim.Identity.Application.Abstractions;

	public class UrlHelper : IUrlHelper
	{
		/// <summary>
		/// 	The base domain name.
		/// </summary>
		private readonly string _baseDomainName;

		/// <summary>
		/// 	Initializes a new instance of the <see cref="UrlHelper"/> class.
		/// </summary>
		public UrlHelper() => _baseDomainName = string.Empty;

		// Todo: Implement these and move to a more global class.

		/// <summary>
		/// 	Generates a Password Reset Callback URL.
		/// </summary>
		/// <param name="userId">
		///		The User ID.
		/// </param>
		/// <param name="resetToken">
		///		The reset token.
		/// </param>
		/// <param name="requestScheme">
		///		The request scheme.
		/// </param>
		/// <returns>
		///		The Password Reset Callback URL.
		/// </returns>
		public string ResetPasswordCallbackLink(string userId, string resetToken, string requestScheme) =>
			$"{requestScheme}://{_baseDomainName}/";

		/// <summary>
		/// 	Generates an Email Confirmation URL.
		/// </summary>
		/// <param name="userId">
		///		The User ID.
		/// </param>
		/// <param name="confirmationToken">
		///		The confirmation token.
		/// </param>
		/// <param name="requestScheme">
		///		The request scheme.
		/// </param>
		/// <returns>
		///		The Email Confirmation URL.
		/// </returns>
		public string EmailConfirmationLink(string userId, string confirmationToken, string requestScheme) => 
			$"{requestScheme}://{_baseDomainName}/";
	}
}
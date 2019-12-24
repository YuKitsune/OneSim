namespace OneSim.Identity.Application.Abstractions
{
	/// <summary>
	/// 	The URL Helper.
	/// </summary>
	public interface IUrlHelper
	{
		/// <summary>
		/// 	Generates a password reset callback URL.
		/// </summary>
		/// <param name="userId">
		///		The <see cref="ApplicationUser.Id"/>.
		/// </param>
		/// <param name="resetToken">
		///		The reset token.
		/// </param>
		/// <param name="requestScheme">
		///		The request scheme.
		/// </param>
		/// <returns>
		///		The password reset callback url.
		/// </returns>
		string ResetPasswordCallbackLink(string userId, string resetToken, string requestScheme);

		/// <summary>
		/// 	Generates an email confirmation url.
		/// </summary>
		/// <param name="userId">
		///		The <see cref="ApplicationUser.Id"/>.
		/// </param>
		/// <param name="confirmationToken">
		///		The confirmation token.
		/// </param>
		/// <param name="requestScheme">
		///		The request scheme.
		/// </param>
		/// <returns>
		///		The email confirmation url.
		/// </returns>
		string EmailConfirmationLink(string userId, string confirmationToken, string requestScheme);
	}
}
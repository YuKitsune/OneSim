namespace OneSim.Identity.Web.Controllers
{
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Mvc;

	using OneSim.Identity.Web.Models.ViewModels.Account;

	/// <summary>
	/// 	The Interface for the Account Management <see cref="Controller"/> providing methods which are common to both
	/// 	the MVC <see cref="Controller"/> and the API <see cref="Controller"/>.
	/// </summary>
	public interface IAccountController
	{
		/// <summary>
		/// 	Handles the request to register a new account.
		/// </summary>
		/// <param name="viewModel">
		/// 	The <see cref="RegisterViewModel" />.
		/// </param>
		/// <returns>
		/// 	The <see cref="IActionResult" />.
		/// </returns>
		public Task<IActionResult> Register(RegisterViewModel viewModel);

		/// <summary>
		/// 	Handles the request to delete an account.
		/// </summary>
		/// <param name="viewModel">
		/// 	The <see cref="DeleteAccountViewModel" />.
		/// </param>
		/// <returns>
		/// 	The <see cref="IActionResult" />.
		/// </returns>
		public Task<IActionResult> DeleteAccount(DeleteAccountViewModel viewModel);

		/// <summary>
		/// 	Handles the request to send an email confirmation email.
		/// </summary>
		/// <returns>
		/// 	The <see cref="IActionResult" />.
		/// </returns>
		public Task<IActionResult> SendEmailConfirmationEmail();

		/// <summary>
		/// 	Handles the request to confirm an email address.
		/// </summary>
		/// <param name="confirmationCode">
		/// 	The confirmation code contained in the email confirmation email.
		/// </param>
		/// <returns>
		/// 	The <see cref="IActionResult" />.
		/// </returns>
		public Task<IActionResult> ConfirmEmail(string confirmationCode);

		/// <summary>
		/// 	Handles the request to send a password reset email.
		/// </summary>
		/// <param name="viewModel">
		/// 	The <see cref="ForgotPasswordViewModel" />.
		/// </param>
		/// <returns>
		/// 	The <see cref="IActionResult" />.
		/// </returns>
		public Task<IActionResult> ForgotPassword(ForgotPasswordViewModel viewModel);

		/// <summary>
		/// 	Handles the request to reset the users password.
		/// </summary>
		/// <param name="viewModel">
		/// 	The <see cref="ResetPasswordViewModel" />.
		/// </param>
		/// <returns>
		/// 	The <see cref="IActionResult" />.
		/// </returns>
		public Task<IActionResult> ResetPassword(ResetPasswordViewModel viewModel);

		/// <summary>
		/// 	Handles the request to change the current users password.
		/// </summary>
		/// <param name="viewModel">
		/// 	The <see cref="ChangePasswordViewModel" />.
		/// </param>
		/// <returns>
		/// 	The <see cref="IActionResult" />.
		/// </returns>
		public Task<IActionResult> ChangePassword(ChangePasswordViewModel viewModel);

		/// <summary>
		/// 	Handles the request to enable Two-Factor Authentication.
		/// </summary>
		/// <param name="viewModel">
		/// 	The <see cref="EnableTwoFactorAuthenticationViewModel" />.
		/// </param>
		/// <returns>
		/// 	The <see cref="IActionResult" />.
		/// </returns>
		public Task<IActionResult> EnableTwoFactorAuthentication(EnableTwoFactorAuthenticationViewModel viewModel);

		/// <summary>
		/// 	Handles the request to reset Two-Factor Authentication.
		/// </summary>
		/// <returns>
		/// 	The <see cref="IActionResult" />.
		/// </returns>
		public Task<IActionResult> ResetTwoFactorAuthentication();

		/// <summary>
		/// 	Handles the request to disable Two-Factor Authentication.
		/// </summary>
		/// <returns>
		/// 	The <see cref="IActionResult" />.
		/// </returns>
		public Task<IActionResult> DisableTwoFactorAuthentication();
	}
}
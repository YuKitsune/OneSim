namespace OneSim.Identity.Web.Models.ViewModels.Account
{
	using System.ComponentModel.DataAnnotations;

	/// <summary>
	/// 	The View Model to use when resetting a password.
	/// </summary>
	public class ResetPasswordViewModel : PasswordAdjustmentViewModel
	{
		/// <summary>
		///		Gets or sets the email address.
		/// </summary>
		[Required, EmailAddress]
		public string Email { get; set; }

		/// <summary>
		/// 	Gets or sets the password reset token.
		/// </summary>
		[Required]
		public string ResetToken { get; set; }
	}
}
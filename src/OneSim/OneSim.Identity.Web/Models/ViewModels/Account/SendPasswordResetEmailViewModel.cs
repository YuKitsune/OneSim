namespace OneSim.Identity.Web.Models.ViewModels.Account
{
	using System.ComponentModel.DataAnnotations;

	/// <summary>
	/// 	The View Model to use for sending a password reset email
	/// </summary>
	public class SendPasswordResetEmailViewModel
	{
		/// <summary>
		/// 	Gets or sets the email address.
		/// </summary>
		[Required, EmailAddress]
		public string Email { get; set; }
	}
}
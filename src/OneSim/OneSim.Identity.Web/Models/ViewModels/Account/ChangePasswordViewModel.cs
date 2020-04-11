namespace OneSim.Identity.Web.Models.ViewModels.Account
{
	using System.ComponentModel.DataAnnotations;

	/// <summary>
	/// 	The View Model to use when changing a password.
	/// </summary>
	public class ChangePasswordViewModel : PasswordAdjustmentViewModel
	{
		/// <summary>
		///		Gets or sets the old password.
		/// </summary>
		[Required, DataType(DataType.Password)]
		public string OldPassword { get; set; }
	}
}
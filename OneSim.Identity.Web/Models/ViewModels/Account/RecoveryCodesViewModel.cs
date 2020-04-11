namespace OneSim.Identity.Web.Models.ViewModels.Account
{
	using System.Collections.Generic;

	/// <summary>
	/// 	The View Model to use when displaying Two-Factor Authentication recovery codes.
	/// </summary>
	public class RecoveryCodesViewModel
	{
		/// <summary>
		/// 	Gets or sets the <see cref="IEnumerable{T}"/> of <see cref="string"/>s representing Two-Factor
		/// 	Authentication recovery codes.
		/// </summary>
		public IEnumerable<string> RecoveryCodes { get; set; }
	}
}
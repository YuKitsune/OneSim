namespace OneSim.Identity.Domain.Entities
{
	using Microsoft.AspNetCore.Identity;

	/// <summary>
	/// 	The Application User.
	/// </summary>
	public class ApplicationUser : IdentityUser
	{
		/// <summary>
		/// 	Gets or sets the <see cref="TwoFactorAuthenticationMethod"/>.
		/// </summary>
		public TwoFactorAuthenticationMethod TwoFactorAuthenticationMethod { get; set; }
	}
}
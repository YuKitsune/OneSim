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

		/// <summary>
		/// 	Gets or sets the <see cref="UserType"/>.
		/// </summary>
		public UserType Type { get; set; }

		/// <summary>
		/// 	Initializes a new instance of the <see cref="ApplicationUser"/> class.
		/// </summary>
		public ApplicationUser() => Type = UserType.User;
	}
}
namespace OneSim.Identity.Domain.Entities
{
	/// <summary>
	/// 	The enum defining the purpose for a <see cref="SecurityKey"/>.
	/// </summary>
	public enum SecurityKeyPurpose
	{
		/// <summary>
		/// 	The <see cref="SecurityKey"/> is required for the Identity logic.
		/// </summary>
		Identity,

		/// <summary>
		/// 	The <see cref="SecurityKey"/> is required for data protection.
		/// </summary>
		DataProtection
	}
}
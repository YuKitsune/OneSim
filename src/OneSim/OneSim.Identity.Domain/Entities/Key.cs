namespace OneSim.Identity.Domain.Entities
{
	/// <summary>
	/// 	The security key entity.
	/// </summary>
	public class Key
	{
		/// <summary>
		/// 	Gets or sets the ID.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// 	Gets or sets the <see cref="SecurityKeyPurpose"/>.
		/// </summary>
		public SecurityKeyPurpose Purpose { get; set; }

		/// <summary>
		/// 	Gets or sets the SecurityKey ID
		/// </summary>
		public string SecurityKeyId { get; set; }

		/// <summary>
		/// 	Gets or sets the key.
		/// </summary>
		public string Data { get; set; }
	}
}
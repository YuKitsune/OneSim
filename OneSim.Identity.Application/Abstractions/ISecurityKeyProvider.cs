namespace OneSim.Identity.Application.Abstractions
{
	using System.Threading.Tasks;

	using Microsoft.IdentityModel.Tokens;

	using OneSim.Identity.Domain.Entities;

	/// <summary>
	/// 	The interface representing a <see cref="SecurityKey"/> Provider.
	/// </summary>
	public interface ISecurityKeyProvider
	{
		/// <summary>
		/// 	Gets the Key Size.
		/// </summary>
		int KeySize { get; }

		/// <summary>
		/// 	Gets the name of the algorithm.
		/// </summary>
		string Algorithm { get; }

		/// <summary>
		/// 	Gets the <see cref="SecurityKey"/> for the intended <see cref="SecurityKeyPurpose"/>.
		/// </summary>
		/// <param name="purpose">
		///		The <see cref="SecurityKeyPurpose"/>.
		/// </param>
		/// <returns>
		///		The <see cref="SecurityKey"/>.
		/// </returns>
		SecurityKey GetSecurityKey(SecurityKeyPurpose purpose);

		/// <summary>
		/// 	Gets the <see cref="SecurityKey"/> for the intended <see cref="SecurityKeyPurpose"/>.
		/// </summary>
		/// <param name="purpose">
		///		The <see cref="SecurityKeyPurpose"/>.
		/// </param>
		/// <returns>
		///		The <see cref="Task"/> containing the <see cref="SecurityKey"/>.
		/// </returns>
		Task<SecurityKey> GetSecurityKeyAsync(SecurityKeyPurpose purpose);
	}
}
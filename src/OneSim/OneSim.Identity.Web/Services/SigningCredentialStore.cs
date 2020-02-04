namespace OneSim.Identity.Web.Services
{
	using System;
	using System.Threading.Tasks;

	using IdentityServer4.Stores;

	using Microsoft.IdentityModel.Tokens;

	using OneSim.Identity.Application.Abstractions;
	using OneSim.Identity.Domain.Entities;

	/// <summary>
	/// 	The implementation of <see cref="ISigningCredentialStore"/>, retrieving <see cref="SecurityKey"/>s from a
	/// 	specified <see cref="ISecurityKeyProvider"/>.
	/// </summary>
	public class SigningCredentialStore : ISigningCredentialStore
	{
		/// <summary>
		/// 	The <see cref="ISecurityKeyProvider"/>.
		/// </summary>
		private readonly ISecurityKeyProvider _securityKeyProvider;

		/// <summary>
		/// 	Initializes a new instance of the <see cref="SigningCredentialStore"/> class.
		/// </summary>
		/// <param name="securityKeyProvider">
		///		The <see cref="ISecurityKeyProvider"/>.
		/// </param>
		public SigningCredentialStore(ISecurityKeyProvider securityKeyProvider) =>
			_securityKeyProvider = securityKeyProvider ??
								   throw new ArgumentNullException(nameof(securityKeyProvider),
																   "The Security Key Provider cannot be null.");

		/// <summary>
		/// 	Gets the <see cref="SigningCredentials"/>.
		/// </summary>
		/// <returns>
		///		The <see cref="Task"/> containing the <see cref="SigningCredentials"/>.
		/// </returns>
		public async Task<SigningCredentials> GetSigningCredentialsAsync()
		{
			// Get the security key
			SecurityKey securityKey = await _securityKeyProvider.GetSecurityKeyAsync(SecurityKeyPurpose.Identity);

			// Convert to Signing Credentials
			SigningCredentials credentials = new SigningCredentials(securityKey, _securityKeyProvider.Algorithm);

			return credentials;
		}
	}
}
namespace OneSim.Identity.Web.Services
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using IdentityServer4.Models;
	using IdentityServer4.Stores;

	using Microsoft.IdentityModel.Tokens;

	/// <summary>
	/// 	The implementation of <see cref="IValidationKeysStore"/>, retrieving <see cref="SecurityKeyInfo"/> from a
	/// 	specified <see cref="SigningCredentialStore"/>.
	/// </summary>
	public class ValidationKeysStore : IValidationKeysStore
	{
		/// <summary>
		/// 	The <see cref="ISigningCredentialStore"/>.
		/// </summary>
		private readonly ISigningCredentialStore _signingCredentialStore;

		/// <summary>
		/// 	Gets the <see cref="IEnumerable{T}"/> of <see cref="SecurityKeyInfo"/>.
		/// </summary>
		private IEnumerable<SecurityKeyInfo> _keys;

		/// <summary>
		/// 	Initializes a new instance of the <see cref="ValidationKeysStore"/> class.
		/// </summary>
		/// <param name="signingCredentialStore">
		///		The <see cref="SigningCredentialStore"/>.
		/// </param>
		public ValidationKeysStore(ISigningCredentialStore signingCredentialStore) =>
			_signingCredentialStore = signingCredentialStore ??
									  throw new ArgumentNullException(nameof(signingCredentialStore),
																	  "The Signing Credential Store cannot be null.");

		/// <summary>
		/// 	Gets all validation keys.
		/// </summary>
		/// <returns>
		///		The <see cref="IEnumerable{T}"/> of <see cref="SecurityKeyInfo"/>.
		/// </returns>
		public async Task<IEnumerable<SecurityKeyInfo>> GetValidationKeysAsync()
		{
			// If no keys exist, create them
			if (_keys == null)
			{
				// Get the credentials from the credential store
				SigningCredentials credentials = await _signingCredentialStore.GetSigningCredentialsAsync();

				// Create new key info
				SecurityKeyInfo info = new SecurityKeyInfo
									   {
										   Key = credentials.Key,
										   SigningAlgorithm = credentials.Algorithm
									   };
				_keys = new[] { info };
			}

			return _keys;
		}
	}
}
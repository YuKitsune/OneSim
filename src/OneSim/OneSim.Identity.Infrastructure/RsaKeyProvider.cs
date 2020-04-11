namespace OneSim.Identity.Infrastructure
{
	using System;
	using System.Security.Cryptography;
	using System.Threading.Tasks;

	using IdentityModel;

	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Logging;
	using Microsoft.IdentityModel.Tokens;

	using Newtonsoft.Json;

	using OneSim.Identity.Application.Abstractions;
	using OneSim.Identity.Domain.Entities;
	using OneSim.Identity.Infrastructure.ContractResolvers;

	/// <summary>
	/// 	The RSA implementation of the <see cref="ISecurityKeyProvider"/>.
	/// </summary>
	public class RsaKeyProvider : ISecurityKeyProvider
	{
		/// <summary>
		/// 	The <see cref="ILogger{TCategoryName}"/>.
		/// </summary>
		private readonly ILogger<RsaKeyProvider> _logger;

		/// <summary>
		/// 	The <see cref="IKeysDbContext"/>.
		/// </summary>
		private readonly IKeysDbContext _dbContext;

		/// <summary>
		/// 	Gets the Key Size.
		/// </summary>
		public int KeySize => 2048;

		/// <summary>
		/// 	Gets the name of the algorithm.
		/// </summary>
		public string Algorithm => $"RSA{KeySize}";

		/// <summary>
		/// 	Initializes a new instance of the <see cref="RsaKeyProvider"/>.
		/// </summary>
		/// <param name="dbContext">
		///     The <see cref="IKeysDbContext"/>.
		/// </param>
		/// <param name="logger">
		/// 	The <see cref="ILogger{TCategoryName}"/>.
		/// </param>
		public RsaKeyProvider(IKeysDbContext dbContext, ILogger<RsaKeyProvider> logger)
		{
			_dbContext = dbContext ??
						 throw new ArgumentNullException(nameof(dbContext), "The Keys DbContext cannot be null");
			_logger = logger ?? throw new ArgumentNullException(nameof(logger), "The Logger cannot be null.");
		}

		/// <summary>
		/// 	Gets the <see cref="SecurityKey"/> for the intended <see cref="SecurityKeyPurpose"/>.
		/// </summary>
		/// <param name="purpose">
		///		The <see cref="SecurityKeyPurpose"/>.
		/// </param>
		/// <returns>
		///		The <see cref="SecurityKey"/>.
		/// </returns>
		public SecurityKey GetSecurityKey(SecurityKeyPurpose purpose) =>
			GetSecurityKeyAsync(purpose).GetAwaiter().GetResult();

		/// <summary>
		/// 	Gets the <see cref="SecurityKey"/> for the intended <see cref="SecurityKeyPurpose"/>.
		/// </summary>
		/// <param name="purpose">
		///		The <see cref="SecurityKeyPurpose"/>.
		/// </param>
		/// <returns>
		///		The <see cref="Task"/> containing the <see cref="SecurityKey"/>.
		/// </returns>
		public async Task<SecurityKey> GetSecurityKeyAsync(SecurityKeyPurpose purpose)
		{
			Key key = await _dbContext.Keys.FirstOrDefaultAsync(k => k.Purpose == purpose);
			if (key == null)
			{
				_logger.LogInformation($"No Security Key for purpose \"{purpose}\" exists. Creating a new key.");

				// If no key exists, create one
				RsaSecurityKey newSecurityKey = CreateRsaSecurityKey();
				string serialisedParameters = JsonConvert.SerializeObject(newSecurityKey.Rsa.ExportParameters(true), new JsonSerializerSettings { ContractResolver = new RsaKeyContractResolver() });

				// Assign to our key variable for later use too
				key = new Key { Purpose = purpose, Data = serialisedParameters };

				await _dbContext.Keys.AddAsync(key);
				_logger.LogInformation("New key created and persisted.");
			}
			else
			{
				_logger.LogInformation("Security Key retrieved.");
			}

			// Get the parameters and create the key
			RSAParameters parameters = JsonConvert.DeserializeObject<RSAParameters>(key.Data, new JsonSerializerSettings { ContractResolver = new RsaKeyContractResolver() });
			RsaSecurityKey securityKey = CreateRsaSecurityKey(parameters, key.SecurityKeyId);

			return securityKey;
		}

		/// <summary>
		/// 	Creates a new <see cref="RsaSecurityKey"/>.
		/// </summary>
		/// <param name="parameters">
		///		The parameters.
		/// </param>
		/// <param name="id">
		/// 	The Key ID.
		/// </param>
		/// <returns>
		///		A new instance of the <see cref="RsaSecurityKey"/> class.
		/// </returns>
		public static RsaSecurityKey CreateRsaSecurityKey(RSAParameters parameters, string id)
		{
			RsaSecurityKey key = new RsaSecurityKey(parameters)
								 {
									 KeyId = id
								 };

			return key;
		}

		/// <summary>
		/// 	Creates a new <see cref="RsaSecurityKey"/>.
		/// </summary>
		/// <returns>
		///		A new instance of the <see cref="RsaSecurityKey"/> class.
		/// </returns>
		public static RsaSecurityKey CreateRsaSecurityKey()
		{
			const int keySize = 2048;

			// Create the RSA key
			RSA rsa = RSA.Create();
			RsaSecurityKey key;

			if (rsa is RSACryptoServiceProvider)
			{
				// If the RSA is a crypto service provider, dispose and do whatever CNG means
				rsa.Dispose();
				RSACng cng = new RSACng(keySize);

				RSAParameters parameters = cng.ExportParameters(includePrivateParameters: true);
				key = new RsaSecurityKey(parameters);
			}
			else
			{
				// Otherwise, set the key size and create a new key
				rsa.KeySize = keySize;
				key = new RsaSecurityKey(rsa);
			}

			// Set the Key ID
			key.KeyId = CryptoRandom.CreateUniqueId(16);

			return key;
		}
	}
}
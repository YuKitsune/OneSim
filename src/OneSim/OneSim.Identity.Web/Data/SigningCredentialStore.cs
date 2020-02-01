namespace OneSim.Identity.Web.Data
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Security.Cryptography.X509Certificates;
	using System.Threading.Tasks;

	using IdentityServer4.Models;
	using IdentityServer4.Stores;

	using Microsoft.EntityFrameworkCore;
	using Microsoft.IdentityModel.Tokens;

	public class SigningCredentialStore : ISigningCredentialStore, IValidationKeysStore
	{
		/// <summary>
		/// 	The <see cref="CertificateDbContext"/>.
		/// </summary>
		private CertificateDbContext _dbContext;

		public SigningCredentialStore(CertificateDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		/// <summary>
		/// 	Gets the <see cref="SigningCredentials"/> as an asynchronous operation.
		/// </summary>
		/// <returns>
		///		The <see cref="Task"/> containing the <see cref="SigningCredentials"/>.
		/// </returns>
		public async Task<SigningCredentials> GetSigningCredentialsAsync()
		{
			// Todo: filter by date and purpose
			Certificate certificate = await _dbContext.Certificates.FirstOrDefaultAsync();

			return new X509SigningCredentials(certificate.GetCertificate());
		}

		/// <summary>
		/// 	Gets the <see cref="IEnumerable{T}"/> of <see cref="SecurityKeyInfo"/> as an asynchronous operation.
		/// </summary>
		/// <returns>
		///		The <see cref="Task"/> containing the <see cref="IEnumerable{T}"/> of <see cref="SecurityKeyInfo"/>.
		/// </returns>
		public async Task<IEnumerable<SecurityKeyInfo>> GetValidationKeysAsync()
		{
			// Todo: filter by date and purpose
			List<Certificate> certificates = await _dbContext.Certificates.Where(c => c.Id == 0).ToListAsync();

			return certificates.Select(c =>
									   {
										   X509Certificate2 certificate = c.GetCertificate();
										   string algorithm = certificate.GetKeyAlgorithm();

										   return new SecurityKeyInfo
												  {
													  Key = new X509SecurityKey(certificate),
													  SigningAlgorithm = algorithm
												  };
									   });
		}
	}
}
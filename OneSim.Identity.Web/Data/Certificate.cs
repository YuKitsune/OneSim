namespace OneSim.Identity.Web.Data
{
	using System;
	using System.Security.Cryptography.X509Certificates;

	// Todo: Need to add purpose property

	/// <summary>
	/// 	The database model for storing a <see cref="X509Certificate2"/> in a database.
	/// </summary>
	public class Certificate
	{
		/// <summary>
		/// 	Gets or sets the ID.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// 	Gets or sets the raw certificate data.
		/// </summary>
		public byte[] RawData { get; set; }

		/// <summary>
		/// 	Gets or sets the <see cref="DateTime"/> at which the current <see cref="Certificate"/>
		/// 	becomes effective.
		/// </summary>
		public DateTime EffectiveDate { get; set; }

		/// <summary>
		/// 	Gets or sets the <see cref="DateTime"/> at which the current <see cref="Certificate"/>
		/// 	expires.
		/// </summary>
		public DateTime ExpiryDate { get; set; }

		/// <summary>
		/// 	Creates a new instance of the <see cref="Certificate"/> class given a <see cref="X509Certificate2"/>.
		/// </summary>
		/// <param name="certificate">
		///		The <see cref="X509Certificate2"/>.
		/// </param>
		/// <returns>
		///		The <see cref="Certificate"/>.
		/// </returns>
		public static Certificate FromX509(X509Certificate2 certificate)
		{
			byte[] rawData = certificate.RawData;

			return new Certificate
				   {
					   RawData = rawData, EffectiveDate = certificate.NotBefore, ExpiryDate = certificate.NotAfter
				   };
		}

		/// <summary>
		/// 	Gets the <see cref="X509Certificate2"/> from the current <see cref="Certificate"/>.
		/// </summary>
		/// <returns>
		///		The <see cref="X509Certificate2"/>.
		/// </returns>
		public X509Certificate2 GetCertificate() => new X509Certificate2(RawData);
	}
}
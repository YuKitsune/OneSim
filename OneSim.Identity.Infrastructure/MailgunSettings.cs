namespace OneSim.Identity.Infrastructure
{
	/// <summary>
	/// 	The Mailgun settings.
	/// </summary>
	public class MailgunSettings
	{
		/// <summary>
		/// 	Gets or sets the API Url.
		/// </summary>
		public string ApiUrl { get; set; }

		/// <summary>
		/// 	Gets or sets the API key.
		/// </summary>
		public string ApiKey { get; set; }

		/// <summary>
		/// 	Gets or sets the domain name of the current application.
		/// </summary>
		public string DomainName { get; set; }
	}
}
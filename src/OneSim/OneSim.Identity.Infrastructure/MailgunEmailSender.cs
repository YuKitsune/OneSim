namespace OneSim.Identity.Infrastructure
{
	using System;
	using System.Net.Http;
	using System.Net.Http.Headers;
	using System.Text;
	using System.Threading.Tasks;

	using Microsoft.Extensions.Configuration;

	using OneSim.Identity.Application.Abstractions;

	/// <summary>
	/// 	The Mailgun <see cref="IEmailSender"/> implementation.
	/// </summary>
	public class MailgunEmailSender : IEmailSender
	{
		/// <summary>
		/// 	The <see cref="MailgunSettings"/>.
		/// </summary>
		private readonly MailgunSettings _settings;

		/// <summary>
		/// 	Initializes a new instance of the <see cref="MailgunEmailSender"/>.
		/// </summary>
		/// <param name="settings">
		///		The <see cref="MailgunSettings"/>.
		/// </param>
		public MailgunEmailSender(MailgunSettings settings) => _settings = settings ?? throw new ArgumentNullException(nameof(settings), "The Mailgun Settings cannot be null.");

		/// <summary>
		/// 	Initializes a new instance of the <see cref="MailgunEmailSender"/>.
		/// </summary>
		/// <param name="configuration">
		///		The <see cref="IConfiguration"/>.
		/// </param>
		public MailgunEmailSender(IConfiguration configuration)
		{
			if (configuration == null) throw new ArgumentNullException(nameof(configuration), "The Configuration cannot be null.");

			MailgunSettings settings = configuration.GetSection("MailgunSettings").Get<MailgunSettings>();
			_settings = settings ?? throw new Exception("Unable to find Mailgun settings.");
		}

		/// <summary>
		/// 	Sends an email to the provided <paramref name="recipientEmail"/>.
		/// </summary>
		/// <param name="recipientEmail">
		///		The recipient.
		/// </param>
		/// <param name="subject">
		///		The subject.
		/// </param>
		/// <param name="message">
		///		The message.
		/// </param>
		/// <returns>
		///		The <see cref="Task"/>.
		/// </returns>
		public async Task SendEmailAsync(string recipientEmail, string subject, string message)
		{
			// Todo: Need to test this works
			using HttpClient httpClient = new HttpClient();
			using MultipartFormDataContent content = new MultipartFormDataContent();

			// Add the API key
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", $"api:{_settings.ApiKey}");

			// Add the parameters?
			content.Add(new StringContent($"OneSim No-Reply <mailgun@{_settings.DomainName}>"), "from");
			content.Add(new StringContent(recipientEmail), "to");
			content.Add(new StringContent(subject), "subject");
			content.Add(new StringContent(message), "text");

			await httpClient.PostAsync(new Uri($"{_settings.ApiUrl}/{_settings.DomainName}/messages"), content);
		}
	}
}
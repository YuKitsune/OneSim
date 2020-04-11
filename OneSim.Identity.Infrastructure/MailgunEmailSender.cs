namespace OneSim.Identity.Infrastructure
{
	using System;
	using System.IO;
	using System.Net.Http;
	using System.Net.Http.Headers;
	using System.Text;
	using System.Threading.Tasks;

	using Microsoft.Extensions.Configuration;

	using OneSim.Identity.Application.Abstractions;

	using RestSharp;
	using RestSharp.Authenticators;

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
			_settings = settings ?? throw new Exception("Couldn't find the \"MailgunSettings\" section in the configuration.");
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
			// Todo: Swap out RestSharp for something native
			RestClient client = new RestClient
								{
									BaseUrl = new Uri("https://api.mailgun.net/v3"),
									Authenticator = new HttpBasicAuthenticator("api", _settings.ApiKey)
								};
			RestRequest request = new RestRequest();
			request.AddParameter("domain", _settings.DomainName, ParameterType.UrlSegment);
			request.Resource = "{domain}/messages";
			request.AddParameter("from", $"OneSim <mailgun@{_settings.DomainName}>");
			request.AddParameter("to", recipientEmail);
			request.AddParameter("subject", subject);
			request.AddParameter("text", message);
			request.Method = Method.POST;

			// Todo: Log response
			IRestResponse response = await client.ExecuteTaskAsync(request);
		}
	}
}
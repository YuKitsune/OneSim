namespace OneSim.Identity.Infrastructure
{
    using System;
    using System.Threading.Tasks;

	using MailKit.Net.Smtp;

	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.Logging;

	using MimeKit;

	using OneSim.Identity.Application.Abstractions;

	/// <summary>
	/// 	The SMTP based email sender.
	/// </summary>
	public class SmtpEmailSender : IEmailSender
	{
		/// <summary>
		/// 	The <see cref="SmtpSettings"/>.
		/// </summary>
		private readonly SmtpSettings _settings;

		/// <summary>
		/// 	The <see cref="ILogger{TCategoryName}"/>.
		/// </summary>
		private readonly ILogger<SmtpEmailSender> _logger;

		/// <summary>
		/// 	Initializes a new instance of the <see cref="SmtpEmailSender"/> class.
		/// </summary>
		/// <param name="settings">
		///		The <see cref="SmtpSettings"/>.
		/// </param>
		/// <param name="logger">
		///		The <see cref="ILogger{TCategoryName}"/>.
		/// </param>
		public SmtpEmailSender(SmtpSettings settings, ILogger<SmtpEmailSender> logger)
		{
			_settings = settings ??
						throw new ArgumentNullException(nameof(settings), "The SMTP settings cannot be null.");
			_logger = logger ??
					  throw new ArgumentNullException(nameof(logger), "The Logger cannot be null.");
		}

		/// <summary>
		/// 	Initializes a new instance of the <see cref="SmtpEmailSender"/> class.
		/// </summary>
		/// <param name="configuration">
		///		The <see cref="IConfiguration"/> containing <see cref="SmtpSettings"/>.
		/// </param>
		/// <param name="logger">
		///		The <see cref="ILogger{TCategoryName}"/>.
		/// </param>
		public SmtpEmailSender(IConfiguration configuration, ILogger<SmtpEmailSender> logger)
		{
			// Check the inputs
			if (configuration == null) throw new ArgumentNullException(nameof(configuration), "The Configuration Object cannot be null.");

			_logger = logger ??
					  throw new ArgumentNullException(nameof(logger), "The Logger cannot be null.");

			// Get the SMTP settings from the Configuration
			SmtpSettings settings = configuration.GetSection("SmtpSettings").Get<SmtpSettings>();
			_settings = settings ?? throw new Exception("Unable to find SMTP settings.");
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
			// Create the mime message
			MimeMessage mimeMessage = new MimeMessage();

			// Define where the message is from
			MailboxAddress from = new MailboxAddress("OneSim No-reply", _settings.NoReplyAddress);
			mimeMessage.From.Add(from);

			// Define where the message should go
			MailboxAddress to = new MailboxAddress(recipientEmail);
			mimeMessage.To.Add(to);

			// Set the subject and body
			mimeMessage.Subject = subject;

			BodyBuilder builder = new BodyBuilder { TextBody = message };
			mimeMessage.Body = builder.ToMessageBody();

			// Get the SMTP client

			using (_logger.BeginScope("Sending Email"))
			{
				_logger.LogInformation($"Sending message to \"{recipientEmail}\" with subject \"{mimeMessage.Subject}\" and body \"{mimeMessage.TextBody}\".");
				using SmtpClient client = GetClient();

				// Send the message
				await client.SendAsync(mimeMessage);
			}
		}

		/// <summary>
		/// 	Gets the <see cref="SmtpClient"/>.
		/// </summary>
		/// <param name="connect">
		///		When set to <c>true</c>, the <see cref="SmtpClient"/> will attempt to connect to the SMTP server prior
		/// 	to returning.
		/// 	Will attempt by default.
		/// </param>
		/// <param name="authenticate">
		///		When set to <c>true</c>, the <see cref="SmtpClient"/> will attempt to authenticate with the SMTP server
		/// 	prior to returning.
		/// 	Will attempt by default.
		/// </param>
		/// <returns>
		///		The <see cref="SmtpClient"/>.
		/// </returns>
		private SmtpClient GetClient(bool connect = true, bool authenticate = true)
		{
			using (_logger.BeginScope("Creating SMTP Client"))
			{
				SmtpClient client = new SmtpClient();

				// Return the client now if we haven't been asked to connect
				if (!connect)
				{
					_logger.LogInformation("Returning client without connecting to SMTP server.");

					return client;
				}

				// Connect, then authenticate if requested
				_logger.LogInformation("Connecting to SMTP server.");
				client.Connect(_settings.Server, _settings.PortNumber, true);
				if (authenticate)
				{
					_logger.LogInformation("Authenticating with SMTP server.");
					client.Authenticate(_settings.Username, _settings.Password);
				}

				_logger.LogInformation("Connected and authenticated.");

				return client;
			}
		}
	}
}
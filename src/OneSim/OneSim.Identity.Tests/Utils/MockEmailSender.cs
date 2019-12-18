namespace OneSim.Identity.Tests.Utils
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using OneSim.Identity.Application.Interfaces;

	/// <summary>
	/// 	The mock <see cref="IEmailSender"/>.
	/// </summary>
	public class MockEmailSender : IEmailSender
	{
		/// <summary>
		/// 	Gets or sets the sent <see cref="Message"/>s.
		/// </summary>
		public List<Message> SentMessages { get; set; }

		/// <summary>
		/// 	Initializes a new instance of the <see cref="MockEmailSender"/> class.
		/// </summary>
		public MockEmailSender() => SentMessages = new List<Message>();

		/// <summary>
		/// 	Sends an email.
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
			await Task.Yield();
			SentMessages.Add(new Message
							 {
								 Recipient = recipientEmail,
								 Subject = subject,
								 Content = message
							 });
		}

		/// <summary>
		/// 	The email message.
		/// </summary>
		public class Message
		{
			/// <summary>
			/// 	Gets or sets the message recipient.
			/// </summary>
			public string Recipient { get; set; }

			/// <summary>
			/// 	Gets or sets the message subject.
			/// </summary>
			public string Subject { get; set; }

			/// <summary>
			/// 	Gets or sets the message content.
			/// </summary>
			public string Content { get; set; }
		}
	}
}
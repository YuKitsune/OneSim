namespace OneSim.Identity.Application.Interfaces
{
	using System.Threading.Tasks;

	/// <summary>
	/// 	The Email Sender.
	/// </summary>
	public interface IEmailSender
	{
		/// <summary>
		///		Sends the email.
		/// </summary>
		/// <param name="recipientEmail">
		///		The recipient address.
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
		Task SendEmailAsync(string recipientEmail, string subject, string message);
	}
}
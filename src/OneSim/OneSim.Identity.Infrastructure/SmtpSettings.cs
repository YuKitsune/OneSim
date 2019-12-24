namespace OneSim.Identity.Infrastructure
{
	/// <summary>
	/// 	The SMTP settings.
	/// </summary>
	public class SmtpSettings
	{
		/// <summary>
		/// 	Gets or sets the IP address or DNS name of the SMTP server.
		/// </summary>
		public string Server { get; set; }

		/// <summary>
		/// 	Gets or sets the port number of the SMTP server.
		/// </summary>
		public int PortNumber { get; set; }

		/// <summary>
		/// 	Gets or sets the no-reply address.
		/// </summary>
		public string NoReplyAddress { get; set; }

		/// <summary>
		/// 	Gets or sets the username.
		/// </summary>
		public string Username { get; set; }

		/// <summary>
		/// 	Gets or sets the password.
		/// </summary>
		public string Password { get; set; }
	}
}
namespace OneSim.Identity.Web.Models.ViewModels.Account
{
	/// <summary>
	/// 	The View Model to use on the account management index page.
	/// </summary>
	public class IndexViewModel
	{
		/// <summary>
		/// 	Gets or sets the message.
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// 	Gets or sets a value indicating whether or not the <see cref="Message"/> is an error.s
		/// </summary>
		public bool MessageIsError { get; set; }

		/// <summary>
		/// 	Initializes a new instance of the <see cref="IndexViewModel"/>.
		/// </summary>
		public IndexViewModel() => MessageIsError = false;
	}
}
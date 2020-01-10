namespace OneSim.Auth.Controllers
{
	using System.Threading.Tasks;

	using IdentityServer4.Models;
	using IdentityServer4.Services;

	using Microsoft.AspNetCore.Mvc;

	using OneSim.Auth.Models;
	using OneSim.Auth.Services;

	/// <summary>
	/// 	The Home <see cref="Controller"/>.
	/// </summary>
	public class HomeController : Controller
	{
		/// <summary>
		/// 	The <see cref="IIdentityServerInteractionService"/>.
		/// </summary>
		private readonly IIdentityServerInteractionService _interactionService;

		/// <summary>
		/// 	The <see cref="IRedirectService"/>.
		/// </summary>
		private readonly IRedirectService _redirectService;

		/// <summary>
		/// 	Initializes a new instance of the <see cref="HomeController"/> class.
		/// </summary>
		/// <param name="interactionService">
		///		The <see cref="IIdentityServerInteractionService"/>.
		/// </param>
		/// <param name="redirectService">
		///		The <see cref="IRedirectService"/>.
		/// </param>
		public HomeController(IIdentityServerInteractionService interactionService, IRedirectService redirectService)
		{
			_interactionService = interactionService;
			_redirectService = redirectService;
		}

		/// <summary>
		/// 	Returns the index view.
		/// </summary>
		/// <param name="returnUrl">
		///		The return URL.
		/// </param>
		/// <returns>
		///		The <see cref="IActionResult"/>.
		/// </returns>
		public IActionResult Index(string returnUrl) => View();

		/// <summary>
		/// 	Redirects to the original application.
		/// </summary>
		/// <param name="returnUrl">
		///		The return URL.
		/// </param>
		/// <returns>
		///		The <see cref="IActionResult"/>.
		/// </returns>
		public IActionResult ReturnToOriginalApplication(string returnUrl)
		{
			// If a url was provided, then redirect to the extracted URL, otherwise, redirect to the index view.
			if (string.IsNullOrEmpty(returnUrl)) return RedirectToAction("Index", "Home");

			return Redirect(_redirectService.ExtractRedirectUriFromReturnUrl(returnUrl));
		}

		/// <summary>
		/// 	Shows the error page
		/// </summary>
		public async Task<IActionResult> Error(string errorId)
		{
			ErrorViewModel vm = new ErrorViewModel();

			// Retrieve error details from IdentityServer
			ErrorMessage message = await _interactionService.GetErrorContextAsync(errorId);
			if (message != null)
			{
				vm.Error = message;
			}

			return View("Error", vm);
		}
	}
}
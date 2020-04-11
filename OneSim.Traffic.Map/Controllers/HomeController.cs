namespace OneSim.Traffic.Map.Controllers
{
	using Microsoft.AspNetCore.Mvc;

	/// <summary>
	/// 	The Home <see cref="Controller"/>.
	/// </summary>
	public class HomeController : Controller
	{
		/// <summary>
		/// 	The index page.
		/// </summary>
		/// <returns>
		///		The <see cref="IActionResult"/>.
		/// </returns>
		public IActionResult Index() => View();
	}
}
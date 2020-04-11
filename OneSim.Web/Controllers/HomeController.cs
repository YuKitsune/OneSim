namespace OneSim.Web.Controllers
{
    using System.Diagnostics;

    using Microsoft.AspNetCore.Mvc;

    using OneSim.Web.Models;

    /// <summary>
    ///     The Home <see cref="Controller"/>.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        ///     The index page.
        /// </summary>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        public IActionResult Index() => View();

        /// <summary>
        ///     The about page.
        /// </summary>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        public IActionResult About() => View();

        /// <summary>
        ///     The contact page.
        /// </summary>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        public IActionResult Contact() => View();

        /// <summary>
        ///     The privacy policy page.
        /// </summary>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        public IActionResult Privacy() => View();

        /// <summary>
        ///     The error page.
        /// </summary>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

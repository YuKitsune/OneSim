// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeController.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     The Home <see cref="Controller"/>.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        ///     The <see cref="ILogger{TCategoryName}"/> for the <see cref="HomeController"/>.
        /// </summary>
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        ///     Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="logger">
        ///     The <see cref="ILogger{TCategoryName}"/> for the <see cref="HomeController"/>.
        /// </param>
        public HomeController(ILogger<HomeController> logger) => _logger = logger;

        /// <summary>
        ///     Gets the index view.
        /// </summary>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        public IActionResult Index() => View();
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeController.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Map.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    ///     The Home <see cref="Controller"/>.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        ///     The index page.
        /// </summary>
        /// <returns>
        ///        The <see cref="IActionResult"/>.
        /// </returns>
        public IActionResult Index() => View();
    }
}

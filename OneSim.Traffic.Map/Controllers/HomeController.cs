// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeController.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Map.Controllers
{
    using Microsoft.AspNetCore.Authorization;
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
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [Authorize]
        public IActionResult Index() => View();

        /// <summary>
        ///     The OIDC Sign-In callback action.
        /// </summary>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        public IActionResult OidcSignInCallback()
        {
            // Todo: What do i even do here?
            return Ok();
        }

        /// <summary>
        ///     The OIDC Sign-Out callback action.
        /// </summary>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        public IActionResult OidcSignOutCallback()
        {
            // Todo: What do i even do here?
            return Ok();
        }
    }
}

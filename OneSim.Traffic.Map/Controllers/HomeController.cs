// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeController.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Map.Controllers
{
    using System.Threading.Tasks;

    using IdentityModel;

    using IdentityServer4.Validation;

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

        public IActionResult Logout()
        {
            // Todo: Only allow from whitlisted servers
            return SignOut("Cookies", "oidc");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace OneSim.Api.Data.RouteHelpers.Identity
{
    /// <summary>
    ///     The Authentication Controller routes.
    /// </summary>
    public class Authentication
    {
        /// <summary>
        ///     The Authentication Controller route.
        /// </summary>
        public const string ControllerRoute = "Authentication";

        /// <summary>
        ///     The LogIn route.
        /// </summary>
        public const string LogIn = "LogIn";

        /// <summary>
        ///     The TwoFactorAuthenticationLogIn route.
        /// </summary>
        public const string TwoFactorAuthenticationLogIn = "TwoFactorAuthenticationLogIn";

        /// <summary>
        ///     The RecoveryCodeLogIn route.
        /// </summary>
        public const string RecoveryCodeLogIn = "RecoveryCodeLogIn";
    }
}

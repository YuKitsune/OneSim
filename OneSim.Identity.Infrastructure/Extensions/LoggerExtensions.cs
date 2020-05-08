// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggerExtensions.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Infrastructure.Extensions
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;

    using OneSim.Identity.Domain;

    /// <summary>
    ///     The <see cref="ILogger"/> extension methods.
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        ///     Writes a the <see cref="SignInResult"/> for the <see cref="IUser"/> to the
        ///     <see cref="ILogger"/>.
        /// </summary>
        /// <param name="logger">
        ///     The <see cref="ILogger"/> to log to.
        /// </param>
        /// <param name="user">
        ///     The <see cref="IUser"/> who the <paramref name="signInResult"/> relates to.
        /// </param>
        /// <param name="signInResult">
        ///     The <see cref="SignInResult"/>.
        /// </param>
        public static void LogResult(this ILogger logger, IUser user, SignInResult signInResult)
        {
            if (signInResult.Succeeded)
            {
                logger.LogInformation($"{user.UserName} has signed-in.");
            }
            else if (signInResult.IsLockedOut)
            {
                logger.LogWarning($"{user.UserName} has been locked out.");
            }
            else if (signInResult.RequiresTwoFactor)
            {
                logger.LogInformation($"{user.UserName} requires Two-Factor Authentication.");
            }
            else
            {
                logger.LogWarning($"Failed to log in {user.UserName}.");
            }
        }
    }
}

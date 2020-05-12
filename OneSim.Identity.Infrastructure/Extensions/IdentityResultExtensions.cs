// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdentityResultExtensions.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Infrastructure.Extensions
{
    using System.Linq;

    using Microsoft.AspNetCore.Identity;

    using OneSim.Identity.Infrastructure.Exceptions;

    /// <summary>
    ///     The <see cref="IdentityResult"/> extension methods.
    /// </summary>
    public static class IdentityResultExtensions
    {
        /// <summary>
        ///     If the <see cref="IdentityResult"/> has any <see cref="IdentityError"/>s, then this will wrap them in an
        ///     <see cref="IdentityException"/> and throw.
        /// </summary>
        /// <param name="result">
        ///     The <see cref="IdentityResult"/>.
        /// </param>
        public static void ThrowIfAnyErrors(this IdentityResult result)
        {
            // Ignore if the result was succeeded, or no errors were found
            if (result.Succeeded ||
                !result.Errors.Any())
                return;

            // Throw otherwise
            int errorCount = result.Errors.Count();
            string message = errorCount > 1 ?
                                 $"{errorCount.ToString()} IdentityErrors have occurred." :
                                 $"An IdentityError has occurred: {result.Errors.First().Description}";

            throw new IdentityException(result.Errors, message);
        }
    }
}

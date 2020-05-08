// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdentityException.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Infrastructure.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.AspNetCore.Identity;

    /// <summary>
    ///     The <see cref="Exception"/> which encapsulates <see cref="IdentityError"/>s.
    /// </summary>
    public class IdentityException : Exception
    {
        /// <summary>
        ///     Gets the <see cref="IReadOnlyCollection{T}"/> of <see cref="IdentityError"/>s.
        /// </summary>
        public IReadOnlyCollection<IdentityError> Errors { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="IdentityException"/> class.
        /// </summary>
        /// <param name="errors">
        ///     The <see cref="IdentityError"/>s.
        /// </param>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <param name="innerException">
        ///     The inner <see cref="Exception"/>.
        /// </param>
        public IdentityException(
            IEnumerable<IdentityError> errors,
            string message = null,
            Exception innerException = null)
            : base(message, innerException)
            => Errors = errors.ToList().AsReadOnly();
    }
}

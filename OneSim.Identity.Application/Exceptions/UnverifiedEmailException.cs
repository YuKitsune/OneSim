// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnverifiedEmailException.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Application.Exceptions
{
    using System;

    /// <summary>
    ///     The <see cref="Exception"/> indicating that a particular email address is not verified when it should be.
    /// </summary>
    public class UnverifiedEmailException : Exception
    {
        /// <summary>
        ///     Gets the Id of the user whose email is not verified.
        /// </summary>
        public string UserId { get; }

        /// <summary>
        ///     Gets the email address which has not been verified.
        /// </summary>
        public string UnverifiedEmail { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UnverifiedEmailException"/> class.
        /// </summary>
        /// <param name="userId">
        ///     The ID of the user whose email is not verified.
        /// </param>
        /// <param name="email">
        ///     The email address which has not been verified.
        /// </param>
        /// <param name="message">
        ///     The message.
        /// </param>
        public UnverifiedEmailException(string userId, string email, string message = null)
            : base(message ?? $"The email address \"{email}\" for user \"{userId}\" has not been verified.")
        {
            if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId));
            if (string.IsNullOrEmpty(email)) throw new ArgumentNullException(nameof(email));

            UserId = userId;
            UnverifiedEmail = email;
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidClientTypeException.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Infrastructure.Exceptions
{
    using System;

    /// <summary>
    ///     The Invalid Client Type <see cref="Exception"/>.
    /// </summary>
    public class InvalidClientTypeException : Exception
    {
        /// <summary>
        ///     Gets the invalid client line.
        /// </summary>
        public string ClientLine { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="InvalidClientTypeException"/> class.
        /// </summary>
        /// <param name="clientLine">
        ///        The invalid client line.
        /// </param>
        /// <param name="message">
        ///        The message.
        /// </param>
        /// <param name="innerException">
        ///        The inner <see cref="Exception"/> if any.
        /// </param>
        public InvalidClientTypeException(string clientLine, string message, Exception innerException = null)
            : base(message, innerException)
        {
            if (string.IsNullOrEmpty(clientLine)) throw new ArgumentNullException(nameof(clientLine), "The Client Line cannot be empty.");

            ClientLine = clientLine;
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidTwoFactorAuthenticationCodeException.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Infrastructure.Exceptions
{
    using System;

    /// <summary>
    ///     The <see cref="Exception"/> for when an invalid Two-Factor Authentication Code has been provided.
    /// </summary>
    public class InvalidTwoFactorAuthenticationCodeException : Exception
    {
        /// <summary>
        ///     Gets the invalid Two-Factor Authentication code.
        /// </summary>
        public string Code { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="InvalidTwoFactorAuthenticationCodeException"/> class..
        /// </summary>
        /// <param name="code">
        ///     The invalid Two-Factor Authentication code.
        /// </param>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <param name="innerException">
        ///     The inner <see cref="Exception"/>.
        /// </param>
        public InvalidTwoFactorAuthenticationCodeException(
            string code,
            string message = null,
            Exception innerException = null)
            : base(message ?? $"The Two-Factor Authentication code \"{code}\" is invalid.", innerException) => Code = code;
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrafficDataParseError.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Application
{
    using System;

    /// <summary>
    ///     Represents an error which has occurred while parsing traffic data.
    /// </summary>
    public class TrafficDataParseError
    {
        /// <summary>
        ///     Gets the error message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        ///     Gets the content of the traffic data, preferably trimmed to give context to the error message.
        /// </summary>
        public string ContextualContent { get; }

        /// <summary>
        ///     Gets the <see cref="Exception"/> that occurred when parsing if any.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TrafficDataParseError"/> class.
        /// </summary>
        /// <param name="message">
        ///     The error message.
        /// </param>
        /// <param name="contextualContent">
        ///     The content of the traffic data, preferably trimmed to give context to the error message.
        /// </param>
        /// <param name="exception">
        ///     The <see cref="Exception"/> that occurred when parsing if any.
        /// </param>
        public TrafficDataParseError(string message, string contextualContent = "", Exception exception = null)
        {
            ContextualContent = contextualContent;
            Message = message;
            Exception = exception;
        }
    }
}

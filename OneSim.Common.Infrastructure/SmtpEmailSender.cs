// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SmtpEmailSender.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Common.Infrastructure
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using OneSim.Common.Application.Abstractions;

    /// <summary>
    ///     The SMTP implementation of the <see cref="IEmailSender"/> interface.
    /// </summary>
    public class SmtpEmailSender : IEmailSender
    {
        /// <summary>
        ///     Sends an email with a HTML body as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TModel">
        ///     The type of view model.
        /// </typeparam>
        /// <param name="recipient">
        ///     The email address of the recipient.
        /// </param>
        /// <param name="subject">
        ///     The subject of the email.
        /// </param>
        /// <param name="viewName">
        ///     The name of the view to use for the email body.
        /// </param>
        /// <param name="viewModel">
        ///     The <typeparamref name="TModel"/> for the <paramref name="viewName"/>.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken"/> to observe while waiting for the <see cref="Task{T}"/> to complete.
        /// </param>
        /// <returns>
        ///     The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        public async Task SendAsync<TModel>(
            string recipient,
            string subject,
            string viewName,
            TModel viewModel,
            CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Sends the email with a plain-text body as an asynchronous operation.
        /// </summary>
        /// <param name="recipient">
        ///     The email address of the recipient.
        /// </param>
        /// <param name="subject">
        ///     The subject of the email.
        /// </param>
        /// <param name="body">
        ///     The plain-text content of the email.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken"/> to observe while waiting for the <see cref="Task{T}"/> to complete.
        /// </param>
        /// <returns>
        ///     The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        public async Task SendAsync(
            string recipient,
            string subject,
            string body,
            CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            throw new NotImplementedException();
        }
    }
}

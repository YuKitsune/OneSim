// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MockEmailSender.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Tests.Mocks
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using OneSim.Common.Application.Abstractions;

    /// <summary>
    ///     The mock <see cref="IEmailSender"/>.
    /// </summary>
    public class MockEmailSender : IEmailSender
    {
        /// <summary>
        ///     Gets the sent <see cref="Message"/>s.
        /// </summary>
        public List<Message> SentMessages { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MockEmailSender"/> class.
        /// </summary>
        public MockEmailSender() => SentMessages = new List<Message>();

        /// <summary>
        ///     Sends an email.
        /// </summary>
        /// <typeparam name="TModel">
        ///     The type of view model.
        /// </typeparam>
        /// <param name="recipient">
        ///     The recipient.
        /// </param>
        /// <param name="subject">
        ///     The subject.
        /// </param>
        /// <param name="viewName">
        ///     The message body.
        /// </param>
        /// <param name="viewModel">
        ///     The <typeparamref name="TModel"/>.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken"/> to observe while waiting for the <see cref="Task"/> to complete.
        /// </param>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        public async Task SendAsync<TModel>(
            string recipient,
            string subject,
            string viewName,
            TModel viewModel,
            CancellationToken cancellationToken = default) =>
            await SendAsync(recipient, subject, viewName, cancellationToken);

        /// <summary>
        ///     Sends an email.
        /// </summary>
        /// <param name="recipient">
        ///     The recipient.
        /// </param>
        /// <param name="subject">
        ///     The subject.
        /// </param>
        /// <param name="body">
        ///     The message body.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken"/> to observe while waiting for the <see cref="Task{T}"/> to complete.
        /// </param>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        public async Task SendAsync(
            string recipient,
            string subject,
            string body,
            CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            SentMessages.Add(new Message { Recipient = recipient, Subject = subject, Content = body });
        }

        /// <summary>
        ///     The email message.
        /// </summary>
        public class Message
        {
            /// <summary>
            ///     Gets or sets the message recipient.
            /// </summary>
            public string Recipient { get; set; }

            /// <summary>
            ///     Gets or sets the message subject.
            /// </summary>
            public string Subject { get; set; }

            /// <summary>
            ///     Gets or sets the message content.
            /// </summary>
            public string Content { get; set; }
        }
    }
}

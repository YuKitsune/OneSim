// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MockTrafficNotifier.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Tests.Mocks
{
    using System.Threading.Tasks;

    using OneSim.Traffic.Application.Abstractions;

    /// <summary>
    ///     The mock <see cref="ITrafficNotifier"/>.
    /// </summary>
    public class MockTrafficNotifier : ITrafficNotifier
    {
        /// <summary>
        ///     Gets the amount of notifications sent.
        /// </summary>
        public int NotificationCount { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MockTrafficNotifier"/> class.
        /// </summary>
        public MockTrafficNotifier() => NotificationCount = 0;

        /// <summary>
        ///     Notifies the subscribers that there is new traffic data available.
        /// </summary>
        public void NewTrafficDataAvailable() => NewTrafficDataAvailableAsync().GetAwaiter().GetResult();

        /// <summary>
        ///     Notifies the subscribers that there is new traffic data available as an asynchronous operation.
        /// </summary>
        /// <returns>
        ///     A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        public async Task NewTrafficDataAvailableAsync()
        {
            await Task.Yield();
            NotificationCount++;
        }
    }
}

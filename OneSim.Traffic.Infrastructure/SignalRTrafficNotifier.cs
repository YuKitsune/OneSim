// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SignalRTrafficNotifier.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Infrastructure
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.SignalR;

    using OneSim.Traffic.Application.Abstractions;

    /// <summary>
    ///     The SignalR <see cref="ITrafficNotifier"/> implementation for sending traffic data notifications over a
    ///     SignalR connection.
    /// </summary>
    public class SignalRTrafficNotifier : ITrafficNotifier
    {
        /// <summary>
        ///     The <see cref="IHubContext{THub}"/> for the <see cref="TrafficDataHub"/>.
        /// </summary>
        private readonly IHubContext<TrafficDataHub> _hubContext;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SignalRTrafficNotifier"/> class.
        /// </summary>
        /// <param name="hubContext">
        ///        The <see cref="IHubContext{THub}"/> for the <see cref="TrafficDataHub"/>.
        /// </param>
        public SignalRTrafficNotifier(IHubContext<TrafficDataHub> hubContext) => _hubContext = hubContext ??
            throw new ArgumentNullException(nameof(hubContext), "The HubContext cannot be null");

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
            await _hubContext.Clients.All.SendAsync("NewTrafficDataAvailable");
        }
    }
}

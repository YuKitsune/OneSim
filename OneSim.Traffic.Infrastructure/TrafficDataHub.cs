// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrafficDataHub.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Infrastructure
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.SignalR;

    /// <summary>
    ///     The SignalR <see cref="Hub"/> for communicating traffic data updates and user actions on the map.
    /// </summary>
    public class TrafficDataHub : Hub
    {
        /// <summary>
        ///     Relays a message from the WebUI to the desktop client, notifying the client that a pilot has been
        ///     selected.
        /// </summary>
        /// <param name="callsign">
        ///     The callsign of the pilot.
        /// </param>
        /// <returns>
        ///     The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        public async Task PilotSelected(string callsign)
        {
            // Todo: Get the user who clicked the pilot
            await Clients.All.SendAsync("pilotSelected", callsign);
        }
    }
}

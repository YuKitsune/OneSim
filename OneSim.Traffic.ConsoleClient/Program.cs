// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.ConsoleClient
{
    using System;
    using System.Net;
    using System.Net.Http;

    using Microsoft.AspNetCore.SignalR.Client;

    /// <summary>
    ///     The program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        ///     Gets or sets the <see cref="HubConnection"/>.
        /// </summary>
        public static HubConnection Connection { get; set; }

        /// <summary>
        ///     Gets or sets the domain name.
        /// </summary>
        public static string DomainName { get; set; }

        /// <summary>
        ///     The main application entry point.
        /// </summary>
        /// <param name="args">
        ///     The arguments.
        /// </param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Console.WriteLine("Traffic API server domain: ");
            DomainName = Console.ReadLine();

            string url = DomainName + "/TrafficDataHub";
            Console.WriteLine($"Building connection for \"{url}\".");
            Connection = new HubConnectionBuilder()
                        .WithUrl(
                             url,
                             conf => conf.HttpMessageHandlerFactory = (x) => new HttpClientHandler
                                                                             {
                                                                                 ServerCertificateCustomValidationCallback
                                                                                     = HttpClientHandler
                                                                                        .DangerousAcceptAnyServerCertificateValidator,
                                                                             })
                        .Build();

            Connection.StartAsync()
                      .ContinueWith(
                           task =>
                           {
                               if (task.IsFaulted)
                               {
                                   Console.WriteLine(
                                       $"There was an error opening the connection:{task.Exception.GetBaseException()}");
                               }
                               else
                               {
                                   Console.WriteLine("Connected");
                               }
                           })
                      .Wait();

            Connection.On("NewTrafficDataAvailable", () => Console.WriteLine("New Traffic Data Available."));
            Connection.On("PilotSelected", (string callsign) => GetPilotInfo(callsign));

            Console.Read();
            Connection.StopAsync();
        }

        /// <summary>
        ///     Gets the pilot info.
        /// </summary>
        /// <param name="callsign">
        ///     The callsign.
        /// </param>
        private static async void GetPilotInfo(string callsign)
        {
            using WebClient client = new WebClient();
            string pilotJson = await client.DownloadStringTaskAsync(DomainName + "/TrafficData/Pilot?callsign=" + callsign);
            Console.WriteLine(pilotJson);
        }
    }
}

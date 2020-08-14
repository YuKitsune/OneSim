// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseClient.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities
{
    using System;
    using System.Diagnostics;

    /// <summary>
    ///     The base OFSN client.
    /// </summary>
    [DebuggerDisplay("{Callsign}")]
    public abstract class BaseClient
    {
        /// <summary>
        ///     Gets or sets the ID.
        /// </summary>
        /// <remarks>
        ///     This is the ID used for storing the current <see cref="BaseClient"/> instance in a relational database.
        /// </remarks>
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the clients OFSN ID.
        /// </summary>
        public string NetworkId { get; set; }

        /// <summary>
        ///     Gets or sets the logon callsign.
        /// </summary>
        public string Callsign { get; set; }

        /// <summary>
        ///     Gets or sets the real name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="OneSim.Traffic.Domain.Entities.Server.NetworkIdentifier"/> of the server the
        ///     client has connected to.
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="AdministrativeRating"/>.
        /// </summary>
        public AdministrativeRating AdministrativeRating { get; set; }

        /// <summary>
        ///     Gets or sets the UTC <see cref="DateTimeOffset"/> at which the current <see cref="BaseClient"/>
        ///     connected to the network.
        /// </summary>
        public DateTimeOffset LogonTime { get; set; }

        /// <summary>
        ///     Gets the <see cref="TimeSpan"/> representing how long the current <see cref="BaseClient"/> has been
        ///     connected to the network for.
        /// </summary>
        public TimeSpan TotalTimeOnline => DateTimeOffset.UtcNow - LogonTime;
    }
}

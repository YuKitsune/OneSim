// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WhazzupDataFileSection.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Infrastructure
{
    using System;

    /// <summary>
    ///     The sections of the VATSIM traffic data file.
    /// </summary>
    public enum WhazzupDataFileSection
    {
        /// <summary>
        ///     The general information section.
        /// </summary>
        General,

        /// <summary>
        ///     The voice servers section.
        /// </summary>
        [Obsolete("VATSIM no longer uses voice servers. However the section still exists.")]
        VoiceServers,

        /// <summary>
        ///     The clients section.
        /// </summary>
        Clients,

        /// <summary>
        ///     The servers section.
        /// </summary>
        Servers,

        /// <summary>
        ///     The prefile section.
        /// </summary>
        Prefile
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetworkType.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities
{
    /// <summary>
    ///     The Online Flight Simulation Network.
    /// </summary>
    public enum NetworkType
    {
        /// <summary>
        ///     The VATSIM Network
        /// </summary>
        Vatsim,

        /// <summary>
        ///     The IVAO Network.
        /// </summary>
        Ivao,

        /// <summary>
        ///     The PilotEdge Network.
        /// </summary>
        PilotEdge,

        /// <summary>
        ///     The POSCON Network
        /// </summary>
        Poscon
    }
}

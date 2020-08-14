// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavaidType.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities.Aeronautical
{
    /// <summary>
    ///     The type of radio navigational aid.
    /// </summary>
    public enum NavaidType
    {
        /// <summary>
        ///     VHF Omnidirectional Range.
        /// </summary>
        Vor,

        /// <summary>
        ///     Non-directional Beacon.
        /// </summary>
        Ndb
    }
}

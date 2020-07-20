// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AirwayDirection.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities.Ais
{
    /// <summary>
    ///     The possible directions an airway can be flown.
    /// </summary>
    public enum AirwayDirection
    {
        /// <summary>
        ///     The airway can only be flown in one direction.
        /// </summary>
        UniDirectional,

        /// <summary>
        ///     The airway can be flown in either direction.
        /// </summary>
        OnmiDirectional
    }
}

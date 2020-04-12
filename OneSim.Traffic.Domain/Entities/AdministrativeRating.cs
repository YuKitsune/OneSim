// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdministrativeRating.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities
{
    /// <summary>
    ///     The OFSN Administrative Rating.
    /// </summary>
    public enum AdministrativeRating
    {
        /// <summary>
        ///     User is Suspended.
        /// </summary>
        Suspended = 0,

        /// <summary>
        ///     User is an Observer.
        /// </summary>
        Observer = 1,

        /// <summary>
        ///     Normal user.
        /// </summary>
        User = 2,

        /// <summary>
        ///     User is a Supervisor.
        /// </summary>
        Supervisor = 11,

        /// <summary>
        ///     User is an Administrator
        /// </summary>
        Administrator = 12
    }
}

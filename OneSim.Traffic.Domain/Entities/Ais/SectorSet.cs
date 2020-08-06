// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectorSet.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities.Ais
{
    using System;
    using System.Collections.Generic;

    using OneSim.Traffic.Domain.Entities;

    /// <summary>
    ///     The class representing a set of data from a sector file.
    /// </summary>
    public class SectorSet
    {
        /// <summary>
        ///     Gets or sets the ID of the current <see cref="SectorSet"/>.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the name of the current <see cref="SectorSet"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DateTimeOffset"/> at which the current <see cref="SectorSet"/> becomes
        ///     effective.
        /// </summary>
        public DateTimeOffset EffectiveDate { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="NetworkType"/> which the current <see cref="SectorSet"/> is valid for.
        /// </summary>
        public NetworkType Network { get; set; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="FlightInformationRegion"/>s which the current
        ///     <see cref="SectorSet"/> covers.
        /// </summary>
        public List<FlightInformationRegion> CoveredFlightInformationRegions { get; }

        /// <summary>
        ///     Gets or sets the ID of the user who submitted the current <see cref="SectorSet"/>.
        /// </summary>
        public string SubmittedByUserId { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SectorSet"/> class.
        /// </summary>
        public SectorSet() => CoveredFlightInformationRegions = new List<FlightInformationRegion>();
    }
}

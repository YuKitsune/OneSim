// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlightInformationRegion.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities.Aeronautical
{
    /// <summary>
    ///     The Flight Information Region (FIR).
    /// </summary>
    public class FlightInformationRegion
    {
        /// <summary>
        ///     Gets or sets the ID of the current <see cref="FlightInformationRegion"/>.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the current <see cref="FlightInformationRegion"/>s ICAO code.
        /// </summary>
        public string IacoCode { get; set; }
    }
}

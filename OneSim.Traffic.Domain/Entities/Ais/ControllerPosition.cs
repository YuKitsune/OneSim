// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControllerPosition.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Domain.Entities.Ais
{
    /// <summary>
    ///     The Air Traffic Controller position.
    /// </summary>
    public class ControllerPosition : SectorSetSpecificEntity
    {
        /// <summary>
        ///     Gets or sets the ID of the current <see cref="ControllerPosition"/>.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the callsign.
        /// </summary>
        public string RadioCallsign { get; set; }

        /// <summary>
        ///     Gets or sets the frequency of the current <see cref="ControllerPosition"/> multiplied by 1000.
        ///     E.g. 120.500 = 120500.
        /// </summary>
        public int Frequency { get; set; }

        // Todo: Check if this is needed.

        /// <summary>
        ///     Gets or sets the ID of the current <see cref="ControllerPosition"/> within the context of the original
        ///     sector file it was imported from.
        /// </summary>
        public string SectorId { get; set; }

        /// <summary>
        ///     Gets or sets the prefix of the network logon callsign.
        ///     E.g. "LON" in "LON_W_CTR".
        /// </summary>
        public string CallsignPrefix { get; set; }

        /// <summary>
        ///     Gets or sets the middle section of the network logon callsign.
        ///     E.g. "W" in "LON_W_CTR".
        /// </summary>
        public string CallsignMiddle { get; set; }

        /// <summary>
        ///     Gets or sets the suffix of the network logon callsign.
        ///     E.g. "CTR" in "LON_W_CTR".
        /// </summary>
        public string CallsignSuffix { get; set; }
    }
}

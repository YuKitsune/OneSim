// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSection.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Application.SectorFileParsers.EuroScopeExtensionFile
{
    /// <summary>
    ///     The EuroScope Extension File (.ese) sections.
    /// </summary>
    public enum FileSection
    {
        /// <summary>
        ///     No section.
        /// </summary>
        None,

        /// <summary>
        ///     The controller positions section.
        /// </summary>
        Positions,

        /// <summary>
        ///     The SIDs and STARs section.
        /// </summary>
        SidsStars,

        /// <summary>
        ///     The free text section.
        /// </summary>
        FreeText,

        /// <summary>
        ///     The airspace definition section.
        /// </summary>
        Airspace,

        /// <summary>
        ///     The radar definition section.
        /// </summary>
        Radar,

        /// <summary>
        ///     The taxiway route section.
        /// </summary>
        Ground,

        /// <summary>
        ///     A section which is not supported by the parser.
        /// </summary>
        Unsupported,
    }
}

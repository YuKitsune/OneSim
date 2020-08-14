// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSection.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Application.SectorFileParsers.SectorFile
{
    /// <summary>
    ///     The Sector File (.sct or .sct2) sections.
    /// </summary>
    public enum FileSection
    {
        /// <summary>
        ///     No section.
        /// </summary>
        None,

        /// <summary>
        ///     The [INFO] section.
        /// </summary>
        Info,

        /// <summary>
        ///     The [VOR] section.
        /// </summary>
        Vor,

        /// <summary>
        ///     The [NDB] section.
        /// </summary>
        Ndb,

        /// <summary>
        ///     The [AIRPORT] section.
        /// </summary>
        Airport,

        /// <summary>
        ///     The [RUNWAY] section.
        /// </summary>
        Runway,

        /// <summary>
        ///     The [FIXES] section.
        /// </summary>
        Fixes,

        /// <summary>
        ///     The [ARTCC] section.
        /// </summary>
        Artcc,

        /// <summary>
        ///     The [ARTCCHIGH] section.
        /// </summary>
        ArtccHigh,

        /// <summary>
        ///     The [ARTCCLOW] section.
        /// </summary>
        ArtccLow,

        /// <summary>
        ///      The [SID] section.
        /// </summary>
        Sid,

        /// <summary>
        ///     The [STAR] section.
        /// </summary>
        Star,

        /// <summary>
        ///     The [GEO] section.
        /// </summary>
        Geo,

        /// <summary>
        ///     The [LOWAIRWAY] section.
        /// </summary>
        LowAirway,

        /// <summary>
        ///     The [HIGHAIRWAY] section.
        /// </summary>
        HighAirway,

        /// <summary>
        ///     The [REGIONS] section.
        /// </summary>
        Regions,

        /// <summary>
        ///     The [LABELS] section.
        /// </summary>
        Labels,

        /// <summary>
        ///     A section which is not supported by the parser.
        /// </summary>
        Unsupported
    }
}

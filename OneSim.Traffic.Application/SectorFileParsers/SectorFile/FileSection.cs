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
        None,
        Info,
        VOR,
        NDB,
        Airport,
        Runway,
        Fixes,
        ARTCC,
        ArtccHigh,
        ArtccLow,
        SID,
        STAR,
        Geo,
        LowAirway,
        HighAirway,
        Regions,
        Labels,
        Unsupported
    }
}

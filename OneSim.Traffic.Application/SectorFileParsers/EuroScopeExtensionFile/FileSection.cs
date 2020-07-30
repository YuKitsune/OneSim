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
        None,
        Positions,
        SidsStars,
        FreeText,
        Airspace,
        Radar,
        Ground,
        Unsupported,
    }
}

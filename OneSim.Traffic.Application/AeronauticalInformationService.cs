// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AeronauticalInformationService.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Application
{
    using System.Threading.Tasks;

    using OneSim.Traffic.Application.Abstractions;
    using OneSim.Traffic.Application.SectorFileParsers.EuroScopeExtensionFile;
    using OneSim.Traffic.Application.SectorFileParsers.PositionFile;
    using OneSim.Traffic.Application.SectorFileParsers.SectorFile;

    /// <summary>
    ///     The service responsible for managing Aeronautical data.
    /// </summary>
    public class AeronauticalInformationService
    {
        /// <summary>
        ///     The <see cref="IAeronauticalDbContext"/>.
        /// </summary>
        private readonly IAeronauticalDbContext _aeronauticalDbContext;

        /// <summary>
        ///     Adds a new set of AIS data to the <see cref="IAeronauticalDbContext"/> as an asynchronous operation.
        /// </summary>
        /// <param name="sectorFileParseResult">
        ///     The <see cref="SectorFileParseResult"/>.
        /// </param>
        /// <param name="positionFileParseResult">
        ///     The <see cref="PositionFileParseResult"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        public async Task AddSectorFile(
            SectorFileParseResult sectorFileParseResult,
            PositionFileParseResult positionFileParseResult)
        {
        }

        /// <summary>
        ///     Adds a new set of AIS data to the <see cref="IAeronauticalDbContext"/> as an asynchronous operation.
        /// </summary>
        /// <param name="sectorFileParseResult">
        ///     The <see cref="SectorFileParseResult"/>.
        /// </param>
        /// <param name="euroScopeExtensionFileParseResult">
        ///     The <see cref="EuroScopeExtensionFileParseResult"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        public async Task AddSectorFile(
            SectorFileParseResult sectorFileParseResult,
            EuroScopeExtensionFileParseResult euroScopeExtensionFileParseResult)
        {
        }
    }
}

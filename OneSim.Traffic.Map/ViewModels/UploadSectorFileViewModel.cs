// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UploadSectorFileViewModel.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Map.ViewModels
{
    using System;
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Http;

    using OneSim.Traffic.Domain.Entities;
    using OneSim.Traffic.Domain.Entities.Aeronautical;

    /// <summary>
    ///     The View Model for uploading a sector file.
    /// </summary>
    public class UploadSectorFileViewModel
    {
        /// <summary>
        ///     Gets or sets the <see cref="NetworkType"/> which the given sector files are valid for.
        /// </summary>
        public NetworkType Network { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="DateTimeOffset"/> at which the given sector files become effective.
        /// </summary>
        public DateTimeOffset EffectiveDate { get; set; }

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of <see cref="FlightInformationRegion.Identifier"/>s which the given
        ///     sector files cover.
        /// </summary>
        public List<string> CoveredFlightInformationRegions { get; }

        /// <summary>
        ///     Gets or sets the <see cref="IFormFile"/> representing the Sector file (.sct).
        /// </summary>
        public IFormFile SectorFile { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="IFormFile"/> representing the EuroScope Extension file (.ese).
        /// </summary>
        public IFormFile EuroScopeExtensionFile { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="IFormFile"/> representing the Position file (.pof).
        /// </summary>
        public IFormFile PositionFile { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UploadSectorFileViewModel"/> class.
        /// </summary>
        public UploadSectorFileViewModel() => CoveredFlightInformationRegions = new List<string>();
    }
}

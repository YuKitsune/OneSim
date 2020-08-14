// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReviewEuroScopeExtensionFileViewModel.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Api.ViewModels
{
    using OneSim.Traffic.Application.SectorFileParsers.EuroScopeExtensionFile;
    using OneSim.Traffic.Application.SectorFileParsers.SectorFile;

    /// <summary>
    ///     The View Model for reviewing a EuroScope Extension File submission.
    /// </summary>
    public class ReviewEuroScopeExtensionFileViewModel
    {
        /// <summary>
        ///     Gets or sets the <see cref="SectorFileParseResult"/>.
        /// </summary>
        public SectorFileParseResult SectorFileParseResult { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="EuroScopeExtensionFileParseResult"/>.
        /// </summary>
        public EuroScopeExtensionFileParseResult EuroScopeExtensionFileParseResult { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the user has accepted the terms and conditions.
        /// </summary>
        public bool TermsAndConditionsAccepted { get; set; }
    }
}

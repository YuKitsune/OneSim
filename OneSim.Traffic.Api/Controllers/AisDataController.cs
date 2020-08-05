// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AisDataController.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Api.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using OneSim.Traffic.Application.SectorFileParsers.EuroScopeExtensionFile;
    using OneSim.Traffic.Application.SectorFileParsers.PositionFile;
    using OneSim.Traffic.Application.SectorFileParsers.SectorFile;

    /// <summary>
    ///     The Aeronautical Information Data <see cref="Controller"/>.
    /// </summary>
    [Authorize]
    public class AisDataController : Controller
    {
        /// <summary>
        ///     Gets the view for submitting AIS data.
        /// </summary>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet]
        public IActionResult SubmitAisData()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Submits the Sector File (.sct) and Position File (.pof) for parsing.
        /// </summary>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpPost]
        public IActionResult SubmitSectorFile()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Submits the Sector File (.sct2) and EuroScope Extension File (.ese) for parsing.
        /// </summary>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpPost]
        public IActionResult SubmitEuroScopeExtensionFile()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Displays the <see cref="SectorFileParseResult"/> and <see cref="PositionFileParseResult"/> for manual
        ///     review.
        /// </summary>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet]
        public IActionResult ReviewSectorFileResult()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Displays the <see cref="SectorFileParseResult"/> and <see cref="EuroScopeExtensionFileParseResult"/> for
        ///     manual review.
        /// </summary>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet]
        public IActionResult ReviewEuroScopeExtensionFileResult()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Commits the <see cref="SectorFileParseResult"/> and <see cref="PositionFileParseResult"/> to the
        ///     database as an asynchronous operation.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task{T}"/> representing the asynchronous operation.
        ///     The <see cref="Task{T}.Result"/> contains the <see cref="IActionResult"/>.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> CommitSectorFile()
        {
            await Task.Yield();
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Commits the <see cref="SectorFileParseResult"/> and <see cref="EuroScopeExtensionFileParseResult"/> to
        ///     the database as an asynchronous operation.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task{T}"/> representing the asynchronous operation.
        ///     The <see cref="Task{T}.Result"/> contains the <see cref="IActionResult"/>.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> CommitEuroScopeExtensionFile()
        {
            await Task.Yield();
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Gets the view to display once AIS data has been submitted.
        /// </summary>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet]
        public IActionResult AisDataSubmitted()
        {
            throw new NotImplementedException();
        }
    }
}

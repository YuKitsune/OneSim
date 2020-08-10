// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AisDataController.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Api.Controllers
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using OneSim.Traffic.Api.ViewModels;
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
        ///     Submits the Sector File (.sct) and Position File (.pof) for parsing as an asynchronous operation.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task{T}"/> representing the asynchronous operation.
        ///     The <see cref="Task{T}.Result"/> contains the <see cref="IActionResult"/>.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> SubmitSectorFile(UploadSectorFileViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Make sure we have the correct combination of files
                if (viewModel.SectorFile != null)
                {
                    string sectorFileContent = string.Empty;
                    await using (Stream stream = viewModel.SectorFile.OpenReadStream())
                    using (StreamReader streamReader = new StreamReader(stream))
                    {
                        sectorFileContent = await streamReader.ReadToEndAsync();
                    }

                    if (!string.IsNullOrEmpty(sectorFileContent))
                    {
                        SectorFileParser sectorFileParser = new SectorFileParser();
                        SectorFileParseResult sectorFileParseResult = sectorFileParser.Parse(sectorFileContent);

                        if (viewModel.EuroScopeExtensionFile != null &&
                            viewModel.PositionFile != null)
                        {
                            ModelState.AddModelError(string.Empty, "Cannot submit a EuroScope Extension file (.ese) and a Position file (.pof) at the same time.");
                        }
                        else if (viewModel.EuroScopeExtensionFile != null)
                        {
                            string euroScopeExtensionFileContent = string.Empty;
                            await using Stream stream = viewModel.EuroScopeExtensionFile.OpenReadStream();
                            using StreamReader streamReader = new StreamReader(stream);
                            euroScopeExtensionFileContent = await streamReader.ReadToEndAsync();

                            if (!string.IsNullOrEmpty(euroScopeExtensionFileContent))
                            {
                                EuroScopeExtensionFileParser euroScopeExtensionFileParser = new EuroScopeExtensionFileParser();
                                EuroScopeExtensionFileParseResult euroScopeExtensionFileParseResult = new EuroScopeExtensionFileParseResult();
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "The EuroScope Extension file (.ese) was empty.");
                            }
                        }
                        else if (viewModel.PositionFile != null)
                        {
                            string positionFileContent = string.Empty;
                            await using Stream stream = viewModel.PositionFile.OpenReadStream();
                            using StreamReader streamReader = new StreamReader(stream);
                            positionFileContent = await streamReader.ReadToEndAsync();

                            if (!string.IsNullOrEmpty(positionFileContent))
                            {
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "The Position file (.pos) was empty.");
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "The Sector file (.sct) was empty.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "No Sector File (.sct) was submitted.");
                }
            }

            // Made it this far, something ain't right
            return View("SubmitAisData", viewModel);
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

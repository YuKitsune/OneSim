// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AeronauticalDataController.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Map.Controllers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using OneSim.Traffic.Application;
    using OneSim.Traffic.Application.SectorFileParsers.EuroScopeExtensionFile;
    using OneSim.Traffic.Application.SectorFileParsers.PositionFile;
    using OneSim.Traffic.Application.SectorFileParsers.SectorFile;
    using OneSim.Traffic.Map.ViewModels;

    /// <summary>
    ///     The Aeronautical Information Data <see cref="Controller"/>.
    /// </summary>
    [Authorize]
    public class AeronauticalDataController : Controller
    {
        /// <summary>
        ///     The <see cref="AeronauticalInformationService"/>.
        /// </summary>
        private readonly AeronauticalInformationService _aeronauticalInformationService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AeronauticalDataController"/> class.
        /// </summary>
        /// <param name="aeronauticalInformationService">
        ///     The <see cref="AeronauticalInformationService"/>.
        /// </param>
        public AeronauticalDataController(AeronauticalInformationService aeronauticalInformationService) =>
            _aeronauticalInformationService = aeronauticalInformationService ??
                                              throw new ArgumentNullException(nameof(aeronauticalInformationService));

        /// <summary>
        ///     Gets the view for submitting aeronautical data.
        /// </summary>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet]
        public IActionResult SubmitData() => View();

        /// <summary>
        ///     Submits the Sector File (.sct) and Position File (.pof) for parsing as an asynchronous operation.
        /// </summary>
        /// <param name="viewModel">
        ///     The <see cref="UploadSectorFileViewModel"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="Task{T}"/> representing the asynchronous operation.
        ///     The <see cref="Task{T}.Result"/> contains the <see cref="IActionResult"/>.
        /// </returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitSectorFile([FromForm] UploadSectorFileViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Make sure we have the correct combination of files
                    if (viewModel.SectorFile != null)
                    {
                        string sectorFileContent;
                        await using (Stream stream = viewModel.SectorFile.OpenReadStream())
                        using (StreamReader streamReader = new StreamReader(stream))
                        {
                            sectorFileContent = await streamReader.ReadToEndAsync();
                        }

                        if (!string.IsNullOrEmpty(sectorFileContent))
                        {
                            SectorFileParser sectorFileParser = new SectorFileParser();
                            SectorFileParseResult sectorFileParseResult = sectorFileParser.Parse(sectorFileContent);
                            sectorFileParseResult.SectorSet.Network = viewModel.Network;
                            sectorFileParseResult.SectorSet.EffectiveDate = viewModel.EffectiveDate;

                            string userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                            sectorFileParseResult.SectorSet.SubmittedByUserId = userId;

                            if (viewModel.EuroScopeExtensionFile != null &&
                                viewModel.PositionFile != null)
                            {
                                ModelState.AddModelError(
                                    string.Empty,
                                    "Cannot submit a EuroScope Extension file (.ese) and a Position file (.pof) at the same time.");
                            }
                            else if (viewModel.EuroScopeExtensionFile != null)
                            {
                                string euroScopeExtensionFileContent = string.Empty;
                                await using Stream stream = viewModel.EuroScopeExtensionFile.OpenReadStream();
                                using StreamReader streamReader = new StreamReader(stream);
                                euroScopeExtensionFileContent = await streamReader.ReadToEndAsync();

                                if (!string.IsNullOrEmpty(euroScopeExtensionFileContent))
                                {
                                    EuroScopeExtensionFileParser euroScopeExtensionFileParser =
                                        new EuroScopeExtensionFileParser();
                                    EuroScopeExtensionFileParseResult euroScopeExtensionFileParseResult =
                                        euroScopeExtensionFileParser.Parse(
                                            euroScopeExtensionFileContent,
                                            sectorFileParseResult);
                                    try
                                    {
                                        await _aeronauticalInformationService.AddSectorFile(
                                            sectorFileParseResult,
                                            euroScopeExtensionFileParseResult);
                                        return RedirectToAction(nameof(DataSubmitted));
                                    }
                                    catch (Exception ex)
                                    {
                                        // Todo: log
                                        ModelState.AddModelError(
                                            string.Empty,
                                            "An error has occurred while submitting the sector file.");
                                    }
                                }
                                else
                                {
                                    ModelState.AddModelError(
                                        string.Empty,
                                        "The EuroScope Extension file (.ese) was empty.");
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
                                    PositionFileParser positionFileParser = new PositionFileParser();
                                    PositionFileParseResult positionFileParseResult =
                                        positionFileParser.Parse(positionFileContent);
                                    try
                                    {
                                        await _aeronauticalInformationService.AddSectorFile(
                                            sectorFileParseResult,
                                            positionFileParseResult);
                                        return RedirectToAction(nameof(DataSubmitted));
                                    }
                                    catch (Exception ex)
                                    {
                                        // Todo: log
                                        ModelState.AddModelError(
                                            string.Empty,
                                            "An error has occurred while submitting the sector file.");
                                    }
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
                catch (Exception ex)
                {
                    // Todo: log
                    ModelState.AddModelError(
                        string.Empty,
                        "An error has occurred while submitting the sector file.");
                }
            }

            // Made it this far, something ain't right
            viewModel.SectorFile = null;
            viewModel.PositionFile = null;
            viewModel.EuroScopeExtensionFile = null;
            return View(nameof(SubmitData), viewModel);
        }

        /// <summary>
        ///     Gets the view to display once AIS data has been submitted.
        /// </summary>
        /// <returns>
        ///     The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet]
        public IActionResult DataSubmitted() => View();
    }
}

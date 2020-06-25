// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IndexViewModel.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Web.ViewModels.Account
{
    /// <summary>
    ///     The View Model to use on the account management index page.
    /// </summary>
    public class IndexViewModel
    {
        /// <summary>
        ///     Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the <see cref="Message"/> is an errors.
        /// </summary>
        public bool MessageIsError { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="IndexViewModel"/> class.
        /// </summary>
        public IndexViewModel() => MessageIsError = false;
    }
}

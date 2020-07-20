// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogInViewModel.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Windows.ViewModels
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using IdentityModel.OidcClient;

    using Strato.EventAggregator.Abstractions;
    using Strato.Mvvm.Commands;
    using Strato.Mvvm.Navigation;
    using Strato.Mvvm.ViewModels;

    /// <summary>
    ///     The <see cref="ViewModel"/> for the Log In View.
    /// </summary>
    public class LogInViewModel : ViewModel
    {
        /// <summary>
        ///     The <see cref="OidcClient"/> used to communicate with the OneSim Identity server.
        /// </summary>
        private readonly OidcClient _oidcClient;

        /// <summary>
        ///     Gets the <see cref="AsyncCommand"/> used to log the user in.
        /// </summary>
        public AsyncCommand LogInCommand => Get(new AsyncCommand(LogInAsync));

        /// <summary>
        ///     Gets the <see cref="AsyncCommand"/> used to configure offline mode.
        /// </summary>
        public AsyncCommand UseOfflineModeCommand => Get(new AsyncCommand(UseOfflineMode));

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogInViewModel"/> class.
        /// </summary>
        /// <param name="eventAggregator">
        ///     The <see cref="IEventAggregator"/> to use for publishing and subscribing to events.
        /// </param>
        /// <param name="navigationContext">
        ///     The <see cref="INavigationContext"/> to use for navigation.
        /// </param>
        /// <param name="oidcClient">
        ///     The <see cref="OidcClient"/> used to communicate with the OneSim Identity server.
        /// </param>
        public LogInViewModel(IEventAggregator eventAggregator, INavigationContext navigationContext, OidcClient oidcClient)
            : base(eventAggregator, navigationContext)
        {
            _oidcClient = oidcClient ?? throw new ArgumentNullException(nameof(oidcClient));
        }

        /// <summary>
        ///     Attempts to log the user in as an asynchronous operation.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        public async Task LogInAsync()
        {
            LoginResult result = await _oidcClient.LoginAsync();
            Debugger.Break();
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Attempts to configure OneSim for use in offline mode as an asynchronous operation.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        public async Task UseOfflineMode()
        {
            await Task.Yield();
            throw new NotImplementedException();
        }
    }
}

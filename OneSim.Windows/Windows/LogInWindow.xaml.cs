﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogInWindow.xaml.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Windows.Windows
{
    using System.Windows;

    using OneSim.Windows.Events;
    using OneSim.Windows.ViewModels;
    using OneSim.Windows.Views.LogInViews;
    using Strato.EventAggregator.Abstractions;
    using Strato.Mvvm.Navigation;
    using Strato.Mvvm.Wpf.Windows;

    /// <summary>
    ///     Interaction logic for LogInWindow.xaml.
    ///     The Log In Window is where the authentication process occurs (Login, 2FA, password reset, etc.)
    /// </summary>
    public partial class LogInWindow : ManagedWindow
    {
        /// <summary>
        ///     Gets the <see cref="IEventAggregator"/>.
        /// </summary>
        public IEventAggregator EventAggregator { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogInWindow"/> class.
        /// </summary>
        /// <param name="windowManager">
        ///     The <see cref="WindowManager"/>.
        /// </param>
        /// <param name="navigationContext">
        ///     The <see cref="INavigationContext"/>.
        /// </param>
        /// <param name="eventAggregator">
        ///     The <see cref="IEventAggregator"/>.
        /// </param>
        public LogInWindow(
            WindowManager windowManager,
            INavigationContext navigationContext,
            IEventAggregator eventAggregator)
            : base(windowManager, navigationContext)
        {
            InitializeComponent();

            // Setup the navigation context
            NavigationContext.Register<LogInView, LogInViewModel>();
            NavigationContext.Register<ResetPasswordView, ResetPasswordViewModel>();
            NavigationControl.UseNavigationContext(NavigationContext);

            // Setup the Event Aggregator
            EventAggregator = eventAggregator;
            EventAggregator.Subscribe<CloseEvent>(HandleCloseEvent);
        }

        /// <summary>
        ///     Method called when the current <see cref="Window"/> has loaded.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The <see cref="RoutedEventArgs"/>.
        /// </param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            NavigationContext.NavigateTo<LogInViewModel>();
        }

        /// <summary>
        ///     Handles the <see cref="CloseEvent"/>.
        /// </summary>
        /// <param name="closeEvent">
        ///     The <see cref="CloseEvent"/> to handle.
        /// </param>
        private void HandleCloseEvent(CloseEvent closeEvent)
        {
            if (closeEvent.ViewModelType == null ||
                closeEvent.ViewModelType == NavigationContext.CurrentViewModel.GetType())
            {
                Close();
            }
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Windows.Windows
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    using ModernWpf.Controls;

    using OneSim.Windows.Controls;
    using OneSim.Windows.Extensions;
    using OneSim.Windows.ViewModels;
    using OneSim.Windows.Views;
    using Strato.EventAggregator.Abstractions;
    using Strato.Mvvm.Navigation;
    using Strato.Mvvm.Navigation.Events;

    using ListView = ModernWpf.Controls.ListView;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml.
    ///     The Main Window is where the majority of interaction happens within OneSim.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        ///     Gets the <see cref="IEventAggregator"/>.
        /// </summary>
        public IEventAggregator EventAggregator { get; }

        /// <summary>
        ///     Gets the <see cref="INavigationContext"/>.
        /// </summary>
        public INavigationContext NavigationContext { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        /// <param name="navigationContext">
        ///     The <see cref="INavigationContext"/>.
        /// </param>
        /// <param name="eventAggregator">
        ///     The <see cref="IEventAggregator"/>.
        /// </param>
        public MainWindow(
            INavigationContext navigationContext,
            IEventAggregator eventAggregator)
        {
            InitializeComponent();

            // Setup the navigation context
            NavigationContext = navigationContext ?? throw new ArgumentNullException(nameof(navigationContext));
            NavigationContext.Register<FlyNowView, FlyNowViewModel>();
            NavigationContext.Register<HangarView, HangarViewModel>();
            NavigationControl.UseNavigationContext(NavigationContext);

            // Setup the Event Aggregator
            EventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            EventAggregator.Subscribe<CloseEvent>(HandleCloseEvent);

            // Show the back button when we're allowed to go back
            SetBinding(TitleBar.IsBackButtonVisibleProperty, new Binding { Path = new PropertyPath(Frame.CanGoBackProperty), Source = NavigationControl });
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
            NavigationContext.NavigateTo<FlyNowViewModel>();
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

        /// <summary>
        ///     Handles the request to go back.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The <see cref="BackRequestedEventArgs"/>.
        /// </param>
        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (NavigationControl.CanGoBack) NavigationControl.GoBack();
        }

        /// <summary>
        ///     Handles when the Sidebars selection has changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The <see cref="SelectionChangedEventArgs"/>.
        /// </param>
        private void SidebarSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NavigationContext == null) return;
            if (sender == null) return;
            if (!(sender is ListView listView)) return;
            if (listView.SelectedItem == null ||
                !(listView.SelectedItem is SidebarItem sidebarItem)) return;

            // Navigate to the ViewModel instance
            NavigationContext.NavigateTo(sidebarItem.ViewModel.GetType(), sidebarItem.ViewModel);
        }
    }
}

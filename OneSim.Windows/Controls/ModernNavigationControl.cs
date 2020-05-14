// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModernNavigationControl.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Windows.Controls
{
    using System;

    using ModernWpf.Controls;

    using Strato.Mvvm;
    using Strato.Mvvm.Navigation;
    using Strato.Mvvm.Wpf.Controls;

    /// <summary>
    ///     The ModernWPF implementation of the <see cref="INavigationControl"/> interface.
    /// </summary>
    public class ModernNavigationControl : TransitionFrame, INavigationControl
    {
        /// <summary>
        ///     Gets the <see cref="INavigationContext"/> to use for managing navigation.
        /// </summary>
        public INavigationContext NavigationContext { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ModernNavigationControl"/> class.
        /// </summary>
        public ModernNavigationControl()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ModernNavigationControl"/> class.
        /// </summary>
        /// <param name="navigationContext">
        ///     The <see cref="INavigationContext"/> to use for managing navigation.
        /// </param>
        public ModernNavigationControl(INavigationContext navigationContext)
        {
            if (navigationContext == null) throw new ArgumentNullException(nameof(navigationContext));
            UseNavigationContext(navigationContext);
        }

        /// <summary>
        ///     Sets the <see cref="NavigationContext"/> to the given <paramref name="navigationContext"/>.
        /// </summary>
        /// <param name="navigationContext">
        ///     The <see cref="INavigationContext"/> to use.
        /// </param>
        public void UseNavigationContext(INavigationContext navigationContext)
        {
            NavigationContext = navigationContext;
            NavigationContext.OnNavigationRequestedAction = OnNavigationRequested;
        }

        /// <summary>
        ///     The method raised when the <see cref="NavigationContext"/> has requested for navigation to be
        ///     conducted.
        /// </summary>
        /// <param name="view">
        ///     The <see cref="IView"/> to navigate to.
        /// </param>
        public void OnNavigationRequested(IView view)
        {
            Navigate(view);
        }
    }
}

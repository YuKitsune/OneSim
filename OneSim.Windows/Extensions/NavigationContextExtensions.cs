// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationContextExtensions.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Windows.Extensions
{
    using System;
    using System.Reflection;

    using Strato.Mvvm.Navigation;
    using Strato.Mvvm.ViewModels;

    /// <summary>
    ///     The <see cref="INavigationContext"/> extension methods.
    /// </summary>
    public static class NavigationContextExtensions
    {
        // Todo: Cleanup exception messages
        // Todo: Migrate to Strato.Mvvm

        /// <summary>
        ///     Navigates to the requested <see cref="ViewModel"/> instance.
        /// </summary>
        /// <param name="context">
        ///     The <see cref="INavigationContext"/>.
        /// </param>
        /// <param name="viewModelType">
        ///     The <see cref="Type"/> of <see cref="ViewModel"/> being provided.
        /// </param>
        /// <param name="viewModel">
        ///     The <see cref="ViewModel"/> instance to navigate to.
        /// </param>
        public static void NavigateTo(this INavigationContext context, Type viewModelType, ViewModel viewModel)
        {
            if (viewModelType == null) throw new ArgumentNullException(nameof(viewModelType));
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            if (!typeof(ViewModel).IsAssignableFrom(viewModelType))
            {
                throw new ArgumentException($"The type {viewModelType} does not implement {nameof(ViewModel)}.");
            }

            // Get the NavigateTo method
            Type contextType = typeof(INavigationContext);
            MethodInfo navigateMethod = contextType.GetMethod(nameof(INavigationContext.NavigateTo));
            if (navigateMethod == null)
            {
                throw new Exception($"Unable to find a {nameof(INavigationContext.NavigateTo)} method.");
            }

            // Add the type parameter
            MethodInfo genericNavigateMethod = navigateMethod.MakeGenericMethod(viewModelType);

            // Invoke the method
            genericNavigateMethod.Invoke(context, new object[] { viewModel });
        }
    }
}

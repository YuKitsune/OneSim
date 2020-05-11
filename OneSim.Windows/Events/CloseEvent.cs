// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CloseEvent.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Windows.Events
{
    using System;

    using Strato.EventAggregator.Abstractions;
    using Strato.Mvvm.ViewModels;

    /// <summary>
    ///     The <see cref="IEvent"/> to raise when closing requesting for a Window to close.
    /// </summary>
    public class CloseEvent : IEvent
    {
        /// <summary>
        ///     Gets the <see cref="Type"/> of the <see cref="ViewModel"/> requesting for the Window to be closed.
        ///     If this property has not been set, then all Windows handling this <see cref="IEvent"/> will close.
        /// </summary>
        public Type ViewModelType { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CloseEvent"/> class.
        /// </summary>
        public CloseEvent()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CloseEvent"/> class.
        /// </summary>
        /// <param name="viewModelType">
        ///     The <see cref="Type"/> of <see cref="ViewModel"/> requesting for the Window to be closed.
        /// </param>
        public CloseEvent(Type viewModelType)
        {
            if (viewModelType == null) throw new ArgumentNullException(nameof(viewModelType));
            if (!typeof(ViewModel).IsAssignableFrom(viewModelType))
            {
                throw new ArgumentException($"The type \"{viewModelType.Name}\" does not implement \"{typeof(ViewModel).Name}\".");
            }

            ViewModelType = viewModelType;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CloseEvent"/> class.
        /// </summary>
        /// <param name="viewModel">
        ///     The <see cref="ViewModel"/> requesting for the Window to be closed.
        /// </param>
        public CloseEvent(ViewModel viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));
            ViewModelType = viewModel.GetType();
        }

        /// <summary>
        ///     Creates a new <see cref="CloseEvent"/> instance.
        /// </summary>
        /// <typeparam name="TViewModel">
        ///     The type of <see cref="ViewModel"/> requesting for the Window to be closed.
        /// </typeparam>
        /// <returns>
        ///     The new <see cref="CloseEvent"/> instance.
        /// </returns>
        public static CloseEvent Create<TViewModel>()
            where TViewModel : ViewModel =>
            new CloseEvent(typeof(TViewModel));
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SidebarItem.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Windows.Controls
{
    using Strato.Mvvm.ViewModels;

    /// <summary>
    ///     The Sidebar Item.
    /// </summary>
    public class SidebarItem
    {
        /// <summary>
        ///     Gets the title of the sidebar item.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        ///     Gets the <see cref="ViewModel"/> instance associated with the current <see cref="SidebarItem"/>.
        /// </summary>
        public ViewModel ViewModel { get; private set; }

        /// <summary>
        ///     Gets a new instance of the <see cref="SidebarItem"/> class with the given <paramref name="title"/>
        ///     And <typeparamref name="TViewModel"/> instance.
        /// </summary>
        /// <typeparam name="TViewModel">
        ///     The type of <see cref="ViewModel"/> to use.
        /// </typeparam>
        /// <param name="title">
        ///     The title.
        /// </param>
        /// <param name="viewModel">
        ///     The <typeparamref name="TViewModel"/>.
        /// </param>
        /// <returns>
        ///     The new <see cref="SidebarItem"/> instance.
        /// </returns>
        public static SidebarItem FromViewModel<TViewModel>(string title, TViewModel viewModel)
            where TViewModel : ViewModel =>
            new SidebarItem { Title = title, ViewModel = viewModel };
    }
}

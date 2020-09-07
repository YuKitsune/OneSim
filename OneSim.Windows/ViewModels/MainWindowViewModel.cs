// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindowViewModel.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Windows.ViewModels
{
    using System.Collections.Generic;

    using OneSim.Windows.Controls;
    using Strato.Mvvm.ViewModels;

    /// <summary>
    ///     The main window <see cref="ViewModel"/>.
    /// </summary>
    public class MainWindowViewModel : ViewModel
    {
        /// <summary>
        ///     Gets or sets the <see cref="SidebarItem"/>s.
        /// </summary>
        public List<SidebarItem> SidebarItems
        {
            get => Get<List<SidebarItem>>();
            set => Set(value);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel() => SidebarItems = GetSidebarItems();

        /// <summary>
        ///     Gets the <see cref="List{T}"/> of available <see cref="SidebarItem"/>s.
        /// </summary>
        /// <returns>
        ///     The <see cref="List{T}"/> of <see cref="SidebarItem"/>s.
        /// </returns>
        public static List<SidebarItem> GetSidebarItems() =>
            new List<SidebarItem>
            {
                SidebarItem.FromViewModel("Fly", new FlyNowViewModel()),
                SidebarItem.FromViewModel("Hangar", new HangarViewModel()),
                SidebarItem.FromViewModel("Map", new FlyNowViewModel()),
                SidebarItem.FromViewModel("Settings", new FlyNowViewModel()),
            };
    }
}

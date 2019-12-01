using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using OneSim.Uwp.Models;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OneSim.Uwp.Pages
{
    /// <summary>
    ///     The default page displayed each time a new tab is created.
    ///     This page contains links to all other pages.
    /// </summary>
    public sealed partial class DefaultPage : BasePage
    {
        /// <summary>
        ///     Gets the header string.
        /// </summary>
        public override string Header => "Home";

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultPage"/> page.
        /// </summary>
        public DefaultPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        ///     Method called when an item has been clicked.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The <see cref="ItemClickEventArgs"/>.
        /// </param>
        private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is PageCardItem item) RequestNewPage(item.PageType);
        }
    }
}

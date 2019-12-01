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
using Microsoft.Toolkit.Uwp.UI.Controls;
using OneSim.Uwp.Pages;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace OneSim.Uwp.Controls
{
    /// <summary>
    ///     An implementation of <see cref="TabViewItem"/> where the content can be changed at runtime.
    /// </summary>
    public sealed partial class DynamicTabViewItem : TabViewItem
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicTabViewItem"/> class.
        /// </summary>
        public DynamicTabViewItem()
        {
            this.InitializeComponent();
        }

        /// <summary>
        ///     Loads the given page <see cref="Type"/> into the current <see cref="DynamicTabViewItem"/>.
        /// </summary>
        /// <param name="pageType">
        ///     The <see cref="Type"/> of <see cref="BasePage"/> to load.
        /// </param>
        public void SetPage(Type pageType)
        {
            // Ignore if no page type was provided
            if (pageType == null) return;

            // Navigate to the page
            RootFrame.Navigate(pageType);

            // Instruct the new page to use this method for requesting new pages
            if (RootFrame.Content is BasePage page)
            {
                Header = page.Header;
                page.NewPageRequested = SetPage;
            }
        }
    }
}

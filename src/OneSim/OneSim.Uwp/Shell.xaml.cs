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
using Microsoft.Toolkit.Uwp.UI.Extensions;
using OneSim.Uwp.Pages;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OneSim.Uwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Shell : Page
    {
        /// <summary>
        ///     The current instance of the <see cref="Shell"/>.
        /// </summary>
        public static Shell Current { get; private set; }

        /// <summary>
        ///     The <see cref="TabView"/>.
        /// </summary>
        private TabView _tabs;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Shell"/> page.
        /// </summary>
        public Shell()
        {
            InitializeComponent();
            Current = this;
        }

        /// <summary>
        ///     Adds a new tab.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The <see cref="RoutedEventArgs"/>.
        /// </param>
        private void AddTab(object sender, RoutedEventArgs e)
        {
            _tabs?.Items?.Add(new TabViewItem()
            {
                IsClosable = true,
                Header = "New tab",
                Content = new DefaultPage()
            });
        }

        /// <summary>
        ///     Method called when the shell has loaded.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The <see cref="RoutedEventArgs"/>.
        /// </param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement control)
            {
                // Get the TabView and assign events
                _tabs = control.FindChildByName(nameof(Tabs)) as TabView;
                if (_tabs != null)
                {
                    // Todo: Assign tab related events
                }

                // Get the Button and assign events
                var btn = control.FindDescendantByName(nameof(AddTabButton)) as Button;
                if (btn != null)
                {
                    btn.Click += AddTab;
                }
            }
        }
    }
}

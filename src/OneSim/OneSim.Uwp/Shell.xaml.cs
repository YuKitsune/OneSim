using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using OneSim.Uwp.Controls;
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
            if (_tabs == null || _tabs.Items == null) return;
            DynamicTabViewItem tab = new DynamicTabViewItem
            {
                Header = "Test"
            };
            tab.SetPage(typeof(DefaultPage));
            _tabs.Items.Add(tab);
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

        private async void OnTabDraggedOutside(object sender, TabDraggedOutsideEventArgs e)
        {
            if (e.Tab is DynamicTabViewItem tab)
            {
                Frame frame = (Frame) tab.Content;
                if (frame?.Content != null)
                {
                    // Create a new window
                    AppWindow appWindow = await AppWindow.TryCreateAsync();
                    
                    // Fill the window with the correct content
                    // Todo: Restore state
                    Frame appWindowContentFrame = new Frame();
                    appWindowContentFrame.Navigate(frame.Content.GetType());
                    ElementCompositionPreview.SetAppWindowContent(appWindow, appWindowContentFrame);

                    // Show the window
                    await appWindow.TryShowAsync();

                    // Todo: Keep track of detached tabs so we can dispose of resources later
                }
            }
        }
    }
}

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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OneSim.Uwp.Pages
{
    /// <summary>
    ///     The Map page.
    ///     Used for viewing online traffic.
    /// </summary>
    public sealed partial class MapPage : BasePage
    {
        /// <summary>
        ///     Gets the header string.
        /// </summary>
        public override string Header => "Map";

        /// <summary>
        ///     Initializes a new instance of the <see cref="MapPage"/> page.
        /// </summary>
        public MapPage()
        {
            this.InitializeComponent();
        }
    }
}

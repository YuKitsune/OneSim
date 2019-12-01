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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace OneSim.Uwp.Pages
{
    /// <summary>
    ///     The base <see cref="Page"/>.
    /// </summary>
    public abstract partial class BasePage : Page
    {
        /// <summary>
        ///     Gets the header string.
        /// </summary>
        public abstract string Header { get; }

        /// <summary>
        ///     Gets or sets the <see cref="Action{T}"/> to run when a new page has been requested.
        /// </summary>
        public Action<Type> NewPageRequested { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BasePage"/> class.
        /// </summary>
        public BasePage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        ///     Requests for a new page to be displayed.
        /// </summary>
        /// <param name="pageType">
        ///     The <see cref="Type"/> of <see cref="Page"/>.
        /// </param>
        protected void RequestNewPage(Type pageType)
        {
            NewPageRequested?.Invoke(pageType);
        }
    }
}

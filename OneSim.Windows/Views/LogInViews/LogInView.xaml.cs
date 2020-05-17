// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogInView.xaml.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Windows.Views.LogInViews
{
    using System;
    using System.Windows.Controls;

    using OneSim.Windows.ViewModels;

    using Strato.Mvvm;

    /// <summary>
    ///     Interaction logic for LogInPage.xaml.
    /// </summary>
    public partial class LogInView : UserControl, IView
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogInView"/> class.
        /// </summary>
        /// <param name="viewModel">
        ///     The <see cref="LogInViewModel"/> to use.
        /// </param>
        public LogInView(LogInViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }
    }
}

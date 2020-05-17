// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResetPasswordView.xaml.cs" company="Strato Systems Pty. Ltd.">
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
    ///     Interaction logic for ResetPasswordView.xaml.
    /// </summary>
    public partial class ResetPasswordView : UserControl, IView
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ResetPasswordView"/> class.
        /// </summary>
        /// <param name="viewModel">
        ///     The <see cref="ResetPasswordViewModel"/> to use.
        /// </param>
        public ResetPasswordView(ResetPasswordViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }
    }
}

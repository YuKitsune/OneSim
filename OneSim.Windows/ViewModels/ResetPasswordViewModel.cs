// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResetPasswordViewModel.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Windows.ViewModels
{
    using System.Threading.Tasks;

    using Strato.EventAggregator.Abstractions;
    using Strato.Mvvm.Commands;
    using Strato.Mvvm.Navigation;
    using Strato.Mvvm.ViewModels;

    /// <summary>
    ///     The <see cref="ViewModel"/> for the Reset Password view.
    /// </summary>
    public class ResetPasswordViewModel : ViewModel
    {
        /// <summary>
        ///     Gets or sets the email.
        /// </summary>
        public string Email
        {
            get => Get<string>();
            set => Set(value);
        }

        /// <summary>
        ///     Gets the <see cref="RelayCommand"/> to return the user to the Log In View.
        /// </summary>
        public RelayCommand CancelCommand => Get(new RelayCommand(Cancel));

        /// <summary>
        ///     Gets the <see cref="RelayCommand"/> to submit a Password Reset Request.
        /// </summary>
        public AsyncCommand ResetPasswordCommand => Get(new AsyncCommand(ResetPasswordAsync));

        /// <summary>
        ///     Initializes a new instance of the <see cref="ResetPasswordViewModel"/> class.
        /// </summary>
        /// <param name="eventAggregator">
        ///     The <see cref="IEventAggregator"/> to use for publishing and subscribing to events.
        /// </param>
        /// <param name="navigationContext">
        ///     The <see cref="INavigationContext"/> to use for navigation.
        /// </param>
        public ResetPasswordViewModel(IEventAggregator eventAggregator, INavigationContext navigationContext)
            : base(eventAggregator, navigationContext)
        {
        }

        /// <summary>
        ///     Returns the user to the Log In View.
        /// </summary>
        public void Cancel()
        {
            NavigationContext.NavigateTo<LogInViewModel>();
        }

        /// <summary>
        ///     Submits a Password Reset as an asynchronous operation.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        public async Task ResetPasswordAsync()
        {
            // Todo: Request a password reset from the server
        }
    }
}

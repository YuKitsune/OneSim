// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogInViewModel.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Windows.ViewModels
{
    using System.Security;
    using System.Threading.Tasks;
    using OneSim.Windows.Events;
    using Strato.EventAggregator.Abstractions;
    using Strato.Mvvm.Commands;
    using Strato.Mvvm.Navigation;
    using Strato.Mvvm.ViewModels;

    /// <summary>
    ///     The <see cref="ViewModel"/> for the Log In View.
    /// </summary>
    public class LogInViewModel : ViewModel
    {
        /// <summary>
        ///     Gets or sets the username.
        /// </summary>
        public string Username
        {
            get => Get<string>();
            set => Set(value);
        }

        /// <summary>
        ///     Gets or sets the password in the form of a <see cref="SecureString"/>.
        /// </summary>
        public string Password
        {
            get => Get<string>();
            set => Set(value);
        }

        /// <summary>
        ///     Gets or sets a value indicating whether or not you should remember the user.
        /// </summary>
        public bool RememberMe
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>
        ///     Gets a value indicating whether or not the user is allowed to log in.
        /// </summary>
        public bool CanLogIn => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);

        /// <summary>
        ///     Gets the <see cref="AsyncCommand"/> used to log the user in.
        /// </summary>
        public AsyncCommand LogInCommand => Get(new AsyncCommand(LogInAsync, () => CanLogIn));

        /// <summary>
        ///     Gets the <see cref="RelayCommand"/> used to display the "Reset Password" view.
        /// </summary>
        public RelayCommand DisplayResetPasswordViewCommand => Get(new RelayCommand(DisplayResetPasswordView));

        /// <summary>
        ///     Gets the <see cref="AsyncCommand"/> used to log the user in.
        /// </summary>
        public RelayCommand CancelCommand => Get(new RelayCommand(Cancel, () => !LogInCommand.IsExecuting));

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogInViewModel"/> class.
        /// </summary>
        /// <param name="eventAggregator">
        ///     The <see cref="IEventAggregator"/> to use for publishing and subscribing to events.
        /// </param>
        /// <param name="navigationContext">
        ///     The <see cref="INavigationContext"/> to use for navigation.
        /// </param>
        public LogInViewModel(IEventAggregator eventAggregator, INavigationContext navigationContext)
            : base(eventAggregator, navigationContext)
        {
        }

        /// <summary>
        ///     Attempts to log the user in as an asynchronous operation.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        public async Task LogInAsync()
        {
            await Task.Yield();

            // Todo: Attempt to log in

            // Todo: Switch to 2FA view if required
            bool twoFactorAuthenticationRequired = true;
            if (twoFactorAuthenticationRequired)
            {
                // NavigationContext.NavigateTo<TwoFactorAuthenticationViewModel>();
            }
        }

        /// <summary>
        ///     Displays the "Reset Password" view.
        /// </summary>
        public void DisplayResetPasswordView()
        {
            NavigationContext.NavigateTo<ResetPasswordViewModel>();
        }

        /// <summary>
        ///     Closes the view and any remaining windows.
        /// </summary>
        public void Cancel()
        {
            EventAggregator.Publish(new CloseEvent(this));
        }
    }
}

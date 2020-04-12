using System;
using System.Collections.Generic;
using System.Text;

namespace OneSim.Windows.ViewModels
{
    using System.Threading.Tasks;
    using Strato.Mvvm;

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

        public async Task ResetPasswordAsync()
        {
            await Task.Yield();

            //TODO Send request to server
        }
    }
}

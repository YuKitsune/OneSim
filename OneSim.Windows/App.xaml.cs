﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Windows
{
    using System;
    using System.IO;
    using System.Windows;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using IdentityModel.OidcClient;

    using OneSim.Windows.Utils;
    using OneSim.Windows.Windows;

    using Strato.EventAggregator;
    using Strato.EventAggregator.Abstractions;
    using Strato.Mvvm.Wpf.Extensions;
    using Strato.Mvvm.Wpf.Windows;

    /// <summary>
    ///     Interaction logic for App.xaml.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        ///     Gets the <see cref="IServiceProvider"/>.
        /// </summary>
        public IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        ///     Gets the <see cref="IConfiguration"/>.
        /// </summary>
        public IConfiguration Configuration { get; private set; }

        /// <summary>
        ///     Method raised when the application starts.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The <see cref="StartupEventArgs"/>.
        /// </param>
        private void OnStartup(object sender, StartupEventArgs e)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                                            .SetBasePath(Directory.GetCurrentDirectory())
                                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();

            // Configure the services
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();

            // Show the window
            // Todo: If the user has already logged in, then show the MainWindow, otherwise, show the login window
            WindowManager windowManager = ServiceProvider.GetRequiredService<WindowManager>();
            windowManager.OpenWindow<MainWindow>();
        }

        /// <summary>
        ///     Configures the services for the service container.
        /// </summary>
        /// <param name="services">
        ///     The <see cref="IServiceCollection"/>.
        /// </param>
        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IEventAggregator, EventAggregator>();
            services.AddWindowManagementAndNavigation();

            // Build the System Browser
            // Todo: Get these from some API given an app version number
            const string Host = "127.0.0.1";
            const int Port = 42069;
            const string Path = "";
            SystemBrowser browser = new SystemBrowser(Host, Port, Path);
            string redirectUrl = $"http://{browser.Host}:{browser.Port}/{browser.Path}";
            if (redirectUrl.EndsWith("/")) redirectUrl = redirectUrl.Substring(0, redirectUrl.Length - 1);

            // Build the OIDC client
            const string Authority = "https://localhost:5001/";
            OidcClient client = new OidcClient(new OidcClientOptions
            {
                Authority = Authority,
                ClientId = "desktop",
                Scope = "openid profile traffic",
                RedirectUri = redirectUrl,
                Browser = browser
            });
            services.AddSingleton(client);

            RegisterWindows(services);
        }

        /// <summary>
        ///     Registers all <see cref="Window"/>s which can be presented in the application.
        /// </summary>
        /// <param name="services">
        ///     The <see cref="IServiceCollection"/>.
        /// </param>
        private void RegisterWindows(IServiceCollection services)
        {
            services.AddTransient<MainWindow>();
            services.AddTransient<LogInWindow>();
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemBrowser.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Windows.Utils
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;

    using IdentityModel.OidcClient.Browser;

    // Todo: Improve documentation.

    /// <summary>
    ///     The <see cref="IBrowser"/> implementation which utilizes the local systems Web Browser.
    /// </summary>
    public class SystemBrowser : IBrowser
    {
        /// <summary>
        ///     Gets the hostname or IP address to use for the OIDC callback.
        /// </summary>
        public string Host { get; }

        /// <summary>
        ///     Gets the port to use for the OIDC callback.
        /// </summary>
        public int Port { get; }

        /// <summary>
        ///     Gets the path to use for the OIDC callback.
        /// </summary>
        public string Path { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SystemBrowser"/> class.
        /// </summary>
        /// <param name="host">
        ///     The hostname or IP address to use.
        /// </param>
        /// <param name="port">
        ///     The port to use.
        /// </param>
        /// <param name="path">
        ///     The URL path after the address and port (http://127.0.0.1:42069/{PATH}).
        /// </param>
        public SystemBrowser(string host, int port, string path = null)
        {
            if (string.IsNullOrEmpty(host)) throw new ArgumentNullException(nameof(host));
            if (port < 0 || port > 65535) throw new ArgumentException($"The {nameof(port)} ({port}) is not a valid port number.");
            Host = host;
            Port = port;
            Path = path;
        }

        /// <summary>
        ///     Invokes the browser as an asynchronous operation.
        /// </summary>
        /// <param name="options">
        ///     The <see cref="BrowserOptions"/>.
        /// </param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="Task{TResult}"/> representing the asynchronous operation.
        ///     The <see cref="Task{TResult}.Result"/> contains the <see cref="BrowserResult"/>.
        /// </returns>
        public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken)
        {
            // Create the listener
            using LoopbackHttpListener listener = new LoopbackHttpListener(Host, Port, Path);

            // Open the browser
            OpenBrowser(options.StartUrl);

            try
            {
                // Wait for the result
                string result = await listener.WaitForCallbackAsync();

                // Return the result
                return string.IsNullOrWhiteSpace(result)
                    ? new BrowserResult { ResultType = BrowserResultType.UnknownError, Error = "Empty response." }
                    : new BrowserResult { Response = result, ResultType = BrowserResultType.Success };
            }
            catch (TaskCanceledException ex)
            {
                return new BrowserResult { ResultType = BrowserResultType.Timeout, Error = ex.Message };
            }
            catch (Exception ex)
            {
                return new BrowserResult { ResultType = BrowserResultType.UnknownError, Error = ex.Message };
            }
        }

        /// <summary>
        ///     Opens the current systems web browser with the given <paramref name="url"/>.
        /// </summary>
        /// <param name="url">
        ///     The URL to open.
        /// </param>
        public static void OpenBrowser(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}

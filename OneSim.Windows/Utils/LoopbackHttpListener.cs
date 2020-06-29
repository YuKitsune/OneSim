// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoopbackHttpListener.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Windows.Utils
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    ///     The HTTP Loopback Listener used to receive commands via HTTP.
    /// </summary>
    internal class LoopbackHttpListener : IDisposable
    {
        /// <summary>
        ///     The <see cref="TaskCompletionSource{TResult}"/>.
        /// </summary>
        private readonly TaskCompletionSource<string> _completionSource = new TaskCompletionSource<string>();

        /// <summary>
        ///     The <see cref="IWebHost"/>.
        /// </summary>
        private readonly IWebHost _host;

        /// <summary>
        ///     Gets the default <see cref="TimeSpan"/> to wait before timing out.
        /// </summary>
        public TimeSpan DefaultTimeOut => TimeSpan.FromMinutes(5);

        /// <summary>
        ///     Gets the URL which the current <see cref="LoopbackHttpListener"/> is listening on.
        /// </summary>
        public string Url { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoopbackHttpListener"/> class.
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
        public LoopbackHttpListener(string host, int port, string path = null)
        {
            if (string.IsNullOrEmpty(host)) throw new ArgumentNullException(nameof(host));

            // Assign the path to an empty string if nothing was provided
            path ??= string.Empty;

            // Trim any excess slashes from the path
            if (path.StartsWith("/")) path = path.Substring(1);

            // Build the URL
            Url = $"http://{host}:{port}/{path}";

            // Build and start the web host
            _host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls(Url)
                .Configure(Configure)
                .Build();
            _host.Start();
        }

        /// <summary>
        ///     Waits until a callback has been received, then returns the result as an asynchronous operation.
        /// </summary>
        /// <param name="timeout">
        ///     The <see cref="TimeSpan"/> to wait before timing out.
        /// </param>
        /// <returns>
        ///     The <see cref="Task{T}"/> representing the asynchronous operation.
        ///     The <see cref="Task{TResult}.Result"/> contains the result.
        /// </returns>
        public Task<string> WaitForCallbackAsync(TimeSpan? timeout = null)
        {
            if (timeout == null)
            {
                timeout = DefaultTimeOut;
            }

            Task.Run(async () =>
            {
                await Task.Delay(timeout.Value);
                _completionSource.TrySetCanceled();
            });

            return _completionSource.Task;
        }

        /// <summary>
        ///     Configures the current <see cref="LoopbackHttpListener"/>.
        /// </summary>
        /// <param name="app">
        ///     The <see cref="IApplicationBuilder"/>.
        /// </param>
        private void Configure(IApplicationBuilder app)
        {
            app.Run(async ctx =>
            {
                switch (ctx.Request.Method)
                {
                    case "GET":
                        SetResult(ctx.Request.QueryString.Value, ctx);
                        break;

                    case "POST" when !ctx.Request.ContentType.Equals("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase):
                        ctx.Response.StatusCode = 415;
                        break;

                    case "POST":
                    {
                        using StreamReader sr = new StreamReader(ctx.Request.Body, Encoding.UTF8);
                        string body = await sr.ReadToEndAsync();
                        SetResult(body, ctx);

                        break;
                    }

                    default: ctx.Response.StatusCode = 405;
                        break;
                }
            });
        }

        /// <summary>
        ///     Disposes the current <see cref="LoopbackHttpListener"/> instance.
        /// </summary>
        public void Dispose()
        {
            Task.Run(async () =>
            {
                await Task.Delay(500);
                _host.Dispose();
            });
        }

        /// <summary>
        ///     Sets the result to be returned by the <see cref="WaitForCallbackAsync"/> method.
        /// </summary>
        /// <param name="value">
        ///     The value to set.
        /// </param>
        /// <param name="ctx">
        ///     The <see cref="HttpContext"/>.
        /// </param>
        private void SetResult(string value, HttpContext ctx)
        {
            // Todo: Custom HTML page? Maybe make a request to the main site for a page to render? Or redirect if possible?
            try
            {
                ctx.Response.StatusCode = 200;
                ctx.Response.ContentType = "text/html";
                ctx.Response.WriteAsync("<h1>You can now return to the application.</h1>");
                ctx.Response.Body.Flush();

                _completionSource.TrySetResult(value);
            }
            catch
            {
                ctx.Response.StatusCode = 400;
                ctx.Response.ContentType = "text/html";
                ctx.Response.WriteAsync("<h1>Invalid request.</h1>");
                ctx.Response.Body.Flush();
            }
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Web
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    ///     The program.
    /// </summary>
    public class Program
    {
        /// <summary>
        ///     The main application entry point.
        /// </summary>
        /// <param name="args">
        ///     The arguments.
        /// </param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        ///     Creates a new <see cref="IHostBuilder"/>.
        /// </summary>
        /// <param name="args">
        ///     The arguments.
        /// </param>
        /// <returns>
        ///     The <see cref="IHostBuilder"/> instance.
        /// </returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}

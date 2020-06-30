// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Map
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using OneSim.Common.Domain.Configuration;

    /// <summary>
    ///     The Startup class.
    /// </summary>
    public class Startup
    {
        /// <summary>
        ///     Gets the <see cref="IConfiguration"/>.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">
        ///     The <see cref="IConfiguration"/>.
        /// </param>
        public Startup(IConfiguration configuration) => Configuration = configuration;

        /// <summary>
        ///     This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">
        ///     The <see cref="IServiceCollection"/>.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            // Add Identity Server authentication
            IdentityServerConfig identityServerConfig = Configuration.GetSection("IdentityServer").Get<IdentityServerConfig>();
            services.AddAuthentication(options =>
                    {
                        options.DefaultScheme = "Cookies";
                        options.DefaultChallengeScheme = "oidc";
                    })
                    .AddCookie("Cookies")
                    .AddOpenIdConnect(
                        "oidc", options =>
                        {
                            options.Authority = identityServerConfig.Authority;

                            options.ClientId = "map";
                            options.ClientSecret = "secret";
                            options.ResponseType = "code";

                            options.SignedOutRedirectUri = "https://localhost:6011";
                            options.SignedOutCallbackPath = "/OidcSignOutCallback";
                            options.CallbackPath = "/OidcSignInCallback";

                            options.Scope.Add("openid");
                            options.Scope.Add("profile");
                            options.Scope.Add("traffic");

                            options.SaveTokens = true;
                        });
        }

        /// <summary>
        ///     This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">
        ///     The <see cref="IApplicationBuilder"/>.
        /// </param>
        /// <param name="env">
        ///     The <see cref="IWebHostEnvironment"/>.
        /// </param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Todo: Find out what this is
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(
                endpoints => endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"));
        }
    }
}

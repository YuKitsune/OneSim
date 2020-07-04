// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Web
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using OneSim.Common.Application.Abstractions;
    using OneSim.Common.Infrastructure;
    using OneSim.Identity.Application.Abstractions;
    using OneSim.Identity.Infrastructure;
    using OneSim.Identity.Persistence;

    /// <summary>
    ///     The startup.
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
        ///     Configures and adds services to the <see cref="IServiceCollection"/> for dependency injection.
        /// </summary>
        /// <param name="services">
        ///     The <see cref="IServiceCollection"/>.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure the DbContext
            services.AddDbContext<IdentityDbContext>(
                options =>
                    options.UseNpgsql(Configuration.GetConnectionString("IdentityConnection")));
            services.AddTransient<IIdentityDbContext<User>, IdentityDbContext>();

            // Configure our custom services
            services.AddTransient<IEmailSender, SmtpEmailSender>();
            services.AddTransient<IUserService<User>, UserService>();
            services.AddTransient<IAuthenticationService<User>, AuthenticationService>();
            services.AddTransient<ITwoFactorAuthenticationService<User>, TwoFactorAuthenticationService>();

            // Configure Identity
            services.AddIdentity<User, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
                    .AddEntityFrameworkStores<IdentityDbContext>()
                    .AddDefaultTokenProviders();

            // Configure IdentityServer
            IIdentityServerBuilder builder = services.AddIdentityServer(
                                                          options =>
                                                          {
                                                              options.Events.RaiseErrorEvents = true;
                                                              options.Events.RaiseInformationEvents = true;
                                                              options.Events.RaiseFailureEvents = true;
                                                              options.Events.RaiseSuccessEvents = true;
                                                              options.UserInteraction.LoginUrl = "/Account/Login";
                                                              options.UserInteraction.LogoutUrl = "/Account/Logout";
                                                          })
                                                     .AddInMemoryIdentityResources(Config.Ids)
                                                     .AddInMemoryApiResources(Config.Apis)
                                                     .AddInMemoryClients(Config.Clients)
                                                     .AddAspNetIdentity<User>();

            // Todo: not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();

            // Setup the MVC stuff
            services.AddControllers();
            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        /// <summary>
        ///     Method called by the runtime. Use this method to configure the HTTP request pipeline.
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
                // Show exception page if in dev environment
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                // Use default error page otherwise
                app.UseExceptionHandler("/Home/Error");

                // The default HSTS value is 30 days.
                // You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();

                // Redirect to HTTPS only
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Account}/{action=Index}/{id?}");
                    endpoints.MapRazorPages();
                });
        }
    }
}

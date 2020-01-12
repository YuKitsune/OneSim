namespace OneSim.Identity.Web
{
    using System;
    using System.Reflection;

    using IdentityServer4.Services;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using Newtonsoft.Json.Serialization;

    using OneSim.Identity.Application;
    using OneSim.Identity.Application.Abstractions;
    using OneSim.Identity.Domain.Entities;
    using OneSim.Identity.Infrastructure;
    using OneSim.Identity.Persistence;
    using OneSim.Identity.Web.Services;

    using IUrlHelper = OneSim.Identity.Application.Abstractions.IUrlHelper;

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
            // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            services.Configure<CookiePolicyOptions>(options =>
                                                    {
                                                        options.CheckConsentNeeded = context => true;
                                                        options.MinimumSameSitePolicy = SameSiteMode.None;
                                                    });

            // Configure the database context
            string connectionString = Configuration.GetConnectionString("IdentityConnection");
            services.AddDbContext<ApplicationIdentityDbContext>(options => options.UseNpgsql(connectionString));

            // Assign the database context for identity
            services.AddDefaultIdentity<ApplicationUser>().AddEntityFrameworkStores<ApplicationIdentityDbContext>();

            // Configure password and username requirements
            services.Configure<IdentityOptions>(options =>
                                                {
                                                    // Password settings
                                                    options.Password.RequireDigit = true;
                                                    options.Password.RequireLowercase = true;
                                                    options.Password.RequireUppercase = true;
                                                    options.Password.RequireNonAlphanumeric = true;
                                                    options.Password.RequiredLength = 8;
                                                    options.Password.RequiredUniqueChars = 1;

                                                    // User settings
                                                    options.User.AllowedUserNameCharacters =
                                                        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                                                    options.User.RequireUniqueEmail = false;
                                                });

            // Add IdentityServer
            string currentAssemblyName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            // Todo: Move to settings
            string issuerUri = "http://localhost:5000";
            services.AddIdentityServer(x =>
                                       {
                                           x.IssuerUri = issuerUri;
                                           x.Authentication.CookieLifetime = TimeSpan.FromHours(2);
                                       })

                     // Todo: .AddSigningCredential(Certificate.Get())
                    .AddDeveloperSigningCredential()
                    .AddAspNetIdentity<ApplicationUser>()

                     // ConfigurationStoreDbContext
                     // dotnet ef migrations add CreateIdentityConfigSchema --context ConfigurationDbContext -o Data/Migrations/IdentityServer/Configuration
                    .AddConfigurationStore(options => options.ConfigureDbContext =
                                                          builder => builder.UseNpgsql(connectionString,
                                                                                       sqlOptions => sqlOptions.MigrationsAssembly(currentAssemblyName)))

                     // PersistedGrantDbContext
                     // dotnet ef migrations add CreateIdentityPersistedGrantSchema --context PersistedGrantDbContext -o Data/Migrations/IdentityServer/PersistedGrant
                    .AddOperationalStore(options => options.ConfigureDbContext =
                                                        builder => builder.UseNpgsql(connectionString,
                                                                                     sqlOptions => sqlOptions.MigrationsAssembly(currentAssemblyName)))
                    .Services.AddTransient<IProfileService, ProfileService>();

            // Determine which EmailSender to use
            Type emailSenderType = GetEmailSenderType();
            services.AddScoped(typeof(IEmailSender), emailSenderType);

            // Add other Dependencies
            services.AddScoped<IIdentityDbContext, ApplicationIdentityDbContext>();
            services.AddScoped<IUrlHelper, UrlHelper>();
            services.AddScoped<AuthenticationService>();
            services.AddScoped<UserService>();

            // Use Newtonsoft.Json for controller actions. Just makes things easier on the client side if we're all
            // using the same thing.
            services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
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

            app.UseStaticFiles();

            // app.UseHttpsRedirection();
            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllerRoute(name: "default",
                                                                       pattern:
                                                                       "{controller=Authentication}/{action=LogIn}"));
        }

        /// <summary>
        ///     Gets the <see cref="Type"/> of <see cref="IEmailSender"/> in use.
        /// </summary>
        /// <returns>
        ///     The <see cref="Type"/> of <see cref="IEmailSender"/> to use.
        /// </returns>
        private Type GetEmailSenderType()
        {
            // Todo: Refactor this to use Reflection or something, means less effort later when we add new mail providers.
            SmtpSettings smtpSettings = Configuration.GetSection("SmtpSettings").Get<SmtpSettings>();

            if (smtpSettings != null) return typeof(SmtpEmailSender);

            MailgunSettings mailgunSettings = Configuration.GetSection("MailgunSettings").Get<MailgunSettings>();

            if (mailgunSettings != null) return typeof(MailgunEmailSender);

            throw new Exception("No email sender found.");
        }
    }
}
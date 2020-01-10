namespace OneSim.Identity.Web
{
    using System;
    using System.Text;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.IdentityModel.Tokens;

    using Newtonsoft.Json.Serialization;

    using OneSim.Identity.Application;
    using OneSim.Identity.Application.Abstractions;
    using OneSim.Identity.Domain.Entities;
    using OneSim.Identity.Infrastructure;
    using OneSim.Identity.Persistence;

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
            services.AddDbContext<ApplicationIdentityDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("IdentityConnection")));

            // Assign the database context for identity
            services.AddDefaultIdentity<ApplicationUser>().AddEntityFrameworkStores<ApplicationIdentityDbContext>();

            // Get the JWT settings
            IConfigurationSection tokenSettingsSection = Configuration.GetSection("TokenSettings");
            TokenSettings tokenSettings = tokenSettingsSection.Get<TokenSettings>();
            byte[] secret = Encoding.ASCII.GetBytes(tokenSettings.Secret);

            // Configure JWT authentication
            services.AddAuthentication(x =>
                                       {
                                           x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                                           x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                                       })
                    .AddJwtBearer(x =>
                                  {
                                      x.RequireHttpsMetadata = false;
                                      x.SaveToken = true;
                                      x.TokenValidationParameters = new TokenValidationParameters
                                                                    {
                                                                        ValidateIssuerSigningKey = true,
                                                                        IssuerSigningKey = new SymmetricSecurityKey(secret),
                                                                        ValidateIssuer = false,
                                                                        ValidateAudience = false
                                                                    };
                                  });

            // Determine which EmailSender to use
            Type emailSenderType = GetEmailSenderType();
            services.AddScoped(typeof(IEmailSender), emailSenderType);

            // Add other Dependencies
            services.AddScoped<IIdentityDbContext, ApplicationIdentityDbContext>();
            services.AddScoped<ITokenFactory, JwtFactory>();
            services.AddScoped<IUrlHelper, UrlHelper>();
            services.AddScoped<AuthenticationService>();
            services.AddScoped<UserService>();

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

            app.UseHttpsRedirection();
            app.UseRouting();
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
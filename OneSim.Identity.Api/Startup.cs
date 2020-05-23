// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Identity.Api
{
    using System;
    using System.Text;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;

    using OneSim.Common.Application.Abstractions;
    using OneSim.Common.Infrastructure;
    using OneSim.Identity.Api.Configuration;
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

            // Configure Identity
            services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
                    .AddEntityFrameworkStores<IdentityDbContext>();
            services.Configure<IdentityOptions>(
                options =>
                {
                    // Password settings
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequiredLength = 8;
                    options.Password.RequiredUniqueChars = 1;

                    // Lockout settings
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    options.Lockout.MaxFailedAccessAttempts = 10;
                    options.Lockout.AllowedForNewUsers = true;

                    // User settings
                    options.User.AllowedUserNameCharacters =
                        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";
                    options.User.RequireUniqueEmail = true;
                });

            // Configure JSON Web Tokens
            JwtSettings jwtSettings = Configuration.GetSection("JwtSettings").Get<JwtSettings>();
            string secret = jwtSettings.Secret;
            byte[] secretBytes = Encoding.ASCII.GetBytes(secret);
            services.AddAuthentication(
                         x =>
                         {
                             x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                             x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                         })
                    .AddJwtBearer(
                         x =>
                         {
                             x.SaveToken = true;
                             x.TokenValidationParameters = new TokenValidationParameters
                                                           {
                                                               ValidateIssuerSigningKey = true,
                                                               IssuerSigningKey = new SymmetricSecurityKey(secretBytes),
                                                               ValidateIssuer = false,
                                                               ValidateAudience = false
                                                           };
                         });
            services.AddTransient<ITokenService, JsonWebTokenService>(x => new JsonWebTokenService(secret));

            // Setup the MVC stuff
            services.AddControllers();
            services.AddCors();
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
            app.UseRouting();
            app.UseCors(
                builder => builder
                          .AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Strato Systems Pty. Ltd.">
//   Copyright (c) Strato Systems Pty. Ltd. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OneSim.Traffic.Api
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    using Hangfire;
    using Hangfire.PostgreSql;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using Newtonsoft.Json.Serialization;

    using OneSim.Common.Domain.Configuration;
    using OneSim.Traffic.Application;
    using OneSim.Traffic.Application.Abstractions;
    using OneSim.Traffic.Domain.Attributes;
    using OneSim.Traffic.Domain.Entities;
    using OneSim.Traffic.Domain.ValueObjects.Converters;
    using OneSim.Traffic.Infrastructure;
    using OneSim.Traffic.Persistence;

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
            // Configure the database context
            services.AddDbContext<TrafficDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("TrafficDataConnection")));
            services.AddDbContext<HistoricalDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("HistoricalConnection")));

            TrafficApiSettings settings = Configuration.GetSection("TrafficApiSettings").Get<TrafficApiSettings>();

            // Todo: Custom domain exception
            if (settings == null) throw new Exception("The TrafficApiSettings were not found in the configuration file.");

            NetworkType targetNetwork = settings.TargetNetwork;

            // Get the TrafficDataProvider
            Type statusFileProviderType = GetImplementationFor<ITrafficDataProvider>(targetNetwork, "OneSim.Traffic.Infrastructure");
            services.AddScoped(typeof(ITrafficDataProvider), statusFileProviderType);

            // Get the TrafficDataParser
            Type statusFileParserType = GetImplementationFor<ITrafficDataParser>(targetNetwork, "OneSim.Traffic.Infrastructure");
            services.AddScoped(typeof(ITrafficDataParser), statusFileParserType);

            // Add DbContext Interfaces
            services.AddScoped<ITrafficDbContext, TrafficDbContext>();
            services.AddScoped<IHistoricalDbContext, HistoricalDbContext>();

            // Add Authentication
            IdentityServerConfig identityServerConfig = Configuration.GetSection("IdentityServer").Get<IdentityServerConfig>();
            services.AddAuthentication("Bearer")
                    .AddJwtBearer(
                         "Bearer",
                         options =>
                         {
                             options.Authority = identityServerConfig.Authority;
                             options.Audience = "traffic";

                             // Todo: Find a better way to determine if we're in development, would like to use env.IsDevelopment()
                             options.RequireHttpsMetadata = !Debugger.IsAttached;
                         });

            // Add Hangfire services
            services.AddHangfire(
                configuration => configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                                              .UseSimpleAssemblyNameTypeSerializer()
                                              .UseRecommendedSerializerSettings()
                                              .UsePostgreSqlStorage(
                                                   Configuration.GetConnectionString("HangfireConnection")));

            // Add the processing server as IHostedService
            services.AddHangfireServer();

            // Use Newtonsoft.Json for controller actions. Just makes things easier on the client side if we're all
            // using the same thing.
            services.AddControllers()
                    .AddNewtonsoftJson(
                         options =>
                         {
                             options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                             options.SerializerSettings.Converters.Add(new SquawkCodeConverter());
                         });

            services.AddCors(
                options => options.AddPolicy(
                    "AllowApi",
                    builder =>
                        builder
                           .WithOrigins("https://localhost:5001")
                           .AllowAnyHeader()
                           .WithMethods("GET", "POST")
                           .AllowCredentials()));

            // Add SignalR and the TrafficNotifier Interface
            services.AddSignalR();
            services.AddTransient<ITrafficNotifier, SignalRTrafficNotifier>();
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

            // Add these things for some reverse proxy thing that i don't understand
            app.UseForwardedHeaders(
                new ForwardedHeadersOptions
                {
                    ForwardedHeaders =
                        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });

            // TODO: Add authentication with the Identity server over the hangfire dashboard
            app.UseHangfireServer();
            app.UseHangfireDashboard();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowApi");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(
                endpoints =>
                {
                    // Map controller endpoints
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=TrafficData}/{action=All}/{id?}");

                    // Map SignalR hub
                    endpoints.MapHub<TrafficDataHub>("/TrafficDataHub");
                });

            // Setup the Traffic Data Refresh job
            TrafficApiSettings settings = Configuration.GetSection("TrafficApiSettings").Get<TrafficApiSettings>();

            // Todo: Custom domain exception
            if (settings == null) throw new Exception("The TrafficApiSettings were not found in the configuration file.");

            int dataRefreshMinutes = settings.DataRefreshInterval;
            RecurringJob.AddOrUpdate<OnlineTrafficService>(
                "UpdateTrafficData",
                s => s.TryUpdateTrafficDataAsync(),
                $"*/{dataRefreshMinutes} * * * *");
        }

        // Todo: Move this somewhere more accessable

        /// <summary>
        ///     Gets the <see cref="Type"/> which implements the <typeparamref name="T"/> for the specified <see cref="NetworkType"/>.
        /// </summary>
        /// <param name="networkType">
        ///        The <see cref="NetworkType"/>.
        /// </param>
        /// <param name="assemblyName">
        ///        The name of the <see cref="Assembly"/> where the <see cref="Type"/> might be found.
        /// </param>
        /// <typeparam name="T">
        ///        The parent type.
        /// </typeparam>
        /// <returns>
        ///        The derived <see cref="Type"/>.
        /// </returns>
        private static Type GetImplementationFor<T>(NetworkType networkType, string assemblyName)
        {
            Type parentType = typeof(T);
            Assembly assembly = Assembly.Load(assemblyName);
            List<Type> types = assembly.GetTypes()
                                       .Where(
                                            p => p != parentType &&
                                                 parentType.IsAssignableFrom(p))
                                       .ToList();

            Type derivedType = null;
            foreach (Type possibleType in types)
            {
                // Check the network attribute
                NetworkAttribute[] attribute = possibleType.GetCustomAttributes<NetworkAttribute>().ToArray();

                // Ignore if there are no NetworkAttributes
                if (!attribute.Any()) continue;

                // Ignore if there are no matching networks
                if (attribute.All(n => n.Type != networkType)) continue;

                // If we've found a matching network type, make sure we don't have any other candidates
                // Todo: Custom domain exception
                if (derivedType != null) throw new Exception($"Multiple types found inheriting {parentType.Name} for Network {networkType.ToString()}. Please ensure there is only one implementation of {parentType.Name} per network.");

                // Found a type with no conflicts
                derivedType = possibleType;
            }

            // Check we found a match
            // Todo: Custom domain exception
            if (derivedType == null) throw new Exception($"No types found inheriting {parentType.Name} for Network {networkType.ToString()}. Please ensure there is an implementation of {parentType.Name} with a {nameof(NetworkAttribute)} specifying the {nameof(NetworkType)}.");

            return derivedType;
        }
    }
}

namespace OneSim.Api.Map
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	using Hangfire;
	using Hangfire.PostgreSql;

	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;

	using Newtonsoft.Json.Serialization;

	using OneSim.Api.Map.Data;
	using OneSim.Map.Application;
	using OneSim.Map.Application.Abstractions;
	using OneSim.Map.Domain.Attributes;
	using OneSim.Map.Domain.Entities;
	using OneSim.Map.Persistence;

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
			services.AddDbContext<TrafficDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("StatusConnection")));
			services.AddDbContext<HistoricalDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("HistoricalConnection")));

			MapApiSettings settings = Configuration.GetSection("MapApiSettings").Get<MapApiSettings>();

			// Todo: Custom domain exception
			if (settings == null) throw new Exception("The MapApiSettings were not found in the configuration file.");

			NetworkType targetNetwork = settings.TargetNetwork;

			// Get the StatusFileProvider
			Type statusFileProviderType = GetImplementationFor<ITrafficDataProvider>(targetNetwork, "OneSim.Map.Infrastructure");
			services.AddScoped(typeof(ITrafficDataProvider), statusFileProviderType);

			// Get the StatusFileParser
			Type statusFileParserType = GetImplementationFor<ITrafficDataParser>(targetNetwork, "OneSim.Map.Infrastructure");
			services.AddScoped(typeof(ITrafficDataParser), statusFileParserType);

			// Add DbContext Interfaces
			services.AddScoped<ITrafficDbContext, TrafficDbContext>();
			services.AddScoped<IHistoricalDbContext, HistoricalDbContext>();

			// Add Hangfire services
			services.AddHangfire(configuration => configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
															   .UseSimpleAssemblyNameTypeSerializer()
															   .UseRecommendedSerializerSettings()
															   .UsePostgreSqlStorage(Configuration.GetConnectionString("HangfireConnection")));

			// Add the processing server as IHostedService
			services.AddHangfireServer();

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

			// TODO: Add authentication with the Identity server over the hangfire dashboard
			app.UseHangfireServer();
			app.UseHangfireDashboard();

			app.UseHttpsRedirection();
			app.UseRouting();
			app.UseEndpoints(endpoints =>
								 endpoints.MapControllerRoute(name: "default",
															  pattern: "{controller=Status}/{action=All}/{id?}"));

			// Setup the Status Data Refresh job
			MapApiSettings settings = Configuration.GetSection("MapApiSettings").Get<MapApiSettings>();

			// Todo: Custom domain exception
			if (settings == null) throw new Exception("The MapApiSettings were not found in the configuration file.");

			int dataRefreshMinutes = settings.DataRefreshInterval;
			RecurringJob.AddOrUpdate<OnlineTrafficService>("UpdateStatusData",
													s => s.UpdateTrafficDataAsync(),
													$"*/{dataRefreshMinutes} * * * *");
		}

		// Todo: Move this somewhere more accessable

		/// <summary>
		/// 	Gets the <see cref="Type"/> which implements the <typeparamref name="T"/> for the specified <see cref="NetworkType"/>.
		/// </summary>
		/// <param name="networkType">
		///		The <see cref="NetworkType"/>.
		/// </param>
		/// <param name="assemblyName">
		///		The name of the <see cref="Assembly"/> where the <see cref="Type"/> might be found.
		/// </param>
		/// <typeparam name="T">
		///		The parent type.
		/// </typeparam>
		/// <returns>
		///		The derived <see cref="Type"/>.
		/// </returns>
		private static Type GetImplementationFor <T>(NetworkType networkType, string assemblyName)
		{
			Type parentType = typeof(T);
			Assembly assembly = Assembly.Load(assemblyName);
			List<Type> types = assembly.GetTypes()
									   .Where(p => p != parentType &&
												   parentType.IsAssignableFrom(p))
									   .ToList();

			Type derivedType = null;
			foreach (Type possibleType in types)
			{
				// Check the network attribute
				NetworkAttribute attribute = possibleType.GetCustomAttribute<NetworkAttribute>();

				// Ignore if there is no NetworkAttribute
				if (attribute == null) continue;

				// Ignore if the networks don't match
				if (attribute.Type != networkType) continue;

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
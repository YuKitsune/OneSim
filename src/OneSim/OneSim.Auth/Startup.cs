namespace OneSim.Auth
{
	using System;

	using IdentityServer4.Services;

	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;

	using OneSim.Auth.Services;
	using OneSim.Identity.Domain.Entities;
	using OneSim.Identity.Persistence;

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
			services.AddDbContext<ApplicationIdentityDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("IdentityConnection")));

			// Assign the database context for identity
			services.AddDefaultIdentity<ApplicationUser>().AddEntityFrameworkStores<ApplicationIdentityDbContext>();

			// Add IdentityServer
			string configConnection = Configuration.GetConnectionString("IdentityConfigConnection");
			string storeConnection = Configuration.GetConnectionString("IdentityStoreConnection");
			services.AddIdentityServer(x =>
									   {
										   x.IssuerUri = "null";
										   x.Authentication.CookieLifetime = TimeSpan.FromHours(2);
									   })

					 // Todo:.AddSigningCredential(Certificate.Get())
					.AddDeveloperSigningCredential()
					.AddAspNetIdentity<ApplicationUser>()
					.AddConfigurationStore(options => options.ConfigureDbContext =
														  builder => builder.UseNpgsql(configConnection))
					.AddOperationalStore(options => options.ConfigureDbContext =
														builder => builder.UseNpgsql(storeConnection))
					.Services.AddTransient<IProfileService, ProfileService>();

			services.AddControllersWithViews();
			services.AddRazorPages();
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
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");

				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			// Add IdentityServer
			app.UseIdentityServer();

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
							 {
								 endpoints.MapControllerRoute(name: "default",
															  pattern: "{controller=Home}/{action=Index}/{id?}");
								 endpoints.MapRazorPages();
							 });
		}
	}
}
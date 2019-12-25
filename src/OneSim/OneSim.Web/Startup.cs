namespace OneSim.Web
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    ///     The Startup.
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
        ///     The method that gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">
        ///     The <see cref="IServiceCollection"/>.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
                                                    {
                                                        // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                                                        options.CheckConsentNeeded = context => true;
                                                        options.MinimumSameSitePolicy = SameSiteMode.None;
                                                    });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        /// <summary>
        ///     The method that gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">
        ///     The <see cref="IApplicationBuilder"/>.
        /// </param>
        /// <param name="env">
        ///     The <see cref="Microsoft.AspNetCore.Hosting.IHostingEnvironment"/>.
        /// </param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
                             {
                                 endpoints.MapRazorPages();
                                 endpoints.MapControllerRoute(name: "default",
                                                              pattern: "{controller=Home}/{action=Index}");
                             });
        }
    }
}

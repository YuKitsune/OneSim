namespace OneSim.Map
{
	using Microsoft.AspNetCore;
	using Microsoft.AspNetCore.Hosting;

	/// <summary>
	/// 	The program.
	/// </summary>
	public class Program
	{
		/// <summary>
		/// 	The main application entry point.
		/// </summary>
		/// <param name="args">
		///		The arguments.
		/// </param>
		public static void Main(string[] args) => CreateWebHostBuilder(args).Build().Run();

		/// <summary>
		///     Creates the <see cref="IWebHostBuilder"/>.
		/// </summary>
		/// <param name="args">
		///     The arguments.
		/// </param>
		/// <returns>
		///     The <see cref="IWebHostBuilder"/>.
		/// </returns>
		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
	}
}
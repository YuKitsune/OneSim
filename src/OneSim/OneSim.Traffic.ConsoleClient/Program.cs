namespace OneSim.Traffic.ConsoleClient
{
	using System;
	using System.Net;
	using System.Net.Http;
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.SignalR.Client;

	public class Program
	{
		public static HubConnection Connection { get; set; }

		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");

			Console.WriteLine("Traffic API server domain: ");
			string domainName = Console.ReadLine();

			string url = domainName + "/TrafficDataHub";
			Console.WriteLine($"Building connection for \"{url}\".");
			Connection = new HubConnectionBuilder()
						.WithUrl(url,
								 conf => conf.HttpMessageHandlerFactory = (x) => new HttpClientHandler
																				 {
																					 ServerCertificateCustomValidationCallback
																						 = HttpClientHandler
																							.DangerousAcceptAnyServerCertificateValidator,
																				 })
						.Build();

			Connection.StartAsync()
					  .ContinueWith(task =>
									{
										if (task.IsFaulted)
										{
											Console.WriteLine($"There was an error opening the connection:{task.Exception.GetBaseException()}");
										}
										else
										{
											Console.WriteLine("Connected");
										}
									})
					  .Wait();

			Connection.On("NewTrafficDataAvailable", () => Console.WriteLine("New Traffic Data Available."));

			Console.Read();
			Connection.StopAsync();
		}
	}
}
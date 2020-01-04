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
		public static string DomainName { get; set; }

		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");

			Console.WriteLine("Traffic API server domain: ");
			DomainName = Console.ReadLine();

			string url = DomainName + "/TrafficDataHub";
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
			Connection.On("PilotSelected", (string callsign) => GetPilotInfo(callsign));

			Console.Read();
			Connection.StopAsync();
		}

		static async void GetPilotInfo(string callsign)
		{
			using WebClient client = new WebClient();
			string pilotJson = await client.DownloadStringTaskAsync(DomainName + "/TrafficData/Pilot?callsign=" + callsign);
			Console.WriteLine(pilotJson);
		}
	}
}
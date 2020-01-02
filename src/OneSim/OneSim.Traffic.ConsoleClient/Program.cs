using System;

namespace OneSim.Traffic.ConsoleClient
{
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.SignalR.Client;

	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");

			Console.WriteLine("Traffic API server domain: ");
			string domainName = Console.ReadLine();

			string url = domainName + "/TrafficDataHub";
			Console.WriteLine($"Building connection for \"{url}\".");

			try
			{

				HubConnection = connection = new HubConnectionBuilder()
											.WithUrl(url)
											.Build();

				connection.Closed += async (error) =>
									 {
										 await Task.Delay(new Random().Next(0,5) * 1000);
										 await connection.StartAsync();
									 };
			}
			catch (Exception ex)
			{
				
			}
		}
	}
}
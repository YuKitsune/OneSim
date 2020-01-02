namespace OneSim.Traffic.Infrastructure
{
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.SignalR;

	using OneSim.Traffic.Application.Abstractions;

	/// <summary>
	/// 	The SignalR <see cref="ITrafficNotifier"/> implementation for sending traffic data notifications over a
	/// 	SignalR connection.
	/// </summary>
	public class SignalRTrafficNotifier : Hub, ITrafficNotifier
	{
		/// <summary>
		/// 	Notifies the subscribers that there is new traffic data available.
		/// </summary>
		public void NewTrafficDataAvailable() => NewTrafficDataAvailableAsync().GetAwaiter().GetResult();

		/// <summary>
		/// 	Notifies the subscribers that there is new traffic data available as an asynchronous operation.
		/// </summary>
		public async Task NewTrafficDataAvailableAsync() => await Clients.All.SendAsync("NewTrafficDataAvailable");
	}
}
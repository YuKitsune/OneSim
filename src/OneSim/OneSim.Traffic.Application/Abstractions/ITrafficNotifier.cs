namespace OneSim.Traffic.Application.Abstractions
{
	using System.Threading.Tasks;

	/// <summary>
	/// 	Interface for sending notifications to subscribers.
	/// </summary>
	public interface ITrafficNotifier
	{
		/// <summary>
		/// 	Notifies the subscribers that there is new traffic data available.
		/// </summary>
		void NewTrafficDataAvailable();

		/// <summary>
		/// 	Notifies the subscribers that there is new traffic data available as an asynchronous operation.
		/// </summary>
		Task NewTrafficDataAvailableAsync();
	}
}
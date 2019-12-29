namespace OneSim.Map.Infrastructure
{
	using System;

	/// <summary>
	/// 	The sections of the VATSIM traffic data file.
	/// </summary>
	public enum VatsimTrafficDataFileSection
	{
		/// <summary>
		/// 	The general information section.
		/// </summary>
		General,

		/// <summary>
		/// 	The voice servers section.
		/// </summary>
		[Obsolete("VATSIM no longer uses voice servers. However the section still exists.")]
		VoiceServers,

		/// <summary>
		/// 	The clients section.
		/// </summary>
		Clients,

		/// <summary>
		/// 	The servers section.
		/// </summary>
		Servers,

		/// <summary>
		/// 	The prefile section.
		/// </summary>
		Prefile
	}
}
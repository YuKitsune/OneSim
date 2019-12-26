namespace OneSim.Map.Infrastructure
{
	using System;

	/// <summary>
	/// 	The Status File sections found in the VATSIM Status file.
	/// </summary>
	public enum VatsimStatusFileSection
	{
		/// <summary>
		/// 	The general information section.
		/// </summary>
		General,

		/// <summary>
		/// 	The voice servers section.
		/// </summary>
		[Obsolete("No networks are using voice servers anymore.")]
		VoiceServers,

		/// <summary>
		/// 	The clients section.
		/// </summary>
		Clients,

		/// <summary>
		/// 	The server section.
		/// </summary>
		Servers,

		/// <summary>
		/// 	The prefile section.
		/// </summary>
		Prefile
	}
}
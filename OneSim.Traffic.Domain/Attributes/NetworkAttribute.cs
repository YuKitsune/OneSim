namespace OneSim.Traffic.Domain.Attributes
{
	using System;

	using OneSim.Traffic.Domain.Entities;

	/// <summary>
	/// 	The Online Flight Simulation Network <see cref="Attribute"/>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class NetworkAttribute : Attribute
	{
		/// <summary>
		/// 	Gets the <see cref="NetworkType"/> which the current <see cref="Attribute"/> holder is valid for.
		/// </summary>
		public NetworkType Type { get; }

		/// <summary>
		/// 	Initializes a new instance of the <see cref="NetworkAttribute"/> class.
		/// </summary>
		/// <param name="type">
		///		The <see cref="NetworkType"/>.
		/// </param>
		public NetworkAttribute(NetworkType type) => Type = type;
	}
}
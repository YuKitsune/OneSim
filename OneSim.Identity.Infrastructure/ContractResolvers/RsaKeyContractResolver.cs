namespace OneSim.Identity.Infrastructure.ContractResolvers
{
	using System.Reflection;

	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;

	/// <summary>
	/// 	The RSA Key <see cref="DefaultContractResolver"/>.
	/// </summary>
	internal class RsaKeyContractResolver : DefaultContractResolver
	{
		/// <summary>
		/// 	Creates a <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for the given <see cref="T:System.Reflection.MemberInfo" />.
		/// </summary>
		/// <param name="memberSerialization">
		/// 	The member's parent <see cref="T:Newtonsoft.Json.MemberSerialization" />.
		/// </param>
		/// <param name="member">
		/// 	The member to create a <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for.
		/// </param>
		/// <returns>
		/// 	A created <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for the given <see cref="T:System.Reflection.MemberInfo" />.
		/// </returns>
		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			JsonProperty property = base.CreateProperty(member, memberSerialization);

			property.Ignored = false;

			return property;
		}
	}
}
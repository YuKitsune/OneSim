namespace OneSim.Map.Domain.ValueObjects
{
	using System;

	using Newtonsoft.Json;

	/// <summary>
	/// 	Converts the <see cref="SquawkCode"/> to and from JSON.
	/// </summary>
	public class SquawkCodeConverter : JsonConverter<SquawkCode>
	{
		/// <summary>
		/// 	Writes the JSON representation of the <see cref="SquawkCode"/>.
		/// </summary>
		/// <param name="writer">
		///		The <see cref="JsonWriter"/> to write to.
		/// </param>
		/// <param name="value">
		///		The <see cref="SquawkCode"/> to write.
		/// </param>
		/// <param name="serializer">
		///		The calling <see cref="JsonSerializer"/>.
		/// </param>
		public override void WriteJson(JsonWriter writer, SquawkCode value, JsonSerializer serializer)
		{
			writer.WriteValue(value.ToString());
		}

		/// <summary>
		/// 	Reads the JSON representation of the <see cref="SquawkCode"/>.
		/// </summary>
		/// <param name="reader">
		///		The <see cref="JsonReader"/> to read from.
		/// </param>
		/// <param name="objectType">
		///		The type of object.
		/// </param>
		/// <param name="existingValue">
		///		The existing value of the <see cref="SquawkCode"/> being read.
		/// </param>
		/// <param name="hasExistingValue">
		///		The <paramref name="existingValue"/> has a value.
		/// </param>
		/// <param name="serializer">
		///		The calling <see cref="JsonSerializer"/>.
		/// </param>
		/// <returns>
		///		The <see cref="SquawkCode"/>.
		/// </returns>
		public override SquawkCode ReadJson(JsonReader reader, Type objectType, SquawkCode existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			string s = (string) reader.Value;

			return new SquawkCode(s);
		}
	}
}
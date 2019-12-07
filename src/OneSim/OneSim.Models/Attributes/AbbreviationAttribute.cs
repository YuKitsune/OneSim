namespace OneSim.Models.Attributes
{
	using System;

	/// <summary>
	/// 	The Abbreviation <see cref="Attribute"/> used to describe properties or values in the form of an abbreviation,
	/// 	whilst also providing the meaning of the abbreviation. E.g: ICAO (International Civil Aviation Organisation).
	/// </summary>
	public class AbbreviationAttribute : Attribute
	{
		/// <summary>
		/// 	Gets the abbreviation.
		/// </summary>
		public string Abbreviation { get; }

		/// <summary>
		/// 	Gets the meaning of the <see cref="Abbreviation"/>.
		/// </summary>
		public string Meaning { get; }

		/// <summary>
		/// 	Initializes a new instance of the <see cref="AbbreviationAttribute"/> class.
		/// </summary>
		/// <param name="abbreviation">
		///		The abbreviation.
		/// </param>
		/// <param name="meaning">
		///		The meaning of the abbreviation.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///		Throws an <see cref="ArgumentNullException"/> if either <paramref name="abbreviation"/> or
		/// 	<paramref name="meaning"/> are null or empty.
		/// </exception>
		public AbbreviationAttribute(string abbreviation, string meaning)
		{
			if (string.IsNullOrEmpty(abbreviation)) throw new ArgumentNullException(nameof(abbreviation));
			if (string.IsNullOrEmpty(meaning)) throw new ArgumentNullException(nameof(meaning));

			Abbreviation = abbreviation;
			Meaning = meaning;
		}
	}
}
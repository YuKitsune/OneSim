namespace OneSim.Map.Application.Abstractions
{
	/// <summary>
	/// 	The status file parser interface.
	/// </summary>
	public interface IStatusFileParser
	{
		/// <summary>
		/// 	Parses the given <see cref="string"/> as a status File.
		/// </summary>
		/// <param name="rawStatusFile">
		///		The raw status file.
		/// </param>
		/// <returns>
		///		The <see cref="StatusFileParseResult"/>.
		/// </returns>
		StatusFileParseResult Parse(string rawStatusFile);
	}
}
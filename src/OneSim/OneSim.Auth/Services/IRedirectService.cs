namespace OneSim.Auth.Services
{
	/// <summary>
	/// 	The Redirect Service Interface.
	/// </summary>
	public interface IRedirectService
	{
		/// <summary>
		/// 	Extracts the redirect URI from the given URL.
		/// </summary>
		/// <param name="url">
		///		The return URL.
		/// </param>
		/// <returns>
		///		The extracted URL.
		/// </returns>
		string ExtractRedirectUriFromReturnUrl(string url);
	}
}
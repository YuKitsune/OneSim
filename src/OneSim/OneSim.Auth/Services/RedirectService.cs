namespace OneSim.Auth.Services
{
	using System.Text.RegularExpressions;

	/// <summary>
	/// 	The <see cref="IRedirectService"/> implementation.
	/// </summary>
	public class RedirectService : IRedirectService
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
		public string ExtractRedirectUriFromReturnUrl(string url)
		{
			string result = "";

			// Attempt to decode the URL
			string decodedUrl = System.Net.WebUtility.HtmlDecode(url);
			string[] results = Regex.Split(decodedUrl, "redirect_uri=");

			// If the string is too short, then return an empty string
			if (results.Length < 2) return string.Empty;

			result = results[1];

			string splitKey = "";
			splitKey = result.Contains("signin-oidc") ? "signin-oidc" : "scope";

			results = Regex.Split(result, splitKey);

			if (results.Length < 2) return "";

			result = results[0];

			return result.Replace("%3A", ":").Replace("%2F", "/").Replace("&", "");
		}
	}
}
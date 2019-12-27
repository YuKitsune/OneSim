namespace OneSim.Identity.Application.Abstractions
{
	using OneSim.Identity.Domain.Entities;

	/// <summary>
	/// 	The Token Factory Interface.
	/// </summary>
	public interface ITokenFactory
	{
		/// <summary>
		/// 	Generates a token for the given <paramref name="user"/>.
		/// </summary>
		/// <param name="user">
		///		The <see cref="ApplicationUser"/>.
		/// </param>
		/// <returns>
		///		The token.
		/// </returns>
		string GenerateToken(ApplicationUser user);
	}
}
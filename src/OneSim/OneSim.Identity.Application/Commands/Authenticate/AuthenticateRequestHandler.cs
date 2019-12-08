namespace OneSim.Identity.Application.Commands.Authenticate
{
	using System;
	using System.IdentityModel.Tokens.Jwt;
	using System.Security.Claims;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;

	using MediatR;

	using Microsoft.AspNetCore.Identity;
	using Microsoft.Extensions.Options;
	using Microsoft.IdentityModel.Tokens;

	using OneSim.Identity.Application.Interfaces;
	using OneSim.Identity.Application.Queries.GetUserById;
	using OneSim.Identity.Application.Queries.GetUserByUserNameOrEmail;
	using OneSim.Identity.Domain;
	using OneSim.Identity.Domain.Entities;
	using OneSim.Identity.Domain.Exceptions;

	/// <summary>
	/// 	The Authentication <see cref="IRequestHandler{TRequest, TResponse}"/>.
	/// </summary>
	public class AuthenticateRequestHandler : IRequestHandler<AuthenticateRequest, AuthenticateResponse>
	{
		/// <summary>
		/// 	The <see cref="IIdentityDbContext"/>.
		/// </summary>
		public readonly IIdentityDbContext DbContext;

		/// <summary>
		/// 	The <see cref="IMediator"/>.
		/// </summary>
		public readonly IMediator Mediator;

		/// <summary>
		/// 	The <see cref="SignInManager{TUser}"/>.
		/// </summary>
		public readonly SignInManager<ApplicationUser> SignInManager;

		/// <summary>
		/// 	The <see cref="TokenSettings"/>.
		/// </summary>
		public readonly IOptions<TokenSettings> Settings;

		/// <summary>
		/// 	Initializes a new instance of the <see cref="AuthenticateRequestHandler"/>.
		/// </summary>
		/// <param name="dbContext">
		///		The <see cref="IIdentityDbContext"/>.
		/// </param>
		/// <param name="mediator">
		///		The <see cref="IMediator"/>.
		/// </param>
		/// <param name="signInManager">
		/// 	The <see cref="SignInManager{TUser}"/>.
		/// </param>
		/// <param name="settings">
		///		The <see cref="TokenSettings"/>.
		/// </param>
		public AuthenticateRequestHandler(
			IIdentityDbContext dbContext,
			IMediator mediator,
			SignInManager<ApplicationUser> signInManager,
			IOptions<TokenSettings> settings)
		{
			DbContext = dbContext;
			Mediator = mediator;
			SignInManager = signInManager;
			Settings = settings;
		}

		/// <summary>
		/// 	Handles the <see cref="AuthenticateRequest"/>.
		/// </summary>
		/// <param name="request">
		///		The <see cref="AuthenticateRequest"/>.
		/// </param>
		/// <param name="cancellationToken">
		///		The <see cref="CancellationToken"/>.
		/// </param>
		/// <returns>
		///		The <see cref="AuthenticateResponse"/>.
		/// </returns>
		public async Task<AuthenticateResponse> Handle(AuthenticateRequest request, CancellationToken cancellationToken)
		{
			// Get the user
			ApplicationUser user =
				(await Mediator.Send(new GetUserByUserNameOrEmailRequest { UserNameOrEmail = request.UserNameOrEmail },
									 cancellationToken)).User;

			// Sign the user in
			SignInResult result = await SignInManager.PasswordSignInAsync(user, request.Password, true, false);

			// Todo: Implement 2FA
			if (result.RequiresTwoFactor) throw new NotImplementedException("Two Factor Authentication (2FA) has not been implemented yet.");

			// Throw if login failed
			if (!result.Succeeded) throw new AuthenticationFailedException($"Failed to authenticate user with ID \"{user.Id}\".");

			// Authentication successful, generate JSON Web Token (JWT)
			JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
			byte[] key = Encoding.ASCII.GetBytes(Settings.Value.Secret);
			SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
								  {
									  // Todo: Refine these settings
									  Subject = new ClaimsIdentity(new Claim[]
																   {
																	   new Claim(ClaimTypes.Name, user.Id)
																   }),
									  /*Expires = DateTime.UtcNow.AddDays(7),*/
									  SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
								  };
			SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
			string token = tokenHandler.WriteToken(securityToken);

			// Return authentication response
			return new AuthenticateResponse(token);
		}
	}
}
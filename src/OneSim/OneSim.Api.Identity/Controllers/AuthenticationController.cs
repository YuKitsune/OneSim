namespace OneSim.Api.Identity.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    using OneSim.Api.Data.Requests;
    using OneSim.Api.Data.Responses;
    using OneSim.Api.Identity.Data;
    using OneSim.Identity.Application;
    using OneSim.Identity.Application.Abstractions;
    using OneSim.Identity.Domain.Entities;

    /// <summary>
    ///     The Authentication <see cref="Controller"/>.
    /// </summary>
    public class AuthenticationController : Controller
    {
        /// <summary>
        ///     The <see cref="ApplicationIdentityDbContext"/>.
        /// </summary>
        private readonly ApplicationIdentityDbContext _dbContext;

        /// <summary>
        ///     The <see cref="AuthenticationService"/>.
        /// </summary>
        private readonly AuthenticationService _authenticationService;

        /// <summary>
        ///     The <see cref="ILogger{TCategoryName}"/>.
        /// </summary>
        private readonly ILogger<AuthenticationController> _logger;

        /// <summary>
        ///     The <see cref="ITokenFactory"/>.
        /// </summary>
        private readonly ITokenFactory _tokenFactory;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AuthenticationController"/> class.
        /// </summary>
        /// <param name="service">
        ///     The <see cref="AuthenticationService"/>.
        /// </param>
        /// <param name="dbContext">
        ///     The <see cref="ApplicationIdentityDbContext"/>.
        /// </param>
        /// <param name="logger">
        ///    The <see cref="ILogger{TCategoryName}"/>.
        /// </param>
        /// <param name="tokenFactory">
        ///    The <see cref="ITokenFactory"/>.
        /// </param>
        public AuthenticationController(
            AuthenticationService service,
            ApplicationIdentityDbContext dbContext,
            ILogger<AuthenticationController> logger,
            ITokenFactory tokenFactory)
        {
            _authenticationService = service;
            _dbContext = dbContext;
            _logger = logger;
            _tokenFactory = tokenFactory;
        }

        /// <summary>
        ///     Attempts to log the user in.
        /// </summary>
        /// <param name="request">
        ///     The <see cref="LogInRequest"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult"/> containing the <see cref="LogInResponse"/>, or <see cref="BaseResponse"/>
        ///     in the event of an error.
        /// </returns>
        [HttpPost]
        public async Task<ActionResult> LogIn([FromBody] LogInRequest request)
        {
            try
            {
                // Find the user
                ApplicationUser user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);

                if (user == null) return Json(new BaseResponse(ResponseStatus.Failure, "User with the given email and password not found."));

                // Log in
                Microsoft.AspNetCore.Identity.SignInResult result = await _authenticationService.LogIn(user, request.Password);

                // Reject if failed
                if (!result.Succeeded) return Json(new BaseResponse(ResponseStatus.Failure, "User with the given email and password not found."));

                // Alert client if two-factor is required
                if (result.RequiresTwoFactor)
                {
                    return Json(new LogInResponse(ResponseStatus.Success, "Two-Factor Authentication Token required.")
                                {
                                    TwoFactorAuthenticationRequired = true
                                });
                }

                // Generate JWT
                string token = _tokenFactory.GenerateToken(user);
                LogInResponse response = new LogInResponse(ResponseStatus.Success, "Log in successful.")
                                         {
                                             Token = token
                                         };

                return Json(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has occurred logging in a user with username \"{request.UserName}\".");

                return Json(new BaseResponse(ResponseStatus.Error, "An error has occurred processing the Log In request."));
            }
        }

        /// <summary>
        ///     Attempts to log the user in using Two-Factor Authentication.
        /// </summary>
        /// <param name="request">
        ///     The <see cref="TwoFactorAuthenticationLogInRequest"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult"/> containing the <see cref="LogInResponse"/>, or <see cref="BaseResponse"/>
        ///     in the event of an error.
        /// </returns>
        [HttpPost]
        public async Task<ActionResult> TwoFactorAuthenticationLogIn(
            [FromBody] TwoFactorAuthenticationLogInRequest request)
        {
            try
            {
                // Find the user
                ApplicationUser user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);

                if (user == null) return Json(new BaseResponse(ResponseStatus.Failure, "User not found."));

                // Log in
                Microsoft.AspNetCore.Identity.SignInResult result = await _authenticationService.TwoFactorAuthenticationLogIn(user, request.Token);

                // Reject if failed
                if (!result.Succeeded) return Json(new BaseResponse(ResponseStatus.Failure, "Two-Factor Authentication token rejected."));

                // Generate JWT
                string token = _tokenFactory.GenerateToken(user);
                LogInResponse response = new LogInResponse(ResponseStatus.Success, "Two-Factor Authentication token accepted.")
                                         {
                                             Token = token
                                         };

                return Json(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has occurred logging in user using 2FA with username \"{request.UserName}\".");

                return Json(new BaseResponse(ResponseStatus.Error, "An error has occurred processing the Two-Factor Authentication Log In request."));
            }
        }

        /// <summary>
        ///     Attempts to log the user in using a Two-Factor Authentication Recovery Code.
        /// </summary>
        /// <param name="request">
        ///     The <see cref="TwoFactorAuthenticationLogInRequest"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult"/> containing the <see cref="LogInResponse"/>, or <see cref="BaseResponse"/>
        ///     in the event of an error.
        /// </returns>
        public async Task<ActionResult> RecoveryCodeLogIn([FromBody] TwoFactorAuthenticationLogInRequest request)
        {
            try
            {
                // Find the user
                ApplicationUser user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);

                if (user == null) return Json(new BaseResponse(ResponseStatus.Failure, "User not found."));

                // Log in
                Microsoft.AspNetCore.Identity.SignInResult result = await _authenticationService.RecoveryCodeLogIn(user, request.Token);

                // Reject if failed
                if (!result.Succeeded) return Json(new BaseResponse(ResponseStatus.Failure, "Two-Factor Authentication recovery code rejected."));

                // Generate JWT
                string token = _tokenFactory.GenerateToken(user);
                LogInResponse response = new LogInResponse(ResponseStatus.Success, "Recovery Code accepted.")
                                         {
                                             Token = token
                                         };

                return Json(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has occurred logging in user using 2FA recovery code with username \"{request.UserName}\".");

                return Json(new BaseResponse(ResponseStatus.Error, "An error has occurred processing the Two-Factor Authentication Recovery Code Log In request."));
            }
        }
    }
}
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
    using OneSim.Identity.Application.Interfaces;
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
        ///     Initializes a new instance of t he <see cref="AuthenticationController"/> class.
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
        /// <param name="logInRequest">
        ///     The <see cref="LogInRequest"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        public async Task<ActionResult> LogIn([FromBody] LogInRequest logInRequest)
        {
            try
            {
                // Find the user
                ApplicationUser user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == logInRequest.UserName);

                if (user == null) return new UnauthorizedResult();

                // Log in
                Microsoft.AspNetCore.Identity.SignInResult result = await _authenticationService.LogIn(user, logInRequest.Password);

                // Reject if failed
                if (!result.Succeeded) return new UnauthorizedResult();

                // Alert client if two-factor is required
                if (!result.RequiresTwoFactor)
                {
                    return new JsonResult(new LogInResponse
                                          {
                                              TwoFactorAuthenticationRequired = false
                                          });
                }

                // Generate JWT
                string token = _tokenFactory.GenerateToken(user);
                LogInResponse response = new LogInResponse
                                         {
                                             Token = token
                                         };

                return new JsonResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Failed to log user in. Exception:{Environment.NewLine}{ex}");

                return StatusCode(500, "An error has occurred processing the log in request.");
            }
        }

        /// <summary>
        ///     Attempts to log the user in using Two-Factor Authentication.
        /// </summary>
        /// <param name="logInRequest">
        ///     The <see cref="TwoFactorAuthenticationLogInRequest"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        public async Task<ActionResult> TwoFactorAuthenticationLogIn(
            [FromBody] TwoFactorAuthenticationLogInRequest logInRequest)
        {
            try
            {
                // Find the user
                ApplicationUser user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == logInRequest.UserName);

                if (user == null) return new UnauthorizedResult();

                // Log in
                Microsoft.AspNetCore.Identity.SignInResult result = await _authenticationService.TwoFactorAuthenticationLogIn(user, logInRequest.Token);

                // Reject if failed
                if (!result.Succeeded) return new UnauthorizedResult();

                // Generate JWT
                string token = _tokenFactory.GenerateToken(user);
                LogInResponse response = new LogInResponse
                                         {
                                             Token = token
                                         };

                return new JsonResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Failed to log user in using Two-Factor authentication. Exception:{Environment.NewLine}{ex}");

                return StatusCode(500, "An error has occurred processing the Two-Factor Authentication log in request.");
            }
        }

        /// <summary>
        ///     Attempts to log the user in using a Two-Factor Authentication Recovery Code.
        /// </summary>
        /// <param name="logInRequest">
        ///     The <see cref="TwoFactorAuthenticationLogInRequest"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="ActionResult"/>.
        /// </returns>
        public async Task<ActionResult> RecoveryCodeLogIn([FromBody] TwoFactorAuthenticationLogInRequest logInRequest)
        {
            try
            {
                // Find the user
                ApplicationUser user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == logInRequest.UserName);

                if (user == null) return new UnauthorizedResult();

                // Log in
                Microsoft.AspNetCore.Identity.SignInResult result = await _authenticationService.RecoveryCodeLogIn(user, logInRequest.Token);

                // Reject if failed
                if (!result.Succeeded) return new UnauthorizedResult();

                // Generate JWT
                string token = _tokenFactory.GenerateToken(user);
                LogInResponse response = new LogInResponse
                                         {
                                             Token = token
                                         };

                return new JsonResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Failed to log user in using Two-Factor authentication. Exception:{Environment.NewLine}{ex}");

                return StatusCode(500, "An error has occurred processing the Two-Factor Authentication log in request.");
            }
        }
    }
}
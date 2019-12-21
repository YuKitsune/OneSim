namespace OneSim.Api.Identity.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using OneSim.Api.Data;
    using OneSim.Api.Data.Requests;
    using OneSim.Api.Identity.Data;
    using OneSim.Identity.Application;
    using OneSim.Identity.Application.Interfaces;
    using OneSim.Identity.Domain.Entities;
    using OneSim.Identity.Infrastructure;

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
        ///     The <see cref="ILogger"/>.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        ///     The <see cref="ITokenFactory"/>.
        /// </summary>
        private readonly ITokenFactory _tokenFactory;

        public AuthenticationController(AuthenticationService service, ApplicationIdentityDbContext dbContext, ILogger logger, ITokenFactory tokenFactory)
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
                // Todo: Account for 2FA
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
                _logger.LogCritical($"Failed to log user in. Exception:{Environment.NewLine}{ex}");
                return StatusCode(500, "An error has occurred processing the log in request.");
            }
        }
    }
}
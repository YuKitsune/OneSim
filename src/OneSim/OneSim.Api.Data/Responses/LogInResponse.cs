namespace OneSim.Api.Data.Responses
{
    /// <summary>
    ///     The Log In Request.
    /// </summary>
    public class LogInResponse
    {
        /// <summary>
        ///     Gets or sets a value indicating whether or not Two-Factor Authentication is required.
        /// </summary>
        public bool TwoFactorAuthenticationRequired { get; set; }

        /// <summary>
        ///     Gets or sets the token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogInResponse"/> class.
        /// </summary>
        public LogInResponse() => TwoFactorAuthenticationRequired = false;
    }
}
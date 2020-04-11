namespace OneSim.Api.Data.Responses
{
    /// <summary>
    ///     The Log In <see cref="BaseResponse"/>.
    /// </summary>
    public class LogInResponse : BaseResponse
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
        /// 	Initializes a new instance of the <see cref="LogInResponse"/> class.
        /// </summary>
        /// <param name="status">
        ///		The <see cref="ResponseStatus"/>.
        /// </param>
        /// <param name="message">
        ///		The message.
        /// </param>
        public LogInResponse(ResponseStatus status, string message) : base(status, message) => TwoFactorAuthenticationRequired = false;
    }
}
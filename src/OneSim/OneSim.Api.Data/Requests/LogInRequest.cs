namespace OneSim.Api.Data.Requests
{
    /// <summary>
    ///     The Log In Request.
    /// </summary>
    public class LogInRequest
    {
        /// <summary>
        /// 	Gets or sets the UserName.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///     Gets or sets the un-encrypted password.
        /// </summary>
        public string Password { get; set; }
    }
}
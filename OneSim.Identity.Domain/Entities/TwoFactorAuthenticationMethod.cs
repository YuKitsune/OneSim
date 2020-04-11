namespace OneSim.Identity.Domain.Entities
{
    /// <summary>
    ///     The Two-Factor Authentication methods.
    /// </summary>
    public enum TwoFactorAuthenticationMethod
    {
        /// <summary>
        ///     2FA method where users receive an email with a confirmation code.
        /// </summary>
        Email,

        /// <summary>
        ///     2FA method where users receive an SMS message with a confirmation code.
        /// </summary>
        Sms,

        /// <summary>
        ///     2FA method where users enter a confirmation code from their chosen authenticator app.
        /// </summary>
        Authenticator
    }
}
namespace OneSim.Identity.Application.Exceptions
{
    using System;

    /// <summary>
    ///     The Invalid User <see cref="Exception"/>.
    /// </summary>
    public class UserNotFoundException : Exception
    {
        /// <summary>
        ///     Gets the identifier of the user that could not be found.
        ///     This is not necessarily the UserId, it can be anything that directly identifies the user.
        /// </summary>
        public string UserIdentifier { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserNotFoundException"/> class.
        /// </summary>
        /// <param name="identifier">
        ///     The identifier of the user that could not be found.
        /// </param>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <param name="innerException">
        ///     The inner <see cref="Exception"/>.
        /// </param>
        public UserNotFoundException(string identifier, string message = "", Exception innerException = null) : base(message, innerException)
        {
            if (string.IsNullOrEmpty(identifier)) throw new ArgumentNullException(nameof(identifier), "The Identifier cannot be null or empty.");

            UserIdentifier = identifier;
        }
    }
}

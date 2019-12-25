namespace OneSim.Web.Models
{
    /// <summary>
    ///     The Error View Model.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        ///     Gets or sets the request ID.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        ///     Gets a value indicating whether or not to show the request ID.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
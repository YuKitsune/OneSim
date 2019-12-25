namespace OneSim.Api.Data.Responses
{
	/// <summary>
	/// 	The Response Status.
	/// </summary>
	public enum ResponseStatus
	{
		/// <summary>
		/// 	The request completed successfully.
		/// </summary>
		Success,

		/// <summary>
		/// 	The request was not successfully completed, but not due to an error.
		/// </summary>
		Failure,

		/// <summary>
		/// 	An error occurred processing the request.
		/// </summary>
		Error,

		/// <summary>
		/// 	The user is not authorized to perform the request.
		/// </summary>
		Unauthorized
	}
}
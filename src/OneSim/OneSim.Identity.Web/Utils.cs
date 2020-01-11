namespace OneSim.Identity.Web
{
	using System.Collections.Generic;
	using System.Linq;

	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.ModelBinding;
	using Microsoft.AspNetCore.Mvc.ViewFeatures;

	// Todo: Move these somewhere more useful.

	/// <summary>
	/// 	The Utilities.
	/// </summary>
	public static class Utils
	{
		/// <summary>
		/// 	Gets the name of the controller without the <see cref="Controller"/> suffix.
		/// </summary>
		/// <param name="controllerTypeName">
		///		The type name of the <see cref="Controller"/>
		/// </param>
		/// <returns>
		///		the name of the <see cref="Controller"/>.
		/// </returns>
		public static string GetControllerName(string controllerTypeName) =>
			controllerTypeName.Replace("Controller", string.Empty);

		/// <summary>
		///		Gets the <see cref="ModelError"/>s from the <see cref="ViewDataDictionary"/>.
		/// </summary>
		/// <param name="viewData">
		///		The <see cref="ViewDataDictionary"/>.
		/// </param>
		/// <returns>
		///		The <see cref="ModelError"/>.
		/// </returns>
		public static IEnumerable<ModelError> GetModelErrors(ViewDataDictionary viewData)
		{
			// Check the input
			if (viewData == null) return new List<ModelError>();

			// Go through each value in the model state
			foreach (ModelStateEntry modelState in viewData.ModelState.Values)
			{
				// Ignore if there are no errors
				if (!modelState.Errors.Any()) continue;

				// Return the errors
				return modelState.Errors;
			}

			// Made it here, no data to return
			return new List<ModelError>();
		}
	}
}
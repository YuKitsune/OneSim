using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace OneSim.Uwp.Converters
{
    /// <summary>
    ///     The <c>null</c> to <see cref="Visibility"/> converter.
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        /// <summary>
        ///     The <see cref="Visibility"/> value to return if the given value was <c>null</c>.
        /// </summary>
        public Visibility NullValue { get; set; } = Visibility.Collapsed;

        /// <summary>
        ///     The <see cref="Visibility"/> value to return if the given value was not <c>null</c>.
        /// </summary>
        public Visibility NonNullValue { get; set; } = Visibility.Visible;

        /// <summary>
        ///     Converts the given <paramref name="value"/> to a <see cref="Visibility"/>.
        /// </summary>
        /// <param name="value">
        ///     The value to convert.
        /// </param>
        /// <param name="targetType">
        ///     The target <see cref="Type"/>.
        /// </param>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <param name="language">
        ///     The language.
        /// </param>
        /// <returns>
        ///     The <see cref="Visibility"/>.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value == null) ? NullValue : NonNullValue;
        }

        /// <summary>
        ///     Performs the opposite operation to <see cref="Convert"/>.
        /// </summary>
        /// <param name="value">
        ///     The value to convert.
        /// </param>
        /// <param name="targetType">
        ///     The target <see cref="Type"/>.
        /// </param>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <param name="language">
        ///     The language.
        /// </param>
        /// <returns>
        ///     The object.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

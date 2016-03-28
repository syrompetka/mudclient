// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InverseBooleanToVisibilityConverter.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the InverseBooleanToVisibilityConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Utils
{
    using System;
    using System.Windows;
    using System.Windows.Data;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// A <see cref="IValueConverter"/> that inverts boolean value and converts it to <see cref="Visibility"/>.
    /// </summary>
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <exception cref="InvalidOperationException">The target must be a boolean</exception>
        [NotNull]
        public object Convert([NotNull] object value, [NotNull] Type targetType, [CanBeNull] object parameter, [NotNull] System.Globalization.CultureInfo culture)
        {
            Assert.ArgumentNotNull(value, "value");
            Assert.ArgumentNotNull(targetType, "targetType");
            Assert.ArgumentNotNull(culture, "culture");

            if (targetType != typeof(Visibility))
            {
                throw new InvalidOperationException("The target must be a boolean");
            }

            return !(bool)value ? Visibility.Visible : Visibility.Hidden;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <exception cref="InvalidOperationException">The target must be a boolean</exception>
        [NotNull]
        public object ConvertBack([NotNull] object value, [NotNull] Type targetType, [NotNull] object parameter, [NotNull] System.Globalization.CultureInfo culture)
        {
            Assert.ArgumentNotNull(value, "value");
            Assert.ArgumentNotNull(targetType, "targetType");
            Assert.ArgumentNotNull(parameter, "parameter");
            Assert.ArgumentNotNull(culture, "culture");

            if (targetType != typeof(bool))
            {
                throw new InvalidOperationException("The target must be a boolean");
            }

            return !(bool)value;
        }
    }
}

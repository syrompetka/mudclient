// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecondsToTimeSpanConverter.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the SecondsToTimeSpanConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Utils
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// A <see cref="IValueConverter"/> implementation to convert amount of seconds (<c>float</c>) to <see cref="TimeSpan"/>.
    /// </summary>
    public class SecondsToTimeSpanConverter : IValueConverter
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
        [NotNull]
        public object Convert([NotNull] object value, [NotNull] Type targetType, [CanBeNull] object parameter, [NotNull] CultureInfo culture)
        {
            Assert.ArgumentNotNull(value, "value");
            Assert.ArgumentNotNull(targetType, "targetType");
            Assert.ArgumentNotNull(culture, "culture");
            if (value is float)
            {
                var floatValue = (float) value;
                if (floatValue == float.PositiveInfinity)
                {
                    return TimeSpan.MaxValue;
                }

                return floatValue <= 0 ? TimeSpan.Zero : TimeSpan.FromSeconds(floatValue);
            }

            if (value is int)
            {
                var intValue = (int)value;
                return intValue <= 0 ? TimeSpan.Zero : TimeSpan.FromSeconds(intValue);
            }

            return TimeSpan.MaxValue;
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
        /// <exception cref="NotImplementedException"><c>NotImplementedException</c>.</exception>
        [CanBeNull]
        public object ConvertBack([NotNull] object value, [NotNull] Type targetType, [NotNull] object parameter, [NotNull] CultureInfo culture)
        {
            Assert.ArgumentNotNull(value, "value");
            Assert.ArgumentNotNull(targetType, "targetType");
            Assert.ArgumentNotNull(parameter, "parameter");
            Assert.ArgumentNotNull(culture, "culture");

            throw new NotImplementedException();
        }
    }
}

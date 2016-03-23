using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using AvalonDock;
using CSLib.Net.Diagnostics;

namespace Adan.Client.Utils
{
    /// <summary>
    /// A <see cref="IValueConverter"/> implementation to convert <see cref="DockableContentState"/> to <see cref="bool"/> for databinding.
    /// </summary>
    [ValueConversion(typeof(DockableContentState), typeof(bool))]
    public class DockableContentStateToBoolConverter : IValueConverter
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
        /// <exception cref="InvalidOperationException">The target must be a Brush</exception>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Assert.ArgumentNotNull(value, "value");
            Assert.ArgumentNotNull(targetType, "targetType");
            Assert.ArgumentNotNull(culture, "culture");

            if (targetType != typeof(bool))
            {
                throw new InvalidOperationException("The target must be a Visibility");
            }

            return (DockableContentState)value == DockableContentState.Hidden ? false : true;
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
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

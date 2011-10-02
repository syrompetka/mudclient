// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchConverter.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Produces an output value based upon a collection of case statements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Utils
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;
    using System.Windows.Markup;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Produces an output value based upon a collection of case statements.
    /// </summary>
    [ContentProperty("Cases")]
    public class SwitchConverter : IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchConverter"/> class.
        /// </summary>
        public SwitchConverter()
            : this(new SwitchCaseCollection())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchConverter"/> class.
        /// </summary>
        /// <param name="cases">The case collection.</param>
        internal SwitchConverter([NotNull] SwitchCaseCollection cases)
        {
            Assert.ArgumentNotNull(cases, "cases");

            Cases = cases;
            StringComparison = StringComparison.OrdinalIgnoreCase;
        }

        /// <summary>
        /// Gets a collection of switch cases that determine which output value will be produced for a given input value.
        /// </summary>
        [NotNull]
        public SwitchCaseCollection Cases
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets <see cref="StringComparison"/> to use when comparing the input value against a case.
        /// </summary>
        public StringComparison StringComparison
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an optional value that will be output if none of the cases match.
        /// </summary>
        [CanBeNull]
        public object Else
        {
            get;
            set;
        }

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
        [CanBeNull]
        public object Convert([CanBeNull]object value, [NotNull] Type targetType, [CanBeNull] object parameter, [NotNull] CultureInfo culture)
        {
            Assert.ArgumentNotNull(targetType, "targetType");
            Assert.ArgumentNotNull(culture, "culture");

            if (value == null)
            {
                // Special case for null
                // Null input can only equal null, no convert necessary
                return Cases.FirstOrDefault(x => x.When == null) ?? Else;
            }

            foreach (var c in Cases.Where(x => x.When != null))
            {
                // Special case for string to string comparison
                var valueString = value as string;
                var whenString = c.When as string;
                if (valueString != null && whenString != null)
                {
                    if (String.Equals(valueString, whenString, StringComparison))
                    {
                        return c.Then;
                    }
                }

                object when = c.When;

                // Normalize the types using IConvertible if possible
                if (TryConvert(culture, value, ref when))
                {
                    if (value.Equals(when))
                    {
                        return c.Then;
                    }
                }
            }

            return Else;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        /// <exception cref="NotSupportedException"><c>NotSupportedException</c>.</exception>
        [CanBeNull]
        public object ConvertBack([NotNull] object value, [NotNull] Type targetType, [NotNull] object parameter, [NotNull] CultureInfo culture)
        {
            Assert.ArgumentNotNull(value, "value");
            Assert.ArgumentNotNull(targetType, "targetType");
            Assert.ArgumentNotNull(parameter, "parameter");
            Assert.ArgumentNotNull(culture, "culture");

            throw new NotSupportedException();
        }

        /// <summary>
        /// Attempts to use the IConvertible interface to convert <paramref name="value2"/> into a type
        /// compatible with <paramref name="value1"/>.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="value1">The input value.</param>
        /// <param name="value2">The case value.</param>
        /// <returns>True if conversion was performed, otherwise false.</returns>
        private static bool TryConvert([NotNull] IFormatProvider culture, [NotNull] object value1, [NotNull] ref object value2)
        {
            Assert.ArgumentNotNull(culture, "culture");
            Assert.ArgumentNotNull(value1, "value1");
            Assert.ArgumentNotNull(value2, "value2");

            Type type1 = value1.GetType();
            Type type2 = value2.GetType();

            if (type1 == type2)
            {
                return true;
            }

            if (type1.IsEnum)
            {
                value2 = Enum.Parse(type1, value2.ToString(), true);
                return true;
            }

            var convertible1 = value1 as IConvertible;
            var convertible2 = value2 as IConvertible;

            if (convertible1 != null && convertible2 != null)
            {
                value2 = System.Convert.ChangeType(value2, type1, culture);
                return true;
            }

            return false;
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchCase.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   An individual case in the switch statement.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Utils
{
    using System.Windows;
    using System.Windows.Markup;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// An individual case in the switch statement.
    /// </summary>
    [ContentProperty("Then")]
    public sealed class SwitchCase : DependencyObject
    {
        /// <summary>
        /// Dependency property for the <see cref="When"/> property.
        /// </summary>
        public static readonly DependencyProperty WhenProperty = DependencyProperty.Register("When", typeof(object), typeof(SwitchCase), new PropertyMetadata(default(object)));

        /// <summary>
        /// Dependency property for the <see cref="Then"/> property.
        /// </summary>
        public static readonly DependencyProperty ThenProperty = DependencyProperty.Register("Then", typeof(object), typeof(SwitchCase), new PropertyMetadata(default(object)));

        /// <summary>
        /// Gets or sets the value to match against the input value.
        /// </summary>
        [NotNull]
        public object When
        {
            get
            {
                return GetValue(WhenProperty);
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                SetValue(WhenProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the output value to use if the current case matches.
        /// </summary>
        [NotNull]
        public object Then
        {
            get
            {
                return GetValue(ThenProperty);
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                SetValue(ThenProperty, value);
            }
        }
    }
}

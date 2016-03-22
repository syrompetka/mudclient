// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackgroundColorSelector.xaml.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Interaction logic for ColorSelector.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Controls
{
    using System.Windows;

    using Themes;

    /// <summary>
    /// Interaction logic for BackgroundColorSelector.xaml
    /// </summary>
    public partial class BackgroundColorSelector
    {
        /// <summary>
        /// Dependecy property for <see cref="SelectedColor"/>.
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register("SelectedColor", typeof(TextColor), typeof(BackgroundColorSelector));

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundColorSelector"/> class. 
        /// </summary>
        public BackgroundColorSelector()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the selected color.
        /// </summary>
        /// <value>
        /// The selected color.
        /// </value>
        public TextColor SelectedColor
        {
            get
            {
                return (TextColor)GetValue(SelectedColorProperty);
            }

            set
            {
                SetValue(SelectedColorProperty, value);
            }
        }
    }
}

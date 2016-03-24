// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoadMapDialog.xaml.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Interaction logic for RoadMapDialog.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Map.Dialogs
{
    using System.Windows;
    using System.Windows.Input;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Interaction logic for RoadMapDialog.xaml
    /// </summary>
    public partial class RoadMapDialog : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoadMapDialog"/> class.
        /// </summary>
        public RoadMapDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="UIElement.KeyDown"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="KeyEventArgs"/> that contains the event data.</param>
        protected override void OnKeyDown([NotNull] KeyEventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");

            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                Close();
            }

            base.OnKeyDown(e);
        }
    }
}

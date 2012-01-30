// --------------------------------------------------------------------------------------------------------------------
// <copyright file="YesNoDialog.xaml.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Interaction logic for YesNoDialog.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Dialogs
{
    using System.Windows;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Interaction logic for YesNoDialog.xaml
    /// </summary>
    public partial class YesNoDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YesNoDialog"/> class.
        /// </summary>
        public YesNoDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the text to display.
        /// </summary>
        /// <value>
        /// The text to display.
        /// </value>
        [CanBeNull]
        public string TextToDisplay
        {
            get
            {
                return txtTextToDisplay.Text;
            }

            set
            {
                txtTextToDisplay.Text = value;
            }
        }

        private void HandleYesClicked([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            DialogResult = true;
        }
    }
}

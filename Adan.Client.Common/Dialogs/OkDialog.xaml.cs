// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OkDialog.xaml.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Interaction logic for OkDialog.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Dialogs
{
    using CSLib.Net.Annotations;

    /// <summary>
    /// Interaction logic for OkDialog.xaml
    /// </summary>
    public partial class OkDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OkDialog"/> class.
        /// </summary>
        public OkDialog()
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
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoomEditDialog.xaml.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Interaction logic for RoomEditDialog.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Map.Dialogs
{
    using System.Windows;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Interaction logic for RoomEditDialog.xaml
    /// </summary>
    public partial class RoomEditDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoomEditDialog"/> class.
        /// </summary>
        public RoomEditDialog()
        {
            InitializeComponent();
        }

        private void HandleOkClicked([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");
            DialogResult = true;
        }
    }
}

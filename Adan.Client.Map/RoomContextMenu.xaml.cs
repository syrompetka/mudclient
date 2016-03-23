// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoomContextMenu.xaml.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Interaction logic for RoomContextMenu.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Map
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Model;

    using ViewModel;

    /// <summary>
    /// Interaction logic for RoomContextMenu.xaml
    /// </summary>
    public partial class RoomContextMenu
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoomContextMenu"/> class.
        /// </summary>
        public RoomContextMenu()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Occurs when room edit dialog is required.
        /// </summary>
        public event Action<RoomViewModel> RoomEditDialogRequired;

        /// <summary>
        /// Occurs when user wants to navigate to certain room.
        /// </summary>
        public event Action<RoomViewModel> NavigateToRoomRequired;

        /// <summary>
        /// Handles the color change.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void HandleColorChange([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            var color = (RoomColor)((MenuItem)sender).DataContext;
            var room = (RoomViewModel)DataContext;
            room.Color = color;
        }

        private void HandleIconChange([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            var icon = (RoomIcon)((MenuItem)sender).DataContext;
            var room = (RoomViewModel)DataContext;
            room.Icon = icon;
        }

        private void HandleGotoRoom([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            if (NavigateToRoomRequired != null)
            {
                NavigateToRoomRequired((RoomViewModel)DataContext);
            }
        }

        /// <summary>
        /// Handles the edit room.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void HandleEditRoom([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            if (RoomEditDialogRequired != null)
            {
                RoomEditDialogRequired((RoomViewModel)DataContext);
            }
        }
    }
}

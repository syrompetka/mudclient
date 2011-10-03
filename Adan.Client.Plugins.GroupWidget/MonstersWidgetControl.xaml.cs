// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MonstersWidgetControl.xaml.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Interaction logic for GroupWidgetControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.GroupWidget
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Input;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using ViewModel;

    /// <summary>
    /// Interaction logic for GroupWidgetControl.xaml
    /// </summary>
    public partial class MonstersWidgetControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonstersWidgetControl"/> class.
        /// </summary>
        public MonstersWidgetControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Updates the model.
        /// </summary>
        /// <param name="roomMonstersMessage">The room monsters message.</param>
        public void UpdateModel([NotNull]RoomMonstersMessage roomMonstersMessage)
        {
            Assert.ArgumentNotNull(roomMonstersMessage, "roomMonstersMessage");
            Dispatcher.BeginInvoke((Action)(() => ((RoomMonstersViewModel)DataContext).UpdateModel(roomMonstersMessage.Monsters)));
        }

        private void CancelFocusingListBoxItem([NotNull] object sender, [NotNull] MouseButtonEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");
            ((ListBoxItem)sender).IsSelected = true;
            e.Handled = true;
        }
    }
}

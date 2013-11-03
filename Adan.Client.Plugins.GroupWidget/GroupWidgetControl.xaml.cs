// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupWidgetControl.xaml.cs" company="Adamand MUD">
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
    using Adan.Client.Plugins.GroupWidget.Messages;

    /// <summary>
    /// Interaction logic for GroupWidgetControl.xaml
    /// </summary>
    public partial class GroupWidgetControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupWidgetControl"/> class.
        /// </summary>
        public GroupWidgetControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Updates the model.
        /// </summary>
        /// <param name="groupStatusMessage">The group status message.</param>
        public void UpdateModel([NotNull]GroupStatusMessage groupStatusMessage)
        {
            Assert.ArgumentNotNull(groupStatusMessage, "groupStatusMessage");
            Dispatcher.BeginInvoke((Action)(() => ((GroupStatusViewModel)DataContext).UpdateModel(groupStatusMessage.GroupMates)));
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

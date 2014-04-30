// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupsEditDialog.xaml.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Interaction logic for GroupsEditDialog.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;

namespace Adan.Client.Dialogs
{
    /// <summary>
    /// Interaction logic for GroupsEditDialog.xaml
    /// </summary>
    public partial class GroupsEditDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupsEditDialog"/> class.
        /// </summary>
        public GroupsEditDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HandleItemDoubleClick([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            ((ListBoxItem)lstGroups.SelectedItem).Content.ToString();
        }

        private void HandleCloseClick(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

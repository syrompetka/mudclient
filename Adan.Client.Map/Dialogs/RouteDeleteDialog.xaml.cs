// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RouteDeleteDialog.xaml.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Interaction logic for RouteDescrinationSelectDialog.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Map.Dialogs
{
    using System.Windows;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Interaction logic for RouteStartDialog.xaml
    /// </summary>
    public partial class RouteDeleteDialog 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RouteDeleteDialog"/> class.
        /// </summary>
        public RouteDeleteDialog()
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

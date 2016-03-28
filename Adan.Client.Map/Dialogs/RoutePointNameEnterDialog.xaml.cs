// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoutePointNameEnterDialog.xaml.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Interaction logic for RoutePointNameEnterDialog.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Map.Dialogs
{
    using System.Windows;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Interaction logic for RoutePointNameEnterDialog.xaml
    /// </summary>
    public partial class RoutePointNameEnterDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoutePointNameEnterDialog"/> class.
        /// </summary>
        public RoutePointNameEnterDialog()
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

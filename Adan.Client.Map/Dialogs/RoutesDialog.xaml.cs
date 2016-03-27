// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoutesDialog.xaml.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Interaction logic for RoutesDialog.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Map.Dialogs
{
    using System.Windows;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Interaction logic for RoutesDialog.xaml
    /// </summary>
    public partial class RoutesDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoutesDialog"/> class.
        /// </summary>
        public RoutesDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the route manager.
        /// </summary>
        [NotNull]
        public RouteManager RouteManager
        {
            get
            {
                return (RouteManager)DataContext;
            }
        }

        private void HandleCreateClicked([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            if (RouteManager.StartNewRouteRecording())
            {
                Close();
            }
        }

        private void HandleStopRecordingClicked([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            if (RouteManager.StopRouteRecording())
            {
                Close();
            }
        }

        private void HandleCancelRecordingClicked([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            RouteManager.CancelRouteRecording();
            Close();
        }

        private void HandleDeleteRouteClicked([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            if (RouteManager.DeleteRoute())
            {
                Close();
            }
        }

        private void HandleChooseRouteClicked([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            if (RouteManager.GotoDestination())
            {
                Close();
            }
        }

        private void HandleStopRouteClicked([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            RouteManager.StopRoutingToDestination();
            Close();
        }

        private void HandlePrintHelpClicked([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            RouteManager.PrintHelp();
            Close();
        }
    }
}

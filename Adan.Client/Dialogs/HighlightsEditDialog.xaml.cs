// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HighlightsEditDialog.xaml.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Interaction logic for HighlightsEditDialog.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Dialogs
{
    using System.Windows.Input;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using ViewModel;

    /// <summary>
    /// Interaction logic for HighlightsEditDialog.xaml
    /// </summary>
    public partial class HighlightsEditDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HighlightsEditDialog"/> class.
        /// </summary>
        public HighlightsEditDialog()
        {
            InitializeComponent();
        }

        private void HandleItemDoubleClick([NotNull] object sender, [NotNull] MouseButtonEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");
            var highlightsViewModel = (HighlightsViewModel)DataContext;
            if (highlightsViewModel.EditHighlightCommand.CanBeExecuted)
            {
                highlightsViewModel.EditHighlightCommand.Execute(this);
            }
        }
    }
}

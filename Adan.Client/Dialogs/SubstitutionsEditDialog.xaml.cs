// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubstitutionsEditDialog.xaml.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Interaction logic for SubstitutionsEditDialog.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Dialogs
{
    using System.Windows.Input;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using ViewModel;

    /// <summary>
    /// Interaction logic for SubstitutionsEditDialog.xaml
    /// </summary>
    public partial class SubstitutionsEditDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubstitutionsEditDialog"/> class.
        /// </summary>
        public SubstitutionsEditDialog()
        {
            InitializeComponent();
        }

        private void HandleItemDoubleClick([NotNull] object sender, [NotNull] MouseButtonEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");
            var substitutionsViewModel = (SubstitutionsViewModel)DataContext;
            if (substitutionsViewModel.EditSubstitutionCommand.CanBeExecuted)
            {
                substitutionsViewModel.EditSubstitutionCommand.Execute(this);
            }
        }
    }
}

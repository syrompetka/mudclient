// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AliasesEditDialog.xaml.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Interaction logic for AliasesEditDialog.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Dialogs
{
    using System.Windows.Input;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using ViewModel;

    /// <summary>
    /// Interaction logic for AliasesEditDialog.xaml
    /// </summary>
    public partial class AliasesEditDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AliasesEditDialog"/> class.
        /// </summary>
        public AliasesEditDialog()
        {
            InitializeComponent();
        }

        private void HandleItemDoubleClick([NotNull] object sender, [NotNull] MouseButtonEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");
            var aliasesViewModel = (AliasesViewModel)DataContext;
            if (aliasesViewModel.EditAliasCommand.CanBeExecuted)
            {
                aliasesViewModel.EditAliasCommand.Execute(this);
            }
        }
    }
}

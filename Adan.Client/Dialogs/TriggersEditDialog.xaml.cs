// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TriggersEditDialog.xaml.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Interaction logic for TriggersEditDialog.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Dialogs
{
    using System.Windows.Input;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using ViewModel;

    /// <summary>
    /// Interaction logic for TriggersEditDialog.xaml
    /// </summary>
    public partial class TriggersEditDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TriggersEditDialog"/> class.
        /// </summary>
        public TriggersEditDialog()
        {
            InitializeComponent();
        }

        private void HandleItemDoubleClick([NotNull] object sender, [NotNull] MouseButtonEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");
            var triggersModel = (TriggersViewModel)DataContext;
            if (triggersModel.EditTriggerCommand.CanBeExecuted)
            {
                triggersModel.EditTriggerCommand.Execute(this);
            }
        }
    }
}

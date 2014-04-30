// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HotkeysEditDialog.xaml.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Interaction logic for HotkeysEditDialog.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Dialogs
{
    using System.Windows.Input;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using ViewModel;

    /// <summary>
    /// Interaction logic for HotkeysEditDialog.xaml
    /// </summary>
    public partial class HotkeysEditDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HotkeysEditDialog"/> class.
        /// </summary>
        public HotkeysEditDialog()
        {
            InitializeComponent();
        }

        private void HandleItemDoubleClick([NotNull] object sender, [NotNull] MouseButtonEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");
            var hotKeysModel = (HotkeysViewModel)DataContext;
            if (hotKeysModel.EditHotkeyCommand.CanBeExecuted)
            {
                hotKeysModel.EditHotkeyCommand.Execute(this);
            }
        }

        private void HandleCloseClick(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

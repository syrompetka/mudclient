// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubstitutionEditDialog.xaml.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Interaction logic for SubstitutionEditDialog.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Dialogs
{
    using System.Windows;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using System.Windows.Input;
    /// <summary>
    /// Interaction logic for SubstitutionEditDialog.xaml
    /// </summary>
    public partial class SubstitutionEditDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubstitutionEditDialog"/> class.
        /// </summary>
        public SubstitutionEditDialog()
        {
            InitializeComponent();
            PreviewKeyUp += (s, e) => { if (e.Key == Key.Escape) Close(); };
        }

        public bool Save { get; internal set; }

        private void HandleOkClicked([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            Save = true;
            Close();
        }

        private void HandleCancelClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

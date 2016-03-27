using System;
using System.Windows;
using Adan.Client.ViewModel;
using CSLib.Net.Annotations;
using Adan.Client.Common.Settings;

namespace Adan.Client.Dialogs
{
    public partial class ProfileOptionsEditDialog
    {
        private readonly string profileName;

        /// <summary>
        /// 
        /// </summary>
        public ProfileOptionsEditDialog(string profileName)
        {
            InitializeComponent();

            this.profileName = profileName;
            Title += profileName;
            Closed += ProfileOptionsEditDialog_Closed;
        }

        private void ProfileOptionsEditDialog_Closed(object sender, EventArgs e)
        {
            SettingsHolder.Instance.SetProfile(profileName);
        }

        private void HandleItemDoubleClick([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            ((ProfileOptionsViewModel)this.DataContext).EditOptionsCommand.Execute(this);
        }

        private void HandleCloseClick([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            //DialogResult = true;
            this.Close();
        }
    }
}

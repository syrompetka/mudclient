using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Adan.Client.ViewModel;
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;

namespace Adan.Client.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для ProfilesOptionsEditDialog.xaml
    /// </summary>
    public partial class ProfileOptionsEditDialog : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public ProfileOptionsEditDialog()
        {
            InitializeComponent();
        }

        private void HandleItemDoubleClick([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            ((ProfileOptionsViewModel)this.DataContext).EditOptionsCommand.Execute(this);
        }
    }
}

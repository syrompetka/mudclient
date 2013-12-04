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
    /// Логика взаимодействия для ProfilesChooseDialog.xaml
    /// </summary>
    public partial class ProfilesChooseDialog : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public ProfilesChooseDialog()
        {
            InitializeComponent();
        }

        private void HandleItemDoubleClick([NotNull] object sender, [NotNull] MouseButtonEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            DialogResult = true;

            this.Close();
        }

        private void HandleOkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}

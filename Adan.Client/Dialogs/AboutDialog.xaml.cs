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

namespace Adan.Client.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для AboutDialog.xaml
    /// </summary>
    public partial class AboutDialog : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public AboutDialog()
        {
            InitializeComponent();
        }

        private void HandleOk(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

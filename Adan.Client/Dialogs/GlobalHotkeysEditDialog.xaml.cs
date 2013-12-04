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
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;
using Adan.Client.ViewModel;

namespace Adan.Client.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для GlobalHokeys.xaml
    /// </summary>
    public partial class GlobalHotkeysEditDialog : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public GlobalHotkeysEditDialog()
        {
            InitializeComponent();
        }

        private void HandleItemDoubleClick([NotNull] object sender, [NotNull] MouseButtonEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            var hotKeysModel = (GlobalHotkeysViewModel)DataContext;
            
            hotKeysModel.EditHotkeyCommand.Execute(this);
        }

        private void HandleOkClick(object sender, RoutedEventArgs e)
        {
            ((GlobalHotkeysViewModel)DataContext).EditHotkeyCommand.Execute(this);
        }
    }
}

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
using CSLib.Net.Diagnostics;
using CSLib.Net.Annotations;

namespace Adan.Client.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для GlobalHotkeyEditDialog.xaml
    /// </summary>
    public partial class GlobalHotkeyEditDialog : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public GlobalHotkeyEditDialog()
        {
            InitializeComponent();
        }

        private void HandleOkClicked([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            DialogResult = true;
        }

        private void HandleKeyOnTextBox([NotNull] object sender, [NotNull] KeyEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            var hotkeyViewModel = (GlobalHotkeyViewModel)DataContext;

            hotkeyViewModel.SetHotkey(e.Key == Key.System ? e.SystemKey : e.Key, Keyboard.Modifiers);

            e.Handled = true;
        }
    }
}

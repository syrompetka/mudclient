namespace Adan.Client.Dialogs
{
    using System.Windows;

    public partial class OptionsDialog
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public OptionsDialog()
        {
            InitializeComponent();
        }

        private void HandleOk(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

    }
}

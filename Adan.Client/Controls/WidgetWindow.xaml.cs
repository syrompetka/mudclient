namespace Adan.Client.Controls
{
    using System.ComponentModel;

    /// <summary>
    /// Interaction logic for WidgetWindow.xaml
    /// </summary>
    public partial class WidgetWindow
    {
        public WidgetWindow()
        {
            InitializeComponent();
        }
        private void CancelWindowClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}

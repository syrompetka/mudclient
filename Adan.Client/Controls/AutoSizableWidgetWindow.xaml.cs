
namespace Adan.Client.Controls
{
    using System.ComponentModel;

    /// <summary>
    /// Interaction logic for AutoSizableWidgetWindow.xaml
    /// </summary>
    public partial class AutoSizableWidgetWindow
    {
        public AutoSizableWidgetWindow()
        {
            InitializeComponent();
        }

        private void CancelWindowClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}

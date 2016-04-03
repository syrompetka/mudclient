namespace Adan.Client.Plugins.OutputWindow
{
    using System;
    using System.Windows;
    using Common.Controls;
    using ViewModel;

    /// <summary>
    /// Interaction logic for AdditionalOutputWindow.xaml
    /// </summary>
    public partial class AdditionalOutputWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdditionalOutputWindow"/> class.
        /// </summary>
        public AdditionalOutputWindow(AdditionalOutputWindowsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
        
        private void HandleFlowTextControlInitialized(object sender, EventArgs e)
        {
            var viewModel = (OutputWindowViewModel) ((ScrollableFlowTextControl) sender).DataContext;
            viewModel.TextPresenter = (ScrollableFlowTextControl) sender;
        }
    }
}

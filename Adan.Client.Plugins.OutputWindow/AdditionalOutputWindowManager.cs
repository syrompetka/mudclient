
namespace Adan.Client.Plugins.OutputWindow
{
    using System;
    using System.Linq;
    using System.Windows;
    using Common.Model;
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Messages;
    using ViewModel;

    public class AdditionalOutputWindowManager
    {
        private readonly AdditionalOutputWindowsViewModel _viewModel;
        
        public AdditionalOutputWindowManager(AdditionalOutputWindowsViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public void OutputWindowCreated([NotNull] RootModel rootModel)
        {
            Assert.ArgumentNotNull(rootModel, "rootModel");

            _viewModel.Windows.Add(new OutputWindowViewModel(rootModel) { IsActive = false });
        }

        public void OutputWindowClosed([NotNull] RootModel rootModel)
        {
            Assert.ArgumentNotNull(rootModel, "rootModel");
            var windowToRemove = _viewModel.Windows.FirstOrDefault(w => w.RootModel == rootModel);
            if (windowToRemove != null)
            {
                _viewModel.Windows.Remove(windowToRemove);
            }
        }

        public void OutputWindowChanged([NotNull] RootModel rootModel)
        {
            Assert.ArgumentNotNull(rootModel, "rootModel");

            foreach (var outputWindowViewModel in _viewModel.Windows)
            {
                outputWindowViewModel.IsActive = outputWindowViewModel.RootModel == rootModel;
            }
        }

        public void AddText(RootModel rootModel, OutputToAdditionalWindowMessage message)
        {
            var window = _viewModel.Windows.FirstOrDefault(w => w.RootModel == rootModel);
            if (window != null && window.TextPresenter != null)
            {
                Application.Current.Dispatcher.BeginInvoke((Action)(() => window.TextPresenter.AddMessages(Enumerable.Repeat(message, 1))));


            }
        }
    }
}
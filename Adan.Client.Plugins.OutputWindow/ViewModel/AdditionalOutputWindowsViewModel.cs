namespace Adan.Client.Plugins.OutputWindow.ViewModel
{
    using System.Collections.ObjectModel;
    using Common.Controls;
    using Common.Model;
    using Common.ViewModel;
    public sealed class AdditionalOutputWindowsViewModel : ViewModelBase
    {
        public AdditionalOutputWindowsViewModel()
        {
            Windows = new ObservableCollection<OutputWindowViewModel>();
        }

        public ObservableCollection<OutputWindowViewModel> Windows
        {
            get; private set;
        }
    }

    public sealed class OutputWindowViewModel : ViewModelBase
    {
        private bool _isActive;

        public OutputWindowViewModel(RootModel rootModel)
        {
            RootModel = rootModel;
        }

        public ScrollableFlowTextControl TextPresenter
        {
            get;
            set;
        }

        public RootModel RootModel
        {
            get; private set;
        }

        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnPropertyChanged("IsActive");
                }
            }
        }
    }
}

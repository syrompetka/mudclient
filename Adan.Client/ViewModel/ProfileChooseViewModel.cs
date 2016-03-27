using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Adan.Client.Common.ViewModel;
using CSLib.Net.Annotations;

namespace Adan.Client.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class ProfileChooseViewModel : ViewModelBase
    {
        private ProfileViewModel _selectedProfile;
        private ObservableCollection<ProfileViewModel> _profiles;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="profiles">List of profiles</param>
        /// <param name="selectedProfile">Current selected profile</param>
        public ProfileChooseViewModel(ObservableCollection<ProfileViewModel> profiles, string selectedProfile)
        {
            _profiles = new ObservableCollection<ProfileViewModel>(profiles);
            _selectedProfile = _profiles.FirstOrDefault(prf => prf.NameProfile == selectedProfile);
        }

        /// <summary>
        /// Get Selected profile.
        /// </summary>
        [CanBeNull]
        public ProfileViewModel SelectedProfile
        {
            get
            {
                return _selectedProfile;
            }
            set
            {
                _selectedProfile = value;
                OnPropertyChanged("SelectedProfile");
            }
        }

        /// <summary>
        /// Get All Profiles
        /// </summary>
        [NotNull]
        public ObservableCollection<ProfileViewModel> Profiles
        {
            get
            {
                return _profiles;
            }
            private set
            {
                _profiles = value;
            }
        }
    }
}

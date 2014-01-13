using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Serialization;
using Adan.Client.Common;
using Adan.Client.Common.Model;
using Adan.Client.Common.Settings;
using Adan.Client.Common.Utils;
using Adan.Client.Common.ViewModel;
using Adan.Client.Dialogs;
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;

namespace Adan.Client.ViewModel
{
    /// <summary>
    /// Profile View Model
    /// </summary>
    public class ProfilesEditViewModel : ViewModelBase
    {
        private ObservableCollection<ProfileViewModel> _profiles;
        private ProfileViewModel _selectedProfile;
        private string _newProfileName;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="profiles">List of profiles</param>
        /// <param name="selectedProfile">Current selected profile</param>
        public ProfilesEditViewModel(ObservableCollection<ProfileViewModel> profiles, string selectedProfile)
        {
            _profiles = new ObservableCollection<ProfileViewModel>(profiles);
            _selectedProfile = _profiles.FirstOrDefault(prf => prf.NameProfile == selectedProfile);

            AddProfileCommand = new DelegateCommand(AddProfile, false);
            DeleteProfileCommand = new DelegateCommand(DeleteProfile, false);
            EditProfileCommand = new DelegateCommand(EditProfile, true);
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
                DeleteProfileCommand.CanBeExecuted = (value != null && value.NameProfile != "Default");
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

        /// <summary>
        /// Gets or sets the name for group to create.
        /// </summary>
        /// <value>
        /// The new name of the group.
        /// </value>
        [CanBeNull]
        public string NewProfileName
        {
            get
            {
                return _newProfileName;
            }

            set
            {
                _newProfileName = value;

                UpdateAddProfileCommandCanExecute();
                OnPropertyChanged("NewGroupName");
            }
        }

        /// <summary>
        /// Add Profile
        /// </summary>
        [NotNull]
        public DelegateCommand AddProfileCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Delete Profile
        /// </summary>
        [NotNull]
        public DelegateCommand DeleteProfileCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        [NotNull]
        public DelegateCommand EditProfileCommand
        {
            get;
            private set;
        }

        private void AddProfile([NotNull] object obj)
        {
            Assert.ArgumentNotNull(obj, "obj");

            var castedProfileName = obj as string;
            if (!String.IsNullOrEmpty(castedProfileName))
            {
                Profiles.Add(new ProfileViewModel(castedProfileName, false));
                SettingsHolder.Instance.CreateNewProfile(castedProfileName);
            }

            UpdateAddProfileCommandCanExecute();
        }

        private void DeleteProfile([NotNull] object obj)
        {
            Assert.ArgumentNotNull(obj, "obj");

            var profileView = obj as ProfileViewModel;

            if (profileView == null)
                return;

            if (profileView.NameProfile == "Default")
                return;

            if (Profiles.Contains(profileView))
            {
                Profiles.Remove(profileView);
                SettingsHolder.Instance.DeleteProfile(profileView.NameProfile);
            }

            UpdateAddProfileCommandCanExecute();
        }

        private void EditProfile([NotNull] object obj)
        {
            Assert.ArgumentNotNull(obj, "obj");

            var owner = obj as Window;

            var profile = SettingsHolder.Instance.GetProfile(SelectedProfile.NameProfile);

            var profileOptionDialog = new ProfileOptionsEditDialog()
            {
                DataContext = new ProfileOptionsViewModel(profile),
                Owner = owner,
            };

            var result = profileOptionDialog.ShowDialog();
            SettingsHolder.Instance.SetProfile(profile, true);
        }

        private void UpdateAddProfileCommandCanExecute()
        {
            if (!string.IsNullOrEmpty(_newProfileName) && !Profiles.Any(prof => prof.NameProfile == _newProfileName))
            {
                AddProfileCommand.CanBeExecuted = true;
            }
            else
            {
                AddProfileCommand.CanBeExecuted = false;
            }
        }
    }
}

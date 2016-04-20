using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Adan.Client.Common.Model;
using Adan.Client.Common.Settings;
using Adan.Client.Common.Utils;
using Adan.Client.Common.ViewModel;
using Adan.Client.Dialogs;
using Adan.Client.Model;
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;
using Microsoft.Win32;

namespace Adan.Client.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class ProfileOptionsViewModel : ViewModelBase
    {
        private GroupsViewModel _groupsViewModel;
        private ListBoxItem _selectedOption;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="profile"></param>
        public ProfileOptionsViewModel(ProfileHolder profile)
        {
            _groupsViewModel = new GroupsViewModel(profile.Groups, profile.Name, RootModel.AllActionDescriptions);
            Profile = profile;

            EditOptionsCommand = new DelegateCommand(EditProfile, true);
            ImportProfileCommand = new DelegateCommand(ImportProfile, true);
        }

        /// <summary>
        /// This constructor is supposed for creating global profile.
        /// Global profile cannot be imported for now
        /// </summary>
        /// <param name="name">Name of the profile (e.g. "Global")</param>
        /// <param name="groups">List of profile groups</param>
        public ProfileOptionsViewModel(string name, List<Group> groups)
        {
            _groupsViewModel = new GroupsViewModel(groups, name, RootModel.AllActionDescriptions);
            Profile = null;

            EditOptionsCommand = new DelegateCommand(EditProfile, true);
            ImportProfileCommand = new DelegateCommand(ImportProfile, true);
        }


        /// <summary>
        /// 
        /// </summary>
        public ProfileHolder Profile
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public ListBoxItem SelectedOption
        {
            get
            {
                return _selectedOption;
            }
            set
            {
                if (_selectedOption != value)
                {
                    _selectedOption = value;
                    OnPropertyChanged("SelectedOption");
                    OnPropertyChanged("CanEditProfile");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [NotNull]
        public DelegateCommand EditOptionsCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        [NotNull]
        public DelegateCommand ImportProfileCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Amount of aliases in the profile
        /// </summary>
        public int AliasesCount { get { return _groupsViewModel.Groups.Sum(g => g.Aliases.Count); } }
        /// <summary>
        /// Amount of hotkeys in the profile
        /// </summary>
        public int HotkeysCount { get { return _groupsViewModel.Groups.Sum(g => g.Hotkeys.Count); } }
        /// <summary>
        /// Amount of groups in the profile
        /// </summary>
        public int GroupsCount { get { return _groupsViewModel.AllGroup.Count; } }
        /// <summary>
        /// Amount of highlights in the profile
        /// </summary>
        public int HighlightsCount { get { return _groupsViewModel.Groups.Sum(g => g.Highlights.Count); } }
        /// <summary>
        /// Amount of triggers in the profile
        /// </summary>
        public int TriggersCount { get { return _groupsViewModel.Groups.Sum(g => g.Triggers.Count); } }
        /// <summary>
        /// Amount of triggers in the profile
        /// </summary>
        public int SubstitutionsCount { get { return _groupsViewModel.Groups.Sum(g => g.Substitutions.Count); } }

        /// <summary>
        /// Can profile be imported or not
        /// </summary>
        public bool CanImportProfile { get { return Profile != null;  } }
        public bool CanEditProfile { get { return SelectedOption != null; } }
        private void EditProfile([NotNull] object obj)
        {
            Assert.ArgumentNotNull(obj, "obj");

            var owner = obj as Window;
            if (owner == null)
                return;

            var name = SelectedOption.Tag.ToString();
            switch (name)
            {
                case "Aliases":
                    var aliasesEditDialog = new AliasesEditDialog
                    {
                        DataContext = new AliasesViewModel(_groupsViewModel.Groups, RootModel.AllActionDescriptions),
                        Owner = owner
                    };
                    aliasesEditDialog.Closed += (s, e) =>
                    {
                        OnPropertyChanged("AliasesCount");
                        SettingsHolder.Instance.SetProfile(_groupsViewModel.Name);
                    };
                    aliasesEditDialog.Show();
                    break;
                case "Groups":
                    var groupEditDialog = new GroupsEditDialog
                    {
                        DataContext = _groupsViewModel,
                        Owner = owner
                    };
                    groupEditDialog.Closed += (s, e) =>
                    {
                        OnPropertyChanged("GroupsCount");
                        SettingsHolder.Instance.GetProfile(_groupsViewModel.Name).Groups = _groupsViewModel.AllGroup;
                        SettingsHolder.Instance.SetProfile(_groupsViewModel.Name);
                    };
                    groupEditDialog.Show();
                    break;
                case "Highlights":
                    var highlightsEditDialog = new HighlightsEditDialog
                    {
                        DataContext = new HighlightsViewModel(_groupsViewModel.Groups),
                        Owner = owner
                    };
                    highlightsEditDialog.Closed += (s, e) =>
                    {
                        OnPropertyChanged("HighlightsCount");
                        SettingsHolder.Instance.SetProfile(_groupsViewModel.Name);
                    };
                    highlightsEditDialog.Show();
                    break;
                case "Hotkeys":
                    var hotKeysEditDialog = new HotkeysEditDialog
                    {
                        DataContext = new HotkeysViewModel(_groupsViewModel.Groups, RootModel.AllActionDescriptions),
                        Owner = owner
                    };
                    hotKeysEditDialog.Closed += (s, e) =>
                    {
                        OnPropertyChanged("HotkeysCount");
                        SettingsHolder.Instance.SetProfile(_groupsViewModel.Name);
                    };
                    hotKeysEditDialog.Show();
                    break;
                case "Substitutions":
                    var substitutionsEditDialog = new SubstitutionsEditDialog
                    {
                        DataContext = new SubstitutionsViewModel(_groupsViewModel.Groups),
                        Owner = owner
                    };
                    substitutionsEditDialog.Closed += (s, e) =>
                    {
                        OnPropertyChanged("SubstitutionsCount");
                        SettingsHolder.Instance.SetProfile(_groupsViewModel.Name);
                    };
                    substitutionsEditDialog.Show();
                    break;
                case "Triggers":
                    var triggerEditDialog = new TriggersEditDialog
                    {
                        DataContext = new TriggersViewModel(_groupsViewModel.Groups, RootModel.AllActionDescriptions),
                        Owner = owner
                    };
                    triggerEditDialog.Closed += (s, e) =>
                    {
                        OnPropertyChanged("TriggersCount");
                        SettingsHolder.Instance.SetProfile(_groupsViewModel.Name);
                    };
                    triggerEditDialog.Show();
                    break;
            }
        }

        private void ImportProfile(object obj)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.DefaultExt = ".set";
            fileDialog.Filter = "Config|*.set|All Files|*.*";
            fileDialog.Multiselect = false;

            var result = fileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                if (!File.Exists(fileDialog.FileName))
                    return;

                RootModel rootModel = new RootModel(Profile);
                var conveyor = ConveyorFactory.CreateNew(rootModel);
                try
                {
                    using (var stream = new StreamReader(fileDialog.FileName, Encoding.Default, false, 1024))
                    {
                        string line;
                        while ((line = stream.ReadLine()) != null)
                        {
                            //XML не читает символ \x01B
                            //TODO: Need FIX IT
                            if (!line.Contains("\x001B"))
                            {
                                //rootModel.PushCommandToConveyor(new TextCommand(line)).ImportJMC(line, rootModel);
                                conveyor.ImportJMC(line, rootModel);
                            }
                        }
                    }
                }
                catch
                {
                }

                _groupsViewModel = new GroupsViewModel(Profile.Groups, Profile.Name, RootModel.AllActionDescriptions);
                OnPropertyChanged("AliasesCount");
                OnPropertyChanged("GroupsCount");
                OnPropertyChanged("HighlightsCount");
                OnPropertyChanged("HotkeysCount");
                OnPropertyChanged("SubstitutionsCount");
                OnPropertyChanged("TriggersCount");
            }
        }
    }
}
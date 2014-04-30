using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Adan.Client.Common.Model;
using Adan.Client.Common.Settings;
using Adan.Client.Common.Utils;
using Adan.Client.Common.ViewModel;
using Adan.Client.Dialogs;
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

        private void EditProfile([NotNull] object obj)
        {
            Assert.ArgumentNotNull(obj, "obj");

            var owner = obj as Window;
            if (obj == null)
                return;

            var name = SelectedOption.Content.ToString();
            switch (name)
            {
                case "Aliases":
                    var aliasesEditDialog = new AliasesEditDialog
                    {
                        DataContext = new AliasesViewModel(_groupsViewModel.Groups, RootModel.AllActionDescriptions),
                        Owner = owner
                    };
                    aliasesEditDialog.Show();
                    break;
                case "Groups":
                    var groupEditDialog = new GroupsEditDialog
                    {
                        DataContext = new GroupsViewModel(_groupsViewModel.AllGroup, "Default", RootModel.AllActionDescriptions),
                        Owner = owner
                    };
                    groupEditDialog.Show();
                    break;
                case "Highlights":
                    var highlightsEditDialog = new HighlightsEditDialog
                    {
                        DataContext = new HighlightsViewModel(_groupsViewModel.Groups),
                        Owner = owner
                    };
                    highlightsEditDialog.Show();
                    break;
                case "Hotkeys":
                    var hotKeysEditDialog = new HotkeysEditDialog
                    {
                        DataContext = new HotkeysViewModel(_groupsViewModel.Groups, RootModel.AllActionDescriptions),
                        Owner = owner
                    };
                    hotKeysEditDialog.Show();
                    break;
                case "Substitutions":
                    var substitutionsEditDialog = new SubstitutionsEditDialog
                    {
                        DataContext = new SubstitutionsViewModel(_groupsViewModel.Groups),
                        Owner = owner
                    };
                    substitutionsEditDialog.Show();
                    break;
                case "Triggers":
                    var triggerEditDialog = new TriggersEditDialog
                    {
                        DataContext = new TriggersViewModel(_groupsViewModel.Groups, RootModel.AllActionDescriptions),
                        Owner = owner
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
                ProfileHolder.ImportJmcConfig(fileDialog.FileName, Profile);
                _groupsViewModel = new GroupsViewModel(Profile.Groups, Profile.Name, RootModel.AllActionDescriptions);
            }
        }
    }
}
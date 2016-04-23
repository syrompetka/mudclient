// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupsViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the GroupsViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ViewModel
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Common.Model;
    using Common.Plugins;
    using Common.Utils;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// View model for groups.
    /// </summary>
    public class GroupsViewModel : ViewModelBase
    {
        private readonly List<Group> _allGroups;
        private readonly IEnumerable<ActionDescription> _actionDescriptions;
        private GroupViewModel _selectedGroup;
        private string _newGroupName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allGroups"></param>
        /// <param name="name"></param>
        /// <param name="actionDescriptions"></param>
        public GroupsViewModel([NotNull] List<Group> allGroups, string name, [NotNull] IEnumerable<ActionDescription> actionDescriptions)
        {
            Assert.ArgumentNotNull(allGroups, "allGroups");
            Assert.ArgumentNotNull(actionDescriptions, "actionDescriptions");

            _allGroups = allGroups;
            Name = name;
            _actionDescriptions = actionDescriptions;
            DeleteGroupCommand = new DelegateCommand(DeleteGroup, false);
            AddGroupCommand = new DelegateCommand(AddGroup, false);
            Groups = new ObservableCollection<GroupViewModel>();
            foreach (var group in _allGroups)
            {
                Groups.Add(new GroupViewModel(Groups, group, _actionDescriptions));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<Group> AllGroup
        {
            get
            {
                return _allGroups;
            }
        }

        /// <summary>
        /// Gets the groups.
        /// </summary>
        public ObservableCollection<GroupViewModel> Groups
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the delete group command.
        /// </summary>
        [NotNull]
        public DelegateCommand DeleteGroupCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the add group command.
        /// </summary>
        [NotNull]
        public DelegateCommand AddGroupCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the selected group.
        /// </summary>
        /// <value>
        /// The selected group.
        /// </value>
        [CanBeNull]
        public GroupViewModel SelectedGroup
        {
            get
            {
                return _selectedGroup;
            }

            set
            {
                _selectedGroup = value;
                if (value == null)
                {
                    DeleteGroupCommand.CanBeExecuted = false;
                    return;
                }

                DeleteGroupCommand.CanBeExecuted = !value.IsBuildIn;
            }
        }

        /// <summary>
        /// Gets or sets the name for group to create.
        /// </summary>
        /// <value>
        /// The new name of the group.
        /// </value>
        [CanBeNull]
        public string NewGroupName
        {
            get
            {
                return _newGroupName;
            }

            set
            {
                _newGroupName = value;
                UpdateAddGroupCommandCanExecute();

                OnPropertyChanged("NewGroupName");
            }
        }

        private void UpdateAddGroupCommandCanExecute()
        {
            if (!string.IsNullOrEmpty(_newGroupName) && !Groups.Any(group => group.Name == _newGroupName))
            {
                AddGroupCommand.CanBeExecuted = true;
            }
            else
            {
                AddGroupCommand.CanBeExecuted = false;
            }
        }

        private void DeleteGroup([NotNull] object group)
        {
            Assert.ArgumentNotNull(group, "group");

            var castedGroup = group as GroupViewModel;
            if (castedGroup != null)
            {
                var gr = castedGroup.Group;
                _allGroups.Remove(gr);
                Groups.Remove(castedGroup);
            }

            UpdateAddGroupCommandCanExecute();
        }

        private void AddGroup([NotNull] object groupName)
        {
            Assert.ArgumentNotNull(groupName, "groupName");
            var castedGroupName = groupName as string;
            if (!string.IsNullOrEmpty(castedGroupName))
            {
                var group = new GroupViewModel(Groups, new Group(), _actionDescriptions) { Name = castedGroupName, IsEnabled = true };
                _allGroups.Add(group.Group);
                Groups.Add(group);
                NewGroupName = "";
            }

            UpdateAddGroupCommandCanExecute();
        }
    }
}

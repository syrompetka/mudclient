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
        private readonly IList<Group> _allGroups;
        private readonly IEnumerable<ActionDescription> _actionDescriptions;
        private readonly ObservableCollection<GroupViewModel> _groups;
        private GroupViewModel _selectedGroup;
        private string _newGroupName;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupsViewModel"/> class.
        /// </summary>
        /// <param name="allGroups">All groups.</param>
        /// <param name="actionDescriptions">The action descriptions.</param>
        public GroupsViewModel([NotNull] IList<Group> allGroups, [NotNull] IEnumerable<ActionDescription> actionDescriptions)
        {
            Assert.ArgumentNotNull(allGroups, "allGroups");
            Assert.ArgumentNotNull(actionDescriptions, "actionDescriptions");

            _allGroups = allGroups;
            _actionDescriptions = actionDescriptions;
            DeleteGroupCommand = new DelegateCommand(DeleteGroup, false);
            AddGroupCommand = new DelegateCommand(AddGroup, false);
            _groups = new ObservableCollection<GroupViewModel>();
            foreach (var group in _allGroups)
            {
                _groups.Add(new GroupViewModel(_groups, group, _actionDescriptions));
            }

            Groups = new ReadOnlyObservableCollection<GroupViewModel>(_groups);
        }

        /// <summary>
        /// Gets the groups.
        /// </summary>
        public ReadOnlyObservableCollection<GroupViewModel> Groups
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
                _allGroups.Remove(castedGroup.Group);
                _groups.Remove(castedGroup);
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
                _groups.Add(group);
            }

            UpdateAddGroupCommandCanExecute();
        }
    }
}

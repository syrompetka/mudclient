// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HotkeysViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the HotkeysViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ViewModel
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    using Common.Model;
    using Common.Plugins;
    using Common.Utils;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Dialogs;

    using Model.Actions;

    /// <summary>
    /// View model for hot keys editor.
    /// </summary>
    public class HotkeysViewModel : ViewModelBase
    {
        #region Constants and Fields

        private readonly IEnumerable<GroupViewModel> _allGroups;
        private readonly IEnumerable<ActionDescription> _actionDescriptions;
        private HotkeyViewModel _selectedHotkey;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HotkeysViewModel"/> class.
        /// </summary>
        /// <param name="allGroups">All groups.</param>
        /// <param name="actionDescriptions">The action descriptions.</param>
        public HotkeysViewModel([NotNull] IEnumerable<GroupViewModel> allGroups, [NotNull] IEnumerable<ActionDescription> actionDescriptions) 
        {
            Assert.ArgumentNotNull(allGroups, "allGroups");
            Assert.ArgumentNotNull(actionDescriptions, "actionDescriptions");

            _allGroups = allGroups;
            _actionDescriptions = actionDescriptions;
            AddHotkeyCommand = new DelegateCommand(AddHotkeyCommandExecute, true);
            EditHotkeyCommand = new DelegateCommand(EditHotkeyCommandExecute, false);
            DeleteHotkeyCommand = new DelegateCommand(DeleteHotkeyCommandExecute, false);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the add hot key command.
        /// </summary>
        [NotNull]
        public DelegateCommand AddHotkeyCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the delete hot key command.
        /// </summary>
        [NotNull]
        public DelegateCommand DeleteHotkeyCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the edit hot key command.
        /// </summary>
        [NotNull]
        public DelegateCommand EditHotkeyCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the groups.
        /// </summary>
        [NotNull]
        public IEnumerable<GroupViewModel> Groups
        {
            get
            {
                return _allGroups;
            }
        }

        /// <summary>
        /// Gets or sets the selected hot key.
        /// </summary>
        /// <value>
        /// The selected hot key.
        /// </value>
        [CanBeNull]
        public HotkeyViewModel SelectedHotkey
        {
            get
            {
                return _selectedHotkey;
            }

            set
            {
                _selectedHotkey = value;
                EditHotkeyCommand.CanBeExecuted = value != null;
                DeleteHotkeyCommand.CanBeExecuted = value != null;
                OnPropertyChanged("SelectedHotkey");
            }
        }

        #endregion

        #region Methods

        private void AddHotkeyCommandExecute([NotNull] object obj)
        {
            Assert.ArgumentNotNull(obj, "obj");

            var hotkey = new Hotkey();
            hotkey.Actions.Add(new SendTextAction());
            var hotKeyToAdd = new HotkeyViewModel(Groups, Groups.First(g => g.IsBuildIn), hotkey, _actionDescriptions);
            var hotKeyEditDialog = new HotkeyEditDialog { DataContext = hotKeyToAdd, Owner = (Window)obj };
            var dialogResult = hotKeyEditDialog.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                hotKeyToAdd.HotkeyGroup.AddHotkey(hotKeyToAdd);
            }
        }

        private void DeleteHotkeyCommandExecute([CanBeNull] object obj)
        {
            if (SelectedHotkey == null)
            {
                return;
            }

            foreach (var groupViewModel in Groups)
            {
                if (!groupViewModel.Hotkeys.Contains(SelectedHotkey))
                {
                    continue;
                }

                groupViewModel.RemoveHotkey(SelectedHotkey);
                break;
            }
        }

        private void EditHotkeyCommandExecute([NotNull] object obj)
        {
            Assert.ArgumentNotNull(obj, "obj");

            if (SelectedHotkey == null)
            {
                return;
            }

            var originalHotKey = SelectedHotkey;
            var hotKeyEditDialog = new HotkeyEditDialog { DataContext = originalHotKey.Clone(), Owner = (Window)obj };
            var dialogResult = hotKeyEditDialog.ShowDialog();
            if (!dialogResult.HasValue || !dialogResult.Value)
            {
                return;
            }

            var changedHotKey = (HotkeyViewModel)hotKeyEditDialog.DataContext;
            if (originalHotKey.HotkeyGroup == changedHotKey.HotkeyGroup)
            {
                var originalIndex = originalHotKey.HotkeyGroup.Hotkeys.IndexOf(originalHotKey);
                originalHotKey.HotkeyGroup.InsertHotkey(originalIndex, (HotkeyViewModel)hotKeyEditDialog.DataContext);
            }
            else
            {
                changedHotKey.HotkeyGroup.AddHotkey(changedHotKey);
            }

            originalHotKey.HotkeyGroup.RemoveHotkey(originalHotKey);
            SelectedHotkey = (HotkeyViewModel)hotKeyEditDialog.DataContext;
        }

        #endregion
    }
}

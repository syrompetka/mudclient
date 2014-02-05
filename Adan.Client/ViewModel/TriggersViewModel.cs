// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TriggersViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the TriggersViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ViewModel
{
    #region Namespace Imports

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

    #endregion

    /// <summary>
    /// View model for triggers editor.
    /// </summary>
    public class TriggersViewModel : ViewModelBase
    {
        #region Constants and Fields

        private readonly IEnumerable<GroupViewModel> _allGroups;
        private readonly IEnumerable<ActionDescription> _actionDescriptions;
        private TriggerViewModel _selectedTrigger;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggersViewModel"/> class.
        /// </summary>
        /// <param name="allGroups">All groups.</param>
        /// <param name="actionDescriptions">The action descriptions.</param>
        public TriggersViewModel([NotNull] IEnumerable<GroupViewModel> allGroups, [NotNull] IEnumerable<ActionDescription> actionDescriptions)
        {
            Assert.ArgumentNotNull(allGroups, "allGroups");
            Assert.ArgumentNotNull(actionDescriptions, "actionDescriptions");

            _allGroups = allGroups;
            _actionDescriptions = actionDescriptions;
            AddTriggerCommand = new DelegateCommand(AddTriggerCommandExecute);
            DeleteTriggerCommand = new DelegateCommand(DeleteTriggerCommandExecute, false);
            EditTriggerCommand = new DelegateCommand(EditTriggerCommandExecute, false);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the add trigger command.
        /// </summary>
        [NotNull]
        public DelegateCommand AddTriggerCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the delete trigger command.
        /// </summary>
        [NotNull]
        public DelegateCommand DeleteTriggerCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the edit trigger command.
        /// </summary>
        [NotNull]
        public DelegateCommand EditTriggerCommand
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
        /// Gets or sets the selected trigger.
        /// </summary>
        /// <value>
        /// The selected trigger.
        /// </value>
        [CanBeNull]
        public TriggerViewModel SelectedTrigger
        {
            get
            {
                return _selectedTrigger;
            }

            set
            {
                _selectedTrigger = value;
                EditTriggerCommand.CanBeExecuted = value != null;
                DeleteTriggerCommand.CanBeExecuted = value != null;

                OnPropertyChanged("SelectedTrigger");
            }
        }

        #endregion

        #region Methods

        private void AddTriggerCommandExecute([NotNull] object obj)
        {
            Assert.ArgumentNotNull(obj, "obj");
            var textTrigger = new TextTrigger() { IsRegExp = false };
            textTrigger.Actions.Add(new SendTextAction());
            var triggerToAdd = new TriggerViewModel(Groups, Groups.First(g => g.IsBuildIn), textTrigger, _actionDescriptions);
            var trigerEditDialog = new TriggerEditDialog { DataContext = triggerToAdd, Owner = (Window)obj };
            var dialogResult = trigerEditDialog.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                triggerToAdd.TriggersGroup.AddTrigger(triggerToAdd);
            }
        }

        private void DeleteTriggerCommandExecute([CanBeNull] object obj)
        {
            if (SelectedTrigger == null)
            {
                return;
            }

            foreach (var groupViewModel in Groups)
            {
                if (!groupViewModel.Triggers.Contains(SelectedTrigger))
                {
                    continue;
                }

                groupViewModel.RemoveTrigger(SelectedTrigger);
                break;
            }
        }

        private void EditTriggerCommandExecute([NotNull] object obj)
        {
            Assert.ArgumentNotNull(obj, "obj");

            if (SelectedTrigger == null)
            {
                return;
            }

            var originalTrigger = SelectedTrigger;
            var trigerEditDialog = new TriggerEditDialog { DataContext = originalTrigger.Clone(), Owner = (Window)obj };
            var dialogResult = trigerEditDialog.ShowDialog();
            if (!dialogResult.HasValue || !dialogResult.Value)
            {
                return;
            }

            var changedTrigger = (TriggerViewModel)trigerEditDialog.DataContext;
            if (originalTrigger.TriggersGroup == changedTrigger.TriggersGroup)
            {
                var originalIndex = originalTrigger.TriggersGroup.Triggers.IndexOf(originalTrigger);
                originalTrigger.TriggersGroup.InsertTrigger(
                    originalIndex, (TriggerViewModel)trigerEditDialog.DataContext);
            }
            else
            {
                changedTrigger.TriggersGroup.AddTrigger(changedTrigger);
            }

            originalTrigger.TriggersGroup.RemoveTrigger(originalTrigger);
            SelectedTrigger = (TriggerViewModel)trigerEditDialog.DataContext;
        }

        #endregion
    }
}
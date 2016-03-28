// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AliasesViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the AliasesViewModel type.
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
    /// View model for aliases edit dialog.
    /// </summary>
    public class AliasesViewModel : ViewModelBase
    {
        #region Constants and Fields

        private readonly IEnumerable<GroupViewModel> _allGroups;
        private readonly IEnumerable<ActionDescription> _actionDescriptions;
        private AliasViewModel _selectedAlias;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AliasesViewModel"/> class.
        /// </summary>
        /// <param name="allGroups">All groups.</param>
        /// <param name="actionDescriptions">The action descriptions.</param>
        public AliasesViewModel([NotNull] IEnumerable<GroupViewModel> allGroups, [NotNull] IEnumerable<ActionDescription> actionDescriptions)
        {
            Assert.ArgumentNotNull(allGroups, "allGroups");
            Assert.ArgumentNotNull(actionDescriptions, "actionDescriptions");

            _allGroups = allGroups;
            _actionDescriptions = actionDescriptions;
            AddAliasCommand = new DelegateCommand(AddAliasCommandExecute, true);
            EditAliasCommand = new DelegateCommand(EditAliasCommandExecute, false);
            DeleteAliasCommand = new DelegateCommand(DeleteAliasCommandExecute, false);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the add alias command.
        /// </summary>
        [NotNull]
        public DelegateCommand AddAliasCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the delete alias command.
        /// </summary>
        [NotNull]
        public DelegateCommand DeleteAliasCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the edit alias command.
        /// </summary>
        [NotNull]
        public DelegateCommand EditAliasCommand
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
        /// Gets or sets the selected alias.
        /// </summary>
        /// <value>
        /// The selected alias.
        /// </value>
        [CanBeNull]
        public AliasViewModel SelectedAlias
        {
            get
            {
                return _selectedAlias;
            }

            set
            {
                _selectedAlias = value;
                EditAliasCommand.CanBeExecuted = value != null;
                DeleteAliasCommand.CanBeExecuted = value != null;
                OnPropertyChanged("SelectedAlias");
            }
        }

        #endregion

        #region Methods

        private void AddAliasCommandExecute([NotNull] object obj)
        {
            Assert.ArgumentNotNull(obj, "obj");

            var commandAlias = new CommandAlias();
            commandAlias.Actions.Add(new SendTextAction());
            var aliasToAdd = new AliasViewModel(Groups, Groups.First(g => g.IsBuildIn), commandAlias, _actionDescriptions);
            var aliasEditDialog = new AliasEditDialog { DataContext = aliasToAdd, Owner = (Window)obj };
            aliasEditDialog.Show();
            aliasEditDialog.Closed += (s, e) =>
            {
                if (aliasEditDialog.Save)
                    aliasToAdd.AliasGroup.AddAlias(aliasToAdd);
            };
        }

        private void DeleteAliasCommandExecute([CanBeNull] object obj)
        {
            if (SelectedAlias == null)
            {
                return;
            }

            foreach (var groupViewModel in Groups)
            {
                if (!groupViewModel.Aliases.Contains(SelectedAlias))
                {
                    continue;
                }

                groupViewModel.RemoveAlias(SelectedAlias);
                break;
            }
        }

        private void EditAliasCommandExecute([NotNull] object obj)
        {
            Assert.ArgumentNotNull(obj, "obj");

            if (SelectedAlias == null)
            {
                return;
            }

            var originalAlias = SelectedAlias;
            var aliasEditDialog = new AliasEditDialog { DataContext = originalAlias.Clone(), Owner = (Window)obj };
            aliasEditDialog.Show();
            aliasEditDialog.Closed += (s, e) =>
            {
                if (!aliasEditDialog.Save)
                    return;

                var changedAlias = (AliasViewModel)aliasEditDialog.DataContext;
                if (originalAlias.AliasGroup == changedAlias.AliasGroup)
                {
                    var originalIndex = originalAlias.AliasGroup.Aliases.IndexOf(originalAlias);
                    originalAlias.AliasGroup.InsertAlias(originalIndex, (AliasViewModel)aliasEditDialog.DataContext);
                }
                else
                {
                    changedAlias.AliasGroup.AddAlias(changedAlias);
                }

                originalAlias.AliasGroup.RemoveAlias(originalAlias);
                SelectedAlias = (AliasViewModel)aliasEditDialog.DataContext;
            };
        }

        #endregion
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubstitutionsViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the SubstitutionsViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ViewModel
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    using Common.Model;
    using Common.Utils;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Dialogs;

    /// <summary>
    /// View model for substitutuions editor.
    /// </summary>
    public class SubstitutionsViewModel : ViewModelBase
    {
        #region Constants and Fields

        private readonly IEnumerable<GroupViewModel> _allGroups;
        private SubstitutionViewModel _selectedSubstitution;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubstitutionsViewModel"/> class.
        /// </summary>
        /// <param name="allGroups">All groups.</param>
        public SubstitutionsViewModel([NotNull] IEnumerable<GroupViewModel> allGroups)
        {
            Assert.ArgumentNotNull(allGroups, "allGroups");

            _allGroups = allGroups;
            AddSubstitutionCommand = new DelegateCommand(AddSubstitutionCommandExecute, true);
            EditSubstitutionCommand = new DelegateCommand(EditSubstitutionCommandExecute, false);
            DeleteSubstitutionCommand = new DelegateCommand(DeleteSubstitutionCommandExecute, false);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the add substitution command.
        /// </summary>
        [NotNull]
        public DelegateCommand AddSubstitutionCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the delete substitution command.
        /// </summary>
        [NotNull]
        public DelegateCommand DeleteSubstitutionCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the edit substitution command.
        /// </summary>
        [NotNull]
        public DelegateCommand EditSubstitutionCommand
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
        /// Gets or sets the selected substitution.
        /// </summary>
        /// <value>
        /// The selected substitution.
        /// </value>
        [CanBeNull]
        public SubstitutionViewModel SelectedSubstitution
        {
            get
            {
                return _selectedSubstitution;
            }

            set
            {
                _selectedSubstitution = value;
                EditSubstitutionCommand.CanBeExecuted = value != null;
                DeleteSubstitutionCommand.CanBeExecuted = value != null;
                OnPropertyChanged("SelectedSubstitution");
            }
        }

        #endregion

        #region Methods

        private void AddSubstitutionCommandExecute([NotNull] object obj)
        {
            Assert.ArgumentNotNull(obj, "obj");

            var substitutionToAdd = new SubstitutionViewModel(Groups, Groups.First(g => g.IsBuildIn), new Substitution());
            var substitutionEditDialog = new SubstitutionEditDialog { DataContext = substitutionToAdd, Owner = (Window)obj };
            substitutionEditDialog.Show();
            substitutionEditDialog.Closed += (s, e) =>
            {
                if (substitutionEditDialog.Save)
                    substitutionToAdd.SubstitutionGroup.AddSubstitution(substitutionToAdd);
            };
        }

        private void DeleteSubstitutionCommandExecute([CanBeNull] object obj)
        {
            if (SelectedSubstitution == null)
            {
                return;
            }

            foreach (var groupViewModel in Groups)
            {
                if (!groupViewModel.Substitutions.Contains(SelectedSubstitution))
                {
                    continue;
                }

                groupViewModel.RemoveSubstitution(SelectedSubstitution);
                break;
            }
        }

        private void EditSubstitutionCommandExecute([NotNull] object obj)
        {
            Assert.ArgumentNotNull(obj, "obj");

            if (SelectedSubstitution == null)
            {
                return;
            }

            var originalSubstitution = SelectedSubstitution;
            var substitutionEditDialog = new SubstitutionEditDialog { DataContext = originalSubstitution.Clone(), Owner = (Window)obj };
            substitutionEditDialog.Show();
            substitutionEditDialog.Closed += (object sender, System.EventArgs e) =>
            {
                if (!substitutionEditDialog.Save)
                    return;

                var changedSubstitution = (SubstitutionViewModel)substitutionEditDialog.DataContext;
                if (originalSubstitution.SubstitutionGroup == changedSubstitution.SubstitutionGroup)
                {
                    var originalIndex = originalSubstitution.SubstitutionGroup.Substitutions.IndexOf(originalSubstitution);
                    originalSubstitution.SubstitutionGroup.InsertSubstitution(originalIndex, (SubstitutionViewModel)substitutionEditDialog.DataContext);
                }
                else
                {
                    changedSubstitution.SubstitutionGroup.AddSubstitution(changedSubstitution);
                }

                originalSubstitution.SubstitutionGroup.RemoveSubstitution(originalSubstitution);
                SelectedSubstitution = (SubstitutionViewModel)substitutionEditDialog.DataContext;

            };
        }

        #endregion
    }
}

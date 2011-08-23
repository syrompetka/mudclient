// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HighlightsViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the HighlightsViewModel type.
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
    /// View model for highlights editor.
    /// </summary>
    public class HighlightsViewModel : ViewModelBase
    {
        #region Constants and Fields

        private readonly IEnumerable<GroupViewModel> _allGroups;
        private HighlightViewModel _selectedHighlight;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HighlightsViewModel"/> class.
        /// </summary>
        /// <param name="allGroups">All groups.</param>
        public HighlightsViewModel([NotNull] IEnumerable<GroupViewModel> allGroups)
        {
            Assert.ArgumentNotNull(allGroups, "allGroups");

            _allGroups = allGroups;
            AddHighlightCommand = new DelegateCommand(AddHighlightCommandExecute, true);
            EditHighlightCommand = new DelegateCommand(EditHighlightCommandExecute, false);
            DeleteHighlightCommand = new DelegateCommand(DeleteHighlightCommandExecute, false);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the add highlight command.
        /// </summary>
        [NotNull]
        public DelegateCommand AddHighlightCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the delete highlight command.
        /// </summary>
        [NotNull]
        public DelegateCommand DeleteHighlightCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the edit highlight command.
        /// </summary>
        [NotNull]
        public DelegateCommand EditHighlightCommand
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
        /// Gets or sets the selected highlight.
        /// </summary>
        /// <value>
        /// The selected highlight.
        /// </value>
        [CanBeNull]
        public HighlightViewModel SelectedHighlight
        {
            get
            {
                return _selectedHighlight;
            }

            set
            {
                _selectedHighlight = value;
                EditHighlightCommand.CanBeExecuted = value != null;
                DeleteHighlightCommand.CanBeExecuted = value != null;
                OnPropertyChanged("SelectedHighlight");
            }
        }

        #endregion

        #region Methods

        private void AddHighlightCommandExecute([NotNull] object obj)
        {
            Assert.ArgumentNotNull(obj, "obj");

            var highlightToAdd = new HighlightViewModel(Groups, Groups.First(g => g.IsBuildIn), new Highlight());
            var highlightEditDialog = new HighlightEditDialog { DataContext = highlightToAdd, Owner = (Window)obj };
            var dialogResult = highlightEditDialog.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                highlightToAdd.HighlightGroup.AddHighlight(highlightToAdd);
            }
        }

        private void DeleteHighlightCommandExecute([CanBeNull] object obj)
        {
            if (SelectedHighlight == null)
            {
                return;
            }

            foreach (var groupViewModel in Groups)
            {
                if (!groupViewModel.Highlights.Contains(SelectedHighlight))
                {
                    continue;
                }

                groupViewModel.RemoveHighlight(SelectedHighlight);
                break;
            }
        }

        private void EditHighlightCommandExecute([NotNull] object obj)
        {
            Assert.ArgumentNotNull(obj, "obj");

            if (SelectedHighlight == null)
            {
                return;
            }

            var originalHighlight = SelectedHighlight;
            var highlightEditDialog = new HighlightEditDialog { DataContext = originalHighlight.Clone(), Owner = (Window)obj };
            var dialogResult = highlightEditDialog.ShowDialog();
            if (!dialogResult.HasValue || !dialogResult.Value)
            {
                return;
            }

            var changedHighlight = (HighlightViewModel)highlightEditDialog.DataContext;
            if (originalHighlight.HighlightGroup == changedHighlight.HighlightGroup)
            {
                var originalIndex = originalHighlight.HighlightGroup.Highlights.IndexOf(originalHighlight);
                originalHighlight.HighlightGroup.InsertHighlight(originalIndex, (HighlightViewModel)highlightEditDialog.DataContext);
            }
            else
            {
                changedHighlight.HighlightGroup.AddHighlight(changedHighlight);
            }

            originalHighlight.HighlightGroup.RemoveHighlight(originalHighlight);
            SelectedHighlight = (HighlightViewModel)highlightEditDialog.DataContext;
        }

        #endregion
    }
}

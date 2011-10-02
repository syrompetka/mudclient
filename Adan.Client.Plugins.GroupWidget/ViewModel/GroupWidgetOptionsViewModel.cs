// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupWidgetOptionsViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the GroupWidgetOptionsViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.GroupWidget.ViewModel
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Common.Utils;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// A view model for group widget options dialog
    /// </summary>
    public class GroupWidgetOptionsViewModel : ViewModelBase
    {
        private AffectDescription _selectedAvailableAffect;
        private bool _canAddDisplayedAffect;
        private AffectDescription _selectedDisplayedAffect;
        private bool _canRemoveDisplayedAffect;
        private bool _canMoveDisplayedAffectUp;
        private bool _canMoveDisplayedAffectDown;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupWidgetOptionsViewModel"/> class.
        /// </summary>
        /// <param name="windowTitle">The window title.</param>
        /// <param name="availableAffects">The avalable affects.</param>
        /// <param name="displayedAffects">The displayed affects.</param>
        public GroupWidgetOptionsViewModel([NotNull] string windowTitle, [NotNull] IEnumerable<AffectDescription> availableAffects, [NotNull] IEnumerable<AffectDescription> displayedAffects)
        {
            Assert.ArgumentNotNullOrWhiteSpace(windowTitle, "windowTitle");
            Assert.ArgumentNotNull(availableAffects, "availableAffects");
            Assert.ArgumentNotNull(displayedAffects, "displayedAffects");

            WindowTitle = windowTitle;
            AvailableAffects = new ObservableCollection<AffectDescription>(availableAffects);
            DisplayedAffects = new ObservableCollection<AffectDescription>(displayedAffects);
            AddDisplayedAffectCommand = new DelegateCommandWithCanExecute(AddDisplayedAffectCommandExecute, AddDisplayedAffectCommandCanExecute);
            RemoveDisplayedAffectCommand = new DelegateCommandWithCanExecute(RemoveDisplayedAffectCommandExecute, RemoveDisplayedAffectCommandCanExecute);
            MoveDisplayedAffectUpCommand = new DelegateCommandWithCanExecute(MoveDisplayedAffectUpCommandExecute, MoveDisplayedAffectUpCommandCanExecute);
            MoveDisplayedAffectDownCommand = new DelegateCommandWithCanExecute(MoveDisplayedAffectDownCommandExecute, MoveDisplayedAffectDownCommandCanExecute);
        }

        /// <summary>
        /// Gets the window title.
        /// </summary>
        [NotNull]
        public string WindowTitle
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the available affects.
        /// </summary>
        [NotNull]
        public ObservableCollection<AffectDescription> AvailableAffects
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the displayed affects.
        /// </summary>
        [NotNull]
        public ObservableCollection<AffectDescription> DisplayedAffects
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the selected available affect.
        /// </summary>
        /// <value>
        /// The selected available affect.
        /// </value>
        [CanBeNull]
        public AffectDescription SelectedAvailableAffect
        {
            get
            {
                return _selectedAvailableAffect;
            }

            set
            {
                _selectedAvailableAffect = value;
                UpdateCommandsCanExecute();
                OnPropertyChanged("SelectedAvailableAffect");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether user can add displayed affect.
        /// </summary>
        /// <value>
        /// <c>true</c> if user can add displayed affect; otherwise, <c>false</c>.
        /// </value>
        public bool CanAddDisplayedAffect
        {
            get
            {
                return _canAddDisplayedAffect;
            }

            set
            {
                _canAddDisplayedAffect = value;
                OnPropertyChanged("CanAddDisplayedAffect");
            }
        }

        /// <summary>
        /// Gets or sets the selected displayed affect.
        /// </summary>
        /// <value>
        /// The selected displayed affect.
        /// </value>
        [CanBeNull]
        public AffectDescription SelectedDisplayedAffect
        {
            get
            {
                return _selectedDisplayedAffect;
            }

            set
            {
                _selectedDisplayedAffect = value;
                OnPropertyChanged("SelectedDisplayedAffect");
                CanRemoveDisplayedAffect = value != null;
                UpdateCommandsCanExecute();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether user can remove displayed affect.
        /// </summary>
        /// <value>
        /// <c>true</c> if user can remove displayed affect; otherwise, <c>false</c>.
        /// </value>
        public bool CanRemoveDisplayedAffect
        {
            get
            {
                return _canRemoveDisplayedAffect;
            }

            set
            {
                _canRemoveDisplayedAffect = value;
                OnPropertyChanged("CanRemoveDisplayedAffect");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether user can move displayed affect up.
        /// </summary>
        /// <value>
        /// <c>true</c> if user can move displayed affect up; otherwise, <c>false</c>.
        /// </value>
        public bool CanMoveDisplayedAffectUp
        {
            get
            {
                return _canMoveDisplayedAffectUp;
            }

            set
            {
                _canMoveDisplayedAffectUp = value;
                OnPropertyChanged("CanMoveDisplayedAffectUp");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether user can move displayed affect down.
        /// </summary>
        /// <value>
        /// <c>true</c> if user can move displayed affect down; otherwise, <c>false</c>.
        /// </value>
        public bool CanMoveDisplayedAffectDown
        {
            get
            {
                return _canMoveDisplayedAffectDown;
            }

            set
            {
                _canMoveDisplayedAffectDown = value;
                OnPropertyChanged("CanMoveDisplayedAffectDown");
            }
        }

        /// <summary>
        /// Gets the move displayed affect up command.
        /// </summary>
        [NotNull]
        public DelegateCommandWithCanExecute MoveDisplayedAffectUpCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the move displayed affect down command.
        /// </summary>
        [NotNull]
        public DelegateCommandWithCanExecute MoveDisplayedAffectDownCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the add displayed affect command.
        /// </summary>
        [NotNull]
        public DelegateCommandWithCanExecute AddDisplayedAffectCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the remove displayed affect command.
        /// </summary>
        [NotNull]
        public DelegateCommandWithCanExecute RemoveDisplayedAffectCommand
        {
            get;
            private set;
        }

        private void UpdateCommandsCanExecute()
        {
            AddDisplayedAffectCommand.UpdateCanExecute();
            RemoveDisplayedAffectCommand.UpdateCanExecute();
            MoveDisplayedAffectDownCommand.UpdateCanExecute();
            MoveDisplayedAffectUpCommand.UpdateCanExecute();
        }

        private void MoveDisplayedAffectDownCommandExecute([CanBeNull]object obj)
        {
            if (SelectedDisplayedAffect != null)
            {
                var originalIndex = DisplayedAffects.IndexOf(SelectedDisplayedAffect);
                DisplayedAffects.Move(originalIndex, originalIndex + 1);
            }

            UpdateCommandsCanExecute();
        }

        private bool MoveDisplayedAffectDownCommandCanExecute([CanBeNull]object arg)
        {
            if (SelectedDisplayedAffect != null)
            {
                if (DisplayedAffects.IndexOf(SelectedDisplayedAffect) < DisplayedAffects.Count - 1)
                {
                    return true;
                }
            }

            return false;
        }

        private bool MoveDisplayedAffectUpCommandCanExecute([CanBeNull]object arg)
        {
            if (SelectedDisplayedAffect != null)
            {
                if (DisplayedAffects.IndexOf(SelectedDisplayedAffect) > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void MoveDisplayedAffectUpCommandExecute([CanBeNull]object obj)
        {
            if (SelectedDisplayedAffect != null)
            {
                var originalIndex = DisplayedAffects.IndexOf(SelectedDisplayedAffect);
                DisplayedAffects.Move(originalIndex, originalIndex - 1);
            }

            UpdateCommandsCanExecute();
        }

        private bool RemoveDisplayedAffectCommandCanExecute([CanBeNull]object arg)
        {
            return SelectedDisplayedAffect != null;
        }

        private void RemoveDisplayedAffectCommandExecute([CanBeNull]object obj)
        {
            if (SelectedDisplayedAffect != null)
            {
                AvailableAffects.Add(SelectedDisplayedAffect);
                DisplayedAffects.Remove(SelectedDisplayedAffect);
            }

            UpdateCommandsCanExecute();
        }

        private void AddDisplayedAffectCommandExecute([CanBeNull]object obj)
        {
            if (SelectedAvailableAffect != null)
            {
                DisplayedAffects.Add(SelectedAvailableAffect);
                AvailableAffects.Remove(SelectedAvailableAffect);
            }

            UpdateCommandsCanExecute();
        }

        private bool AddDisplayedAffectCommandCanExecute([CanBeNull]object arg)
        {
            return SelectedAvailableAffect != null;
        }
    }
}

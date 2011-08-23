// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionsViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   View model for action editor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ViewModel.Actions
{
    #region Namespace Imports

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Input;

    using Common.Model;
    using Common.Plugins;
    using Common.Utils;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    #endregion

    /// <summary>
    /// View model for action editor.
    /// </summary>
    public class ActionsViewModel : ViewModelBase
    {
        #region Constants and Fields

        private readonly ObservableCollection<ActionViewModelBase> _actions;
        private readonly IList<ActionBase> _originalActions;
        private readonly IEnumerable<ActionDescription> _actionDescriptions;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionsViewModel"/> class.
        /// </summary>
        /// <param name="actions">The actions.</param>
        /// <param name="actionDescriptions">The action descriptions.</param>
        public ActionsViewModel([NotNull] IList<ActionBase> actions, [NotNull] IEnumerable<ActionDescription> actionDescriptions)
        {
            Assert.ArgumentNotNull(actions, "actions");
            Assert.ArgumentNotNull(actionDescriptions, "actionDescriptions");

            _originalActions = actions;
            _actionDescriptions = actionDescriptions;
            _actions = new ObservableCollection<ActionViewModelBase>();
            Actions = new ReadOnlyObservableCollection<ActionViewModelBase>(_actions);
            AddActionCommand = new DelegateCommand(AddActionCommandExecute, true);
            RemoveActionCommand = new DelegateCommandWithCanExecute(RemoveActionCommandExecute, RemoveActionCommandCanExecute);
            MoveActionUpCommand = new DelegateCommandWithCanExecute(MoveActionUpCommandExecute, MoveActionUpCommandCanExecute);
            MoveActionDownCommand = new DelegateCommandWithCanExecute(MoveActionDownCommandExecute, MoveActionDownCommandCanExecute);

            foreach (var originalAction in _originalActions)
            {
                var newAction = CreateActionViewModel(originalAction, actionDescriptions);
                newAction.PropertyChanged += HandleActionDescriptionChange;
                newAction.ActionTypeChanged += ChangeActionType;
                _actions.Add(newAction);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the actions description.
        /// </summary>
        [NotNull]
        public string ActionsDescription
        {
            get
            {
                var res = string.Empty;
                bool firstAction = true;
                foreach (var action in Actions)
                {
                    if (!firstAction)
                    {
                        res += "; ";
                    }

                    firstAction = false;

                    res += action.ActionDescription;
                }

                return res;
            }
        }

        /// <summary>
        /// Gets the actions.
        /// </summary>
        [NotNull]
        public ReadOnlyObservableCollection<ActionViewModelBase> Actions
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the add action command.
        /// </summary>
        [NotNull]
        public ICommand AddActionCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the move action down command.
        /// </summary>
        public DelegateCommandWithCanExecute MoveActionDownCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the move action up command.
        /// </summary>
        public DelegateCommandWithCanExecute MoveActionUpCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the remove action command.
        /// </summary>
        [NotNull]
        public DelegateCommandWithCanExecute RemoveActionCommand
        {
            get;
            private set;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the action.
        /// </summary>
        /// <param name="actionType">Type of the action.</param>
        public void AddAction([NotNull] ActionDescription actionType)
        {
            Assert.ArgumentNotNull(actionType, "actionType");

            AddAction(actionType, 0);
        }

        /// <summary>
        /// Adds the action.
        /// </summary>
        /// <param name="actionType">Type of the action.</param>
        /// <param name="position">The position where to put new action.</param>
        public void AddAction([NotNull] ActionDescription actionType, int position)
        {
            Assert.ArgumentNotNull(actionType, "actionType");

            var newAction = CreateNewActionFromType(actionType);
            newAction.PropertyChanged += HandleActionDescriptionChange;
            newAction.ActionTypeChanged += ChangeActionType;
            _actions.Insert(position, newAction);
            _originalActions.Insert(position, newAction.Action);
            OnPropertyChanged("ActionsDescription");
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <param name="actions">The actions.</param>
        /// <returns>
        /// A deep copy of this instance.
        /// </returns>
        [NotNull]
        public ActionsViewModel Clone([NotNull] IList<ActionBase> actions)
        {
            Assert.ArgumentNotNull(actions, "actions");

            var res = new ActionsViewModel(actions, _actionDescriptions);
            foreach (var action in Actions)
            {
                res.AddAction(action.Clone());
            }

            return res;
        }

        #endregion

        #region Methods

        [NotNull]
        private static ActionViewModelBase CreateNewActionFromType([NotNull] ActionDescription actionDescription)
        {
            Assert.ArgumentNotNull(actionDescription, "actionDescription");
            var result = actionDescription.CreateActionViewModel(actionDescription.CreateAction());
            Assert.IsNotNull(result, "Specified ActionDescription implementation is not correct.");
            return result;
        }

        /// <summary>
        /// Creates the action view model.
        /// </summary>
        /// <param name="originalAction">The original action.</param>
        /// <param name="actionDescriptions">The action descriptions.</param>
        /// <returns>
        /// A newly created action view model.
        /// </returns>
        /// <exception cref="NotSupportedException"><c>NotSupportedException</c>.</exception>
        [NotNull]
        private static ActionViewModelBase CreateActionViewModel([NotNull] ActionBase originalAction, [NotNull] IEnumerable<ActionDescription> actionDescriptions)
        {
            Assert.ArgumentNotNull(originalAction, "originalAction");
            Assert.ArgumentNotNull(actionDescriptions, "actionDescriptions");

            foreach (var actionDescription in actionDescriptions)
            {
                var res = actionDescription.CreateActionViewModel(originalAction);
                if (res != null)
                {
                    return res;
                }
            }

            throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Action of type '{0}' is not supported.", originalAction.GetType().Name));
        }

        private void AddAction([NotNull]ActionViewModelBase actionToAdd)
        {
            Assert.ArgumentNotNull(actionToAdd, "actionToAdd");

            actionToAdd.PropertyChanged += HandleActionDescriptionChange;
            actionToAdd.ActionTypeChanged += ChangeActionType;
            _actions.Add(actionToAdd);
            _originalActions.Add(actionToAdd.Action);
        }

        private void ChangeActionType([NotNull] ActionViewModelBase action, [NotNull] ActionDescription newactiontype)
        {
            Assert.ArgumentNotNull(action, "action");
            Assert.ArgumentNotNull(newactiontype, "newactiontype");

            var currentIndex = Actions.IndexOf(action);
            RemoveAction(action);
            AddAction(newactiontype, currentIndex);
        }

        private void RemoveAction([NotNull] ActionViewModelBase actionToRemove)
        {
            Assert.ArgumentNotNull(actionToRemove, "actionToRemove");

            actionToRemove.ActionTypeChanged -= ChangeActionType;
            actionToRemove.PropertyChanged -= HandleActionDescriptionChange;
            _originalActions.Remove(actionToRemove.Action);
            _actions.Remove(actionToRemove);
            OnPropertyChanged("ActionsDescription");
        }

        private void HandleActionDescriptionChange([NotNull] object sender, [NotNull] PropertyChangedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            if (e.PropertyName == "ActionDescription")
            {
                OnPropertyChanged("ActionsDescription");
            }
        }

        #endregion

        #region Command methods

        private void AddActionCommandExecute([CanBeNull] object obj)
        {
            var castedAction = obj as ActionViewModelBase;
            if (castedAction != null)
            {
                var originalActionIndex = Actions.IndexOf(castedAction);
                AddAction(_actionDescriptions.First(), originalActionIndex + 1);
            }

            UpdateCommandsCanExectue();
        }

        private bool MoveActionDownCommandCanExecute([CanBeNull] object arg)
        {
            var castedAction = arg as ActionViewModelBase;
            if (castedAction != null)
            {
                var originalIndex = Actions.IndexOf(castedAction);
                return originalIndex < Actions.Count - 1;
            }

            return false;
        }

        private bool MoveActionUpCommandCanExecute([CanBeNull] object arg)
        {
            var castedAction = arg as ActionViewModelBase;
            if (castedAction != null)
            {
                var originalIndex = Actions.IndexOf(castedAction);
                return originalIndex > 0;
            }

            return false;
        }

        private bool RemoveActionCommandCanExecute([CanBeNull] object arg)
        {
            return Actions.Count > 1;
        }

        private void RemoveActionCommandExecute([CanBeNull] object obj)
        {
            var castedAction = obj as ActionViewModelBase;
            if (castedAction != null)
            {
                RemoveAction(castedAction);
            }

            UpdateCommandsCanExectue();
        }

        private void MoveActionDownCommandExecute([CanBeNull] object obj)
        {
            var castedAction = obj as ActionViewModelBase;
            if (castedAction != null)
            {
                var originalIndex = Actions.IndexOf(castedAction);
                var tempAction = _originalActions[originalIndex];
                _originalActions[originalIndex] = _originalActions[originalIndex + 1];
                _originalActions[originalIndex + 1] = tempAction;
                _actions.Move(originalIndex, originalIndex + 1);
            }

            UpdateCommandsCanExectue();
            OnPropertyChanged("ActionsDescription");
        }

        private void MoveActionUpCommandExecute([CanBeNull] object obj)
        {
            var castedAction = obj as ActionViewModelBase;
            if (castedAction != null)
            {
                var originalIndex = Actions.IndexOf(castedAction);
                var tempAction = _originalActions[originalIndex];
                _originalActions[originalIndex] = _originalActions[originalIndex - 1];
                _originalActions[originalIndex - 1] = tempAction;
                _actions.Move(originalIndex, originalIndex - 1);
            }

            UpdateCommandsCanExectue();
            OnPropertyChanged("ActionsDescription");
        }

        private void UpdateCommandsCanExectue()
        {
            RemoveActionCommand.UpdateCanExecute();
            MoveActionUpCommand.UpdateCanExecute();
            MoveActionDownCommand.UpdateCanExecute();
        }

        #endregion
    }
}
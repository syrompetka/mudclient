// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionParametersViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ActionParamtersViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.ViewModel
{
    #region Namespace Imports

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Model;

    using Plugins;

    using Utils;

    #endregion

    /// <summary>
    /// View model for parameters editor.
    /// </summary>
    public class ActionParametersViewModel : ViewModelBase
    {
        #region Constants and Fields

        private readonly ObservableCollection<ActionParameterViewModelBase> _parameters = new ObservableCollection<ActionParameterViewModelBase>();
        private readonly IList<ActionParameterBase> _originalParameters;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionParametersViewModel"/> class.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="parameterDescriptions">The parameter descriptions.</param>
        public ActionParametersViewModel([NotNull]IList<ActionParameterBase> parameters, [NotNull] IEnumerable<ParameterDescription> parameterDescriptions)
        {
            Assert.ArgumentNotNull(parameters, "parameters");
            Assert.ArgumentNotNull(parameterDescriptions, "parameterDescriptions");

            _originalParameters = parameters;
            ParameterDescriptions = parameterDescriptions;

            foreach (var originalParameter in _originalParameters)
            {
                var parameter = CreateParameterViewModel(originalParameter, ParameterDescriptions);
                parameter.ParameterTypeChanged += ChangeParameterType;
                parameter.PropertyChanged += HandleParameterDescriptionChanged;
                _parameters.Add(parameter);
            }

            Parameters = new ReadOnlyObservableCollection<ActionParameterViewModelBase>(_parameters);
            AddParameterCommand = new DelegateCommand(AddParameterCommandExecute);
            DeleteParameterCommand = new DelegateCommand(DeleteParameterCommandExecute);
            MoveParameterUpCommand = new DelegateCommandWithCanExecute(MoveParameterUpCommandExecute, MoveParameterUpCommandCanExecute);
            MoveParameterDownCommand = new DelegateCommandWithCanExecute(MoveParameterDownCommandExecute, MoveParameterDownCommandCanExecute);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the parameter descriptions.
        /// </summary>
        [NotNull]
        public IEnumerable<ParameterDescription> ParameterDescriptions
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the add parameter command.
        /// </summary>
        [NotNull]
        public DelegateCommand AddParameterCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the delete parameter command.
        /// </summary>
        [NotNull]
        public DelegateCommand DeleteParameterCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the move parameter up command.
        /// </summary>
        [NotNull]
        public DelegateCommandWithCanExecute MoveParameterUpCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the move parameter down command.
        /// </summary>
        [NotNull]
        public DelegateCommandWithCanExecute MoveParameterDownCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        [NotNull]
        public ReadOnlyObservableCollection<ActionParameterViewModelBase> Parameters
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the actions description.
        /// </summary>
        [NotNull]
        public string ActionParametersDescription
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var parameter in Parameters)
                {
                    sb.Append(" ");
                    sb.Append(parameter.ParameterDescription);
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets a value indicating whether parameters collection is empty or not.
        /// </summary>
        /// <value>
        /// <c>true</c> if parameters collection is empty; otherwise, <c>false</c>.
        /// </value>
        public bool ParametersCollectionEmpty
        {
            get
            {
                return Parameters.Count == 0;
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Creates the new type of the action parameter from.
        /// </summary>
        /// <param name="parameterDescription">The parameter description.</param>
        /// <returns>
        /// Created parameter view model.
        /// </returns>
        /// <exception cref="InvalidOperationException">If specified <paramref name="parameterDescription"/> has incorrect implementation.</exception>
        [NotNull]
        public static ActionParameterViewModelBase CreateNewActionParameterFromType([NotNull] ParameterDescription parameterDescription)
        {
            Assert.ArgumentNotNull(parameterDescription, "parameterDescription");
            var result = parameterDescription.CreateParameterViewModel(parameterDescription.CreateParameter());
            if (result != null)
            {
                return result;
            }

            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Parameter description {0} has incorrect implementation.", parameterDescription));
        }

        /// <summary>
        /// Creates the parameter view model.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterDescriptions">The parameter descriptions.</param>
        /// <returns>
        /// A parameter view model that corresponds to given <paramref name="parameter"/>.
        /// </returns>
        /// <exception cref="NotSupportedException"><c>NotSupportedException</c>.</exception>
        [NotNull]
        public static ActionParameterViewModelBase CreateParameterViewModel([NotNull] ActionParameterBase parameter, [NotNull] IEnumerable<ParameterDescription> parameterDescriptions)
        {
            Assert.ArgumentNotNull(parameter, "parameter");
            Assert.ArgumentNotNull(parameterDescriptions, "parameterDescriptions");

            foreach (var parameterDescription in parameterDescriptions)
            {
                var result = parameterDescription.CreateParameterViewModel(parameter);
                if (result != null)
                {
                    return result;
                }
            }

            throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Parameter of type '{0}' is not supported.", parameter.GetType().Name));
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        /// A deep copy of this instance.
        /// </returns>
        [NotNull]
        public ActionParametersViewModel Clone([NotNull]IList<ActionParameterBase> parameters)
        {
            Assert.ArgumentNotNull(parameters, "parameters");

            var res = new ActionParametersViewModel(parameters, ParameterDescriptions);
            foreach (var actionParameterViewModelBase in Parameters)
            {
                res.AddParameter(actionParameterViewModelBase.Clone());
            }

            return res;
        }

        private void AddParameter([NotNull]ActionParameterViewModelBase parameterToAdd)
        {
            Assert.ArgumentNotNull(parameterToAdd, "parameterToAdd");

            parameterToAdd.ParameterTypeChanged += ChangeParameterType;
            parameterToAdd.PropertyChanged += HandleParameterDescriptionChanged;
            _parameters.Add(parameterToAdd);
            _originalParameters.Add(parameterToAdd.Parameter);
            MoveParameterDownCommand.UpdateCanExecute();
            MoveParameterUpCommand.UpdateCanExecute();

            OnPropertyChanged("ParametersCollectionEmpty");
            OnPropertyChanged("ActionParametersDescription");
        }

        private void AddParameter([NotNull] ParameterDescription newParameterDescription, int originalIndex)
        {
            Assert.ArgumentNotNull(newParameterDescription, "newParameterDescription");

            var newParameter = CreateNewActionParameterFromType(newParameterDescription);
            newParameter.ParameterTypeChanged += ChangeParameterType;
            newParameter.PropertyChanged += HandleParameterDescriptionChanged;
            _parameters.Insert(originalIndex, newParameter);
            _originalParameters.Insert(originalIndex, newParameter.Parameter);
            MoveParameterDownCommand.UpdateCanExecute();
            MoveParameterUpCommand.UpdateCanExecute();

            OnPropertyChanged("ParametersCollectionEmpty");
            OnPropertyChanged("ActionParametersDescription");
        }

        private void RemoveParameter([NotNull] ActionParameterViewModelBase parameter)
        {
            Assert.ArgumentNotNull(parameter, "parameter");

            parameter.ParameterTypeChanged -= ChangeParameterType;
            _originalParameters.Remove(parameter.Parameter);
            _parameters.Remove(parameter);

            MoveParameterDownCommand.UpdateCanExecute();
            MoveParameterUpCommand.UpdateCanExecute();

            OnPropertyChanged("ParametersCollectionEmpty");
            OnPropertyChanged("ActionParametersDescription");
        }

        private void HandleParameterDescriptionChanged([NotNull] object sender, [NotNull] PropertyChangedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            if (e.PropertyName == "ParameterDescription")
            {
                OnPropertyChanged("ActionParametersDescription");
            }
        }

        private void AddParameterCommandExecute([CanBeNull] object obj)
        {
            var castedParameter = obj as ActionParameterViewModelBase;
            if (castedParameter != null)
            {
                var originalIndex = Parameters.IndexOf(castedParameter);
                AddParameter(ParameterDescriptions.First(), originalIndex + 1);
            }
            else
            {
                AddParameter(ParameterDescriptions.First(), Parameters.Count);
            }
        }

        private void DeleteParameterCommandExecute([CanBeNull]object obj)
        {
            var castedParameter = obj as ActionParameterViewModelBase;
            if (castedParameter != null)
            {
                RemoveParameter(castedParameter);
            }
        }

        private void MoveParameterDownCommandExecute([CanBeNull]object obj)
        {
            var castedParameter = obj as ActionParameterViewModelBase;
            if (castedParameter != null)
            {
                var originalIndex = Parameters.IndexOf(castedParameter);
                var tempParam = _originalParameters[originalIndex];
                _originalParameters[originalIndex] = _originalParameters[originalIndex + 1];
                _originalParameters[originalIndex + 1] = tempParam;
                _parameters.Move(originalIndex, originalIndex + 1);
            }

            MoveParameterDownCommand.UpdateCanExecute();
            MoveParameterUpCommand.UpdateCanExecute();
            OnPropertyChanged("ActionParametersDescription");
        }

        private bool MoveParameterDownCommandCanExecute([CanBeNull] object arg)
        {
            var castedParameter = arg as ActionParameterViewModelBase;
            if (castedParameter != null)
            {
                var originalIndex = Parameters.IndexOf(castedParameter);
                return originalIndex < Parameters.Count - 1;
            }

            return false;
        }

        private bool MoveParameterUpCommandCanExecute([CanBeNull]object arg)
        {
            var castedParameter = arg as ActionParameterViewModelBase;
            if (castedParameter != null)
            {
                var originalIndex = Parameters.IndexOf(castedParameter);
                return originalIndex > 0;
            }

            return false;
        }

        private void MoveParameterUpCommandExecute([CanBeNull] object obj)
        {
            var castedParameter = obj as ActionParameterViewModelBase;
            if (castedParameter != null)
            {
                var originalIndex = Parameters.IndexOf(castedParameter);
                var tempParam = _originalParameters[originalIndex];
                _originalParameters[originalIndex] = _originalParameters[originalIndex - 1];
                _originalParameters[originalIndex - 1] = tempParam;
                _parameters.Move(originalIndex, originalIndex - 1);
            }

            MoveParameterDownCommand.UpdateCanExecute();
            MoveParameterUpCommand.UpdateCanExecute();
            OnPropertyChanged("ActionParametersDescription");
        }

        private void ChangeParameterType([NotNull] ActionParameterViewModelBase parameter, [NotNull] ParameterDescription newParameterDescription)
        {
            Assert.ArgumentNotNull(parameter, "parameter");
            Assert.ArgumentNotNull(newParameterDescription, "newParameterDescription");

            var originalIndex = Parameters.IndexOf(parameter);

            RemoveParameter(parameter);

            AddParameter(newParameterDescription, originalIndex);
        }

        #endregion
    }
}
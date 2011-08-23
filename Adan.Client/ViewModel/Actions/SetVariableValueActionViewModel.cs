// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetVariableValueActionViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the SetVariableValueActionViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ViewModel.Actions
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;

    using Common.Model;
    using Common.Plugins;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Model.Actions;

    /// <summary>
    /// View model for set variable value action.
    /// </summary>
    public class SetVariableValueActionViewModel : ActionViewModelBase
    {
        private readonly SetVariableValueAction _action;
        private ActionParameterViewModelBase _valueToSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetVariableValueActionViewModel"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="allVariables">All variables.</param>
        /// <param name="actionDescriptor">The action descriptor.</param>
        /// <param name="parameterDescriptions">The parameter descriptions.</param>
        /// <param name="allDescriptions">All descriptions.</param>
        public SetVariableValueActionViewModel([NotNull]SetVariableValueAction action, [NotNull] IEnumerable<Variable> allVariables, [NotNull] ActionDescription actionDescriptor, [NotNull] IEnumerable<ParameterDescription> parameterDescriptions, [NotNull] IEnumerable<ActionDescription> allDescriptions)
            : base(action, actionDescriptor, allDescriptions)
        {
            Assert.ArgumentNotNull(action, "action");
            Assert.ArgumentNotNull(allVariables, "allVariables");
            Assert.ArgumentNotNull(actionDescriptor, "actionDescriptor");
            Assert.ArgumentNotNull(parameterDescriptions, "parameterDescriptions");
            Assert.ArgumentNotNull(allDescriptions, "allDescriptions");

            AllVariables = allVariables;
            _action = action;
            ParameterDescriptions = parameterDescriptions;
            _valueToSet = ActionParametersViewModel.CreateParameterViewModel(action.ValueToSet, parameterDescriptions);
            _valueToSet.PropertyChanged += HandleValueToSetDescriptionChange;
        }

        /// <summary>
        /// Gets or sets the name of the variable.
        /// </summary>
        /// <value>
        /// The name of the variable.
        /// </value>
        [NotNull]
        public string VariableName
        {
            get
            {
                return _action.VariableName;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                _action.VariableName = value;
                OnPropertyChanged("VariableName");
                OnPropertyChanged("ActionDescription");
            }
        }

        /// <summary>
        /// Gets or sets the value to set parameter descriptor.
        /// </summary>
        /// <value>
        /// The value to set parameter descriptor.
        /// </value>
        [NotNull]
        public ParameterDescription ValueToSetParameterDescriptor
        {
            get
            {
                return _valueToSet.ParameterDescriptor;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                ValueToSet = ActionParametersViewModel.CreateNewActionParameterFromType(value);
                OnPropertyChanged("LogNameParameterDescriptor");
            }
        }

        /// <summary>
        /// Gets all variables.
        /// </summary>
        [NotNull]
        public IEnumerable<Variable> AllVariables
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the value to set.
        /// </summary>
        /// <value>
        /// The value to set.
        /// </value>
        [NotNull]
        public ActionParameterViewModelBase ValueToSet
        {
            get
            {
                return _valueToSet;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                _valueToSet.PropertyChanged -= HandleValueToSetDescriptionChange;
                _valueToSet = value;
                _action.ValueToSet = value.Parameter;
                _valueToSet.PropertyChanged += HandleValueToSetDescriptionChange;
                OnPropertyChanged("ValueToSet");
                OnPropertyChanged("ActionDescription");
            }
        }

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
        /// Gets the action description.
        /// </summary>
        public override string ActionDescription
        {
            get
            {
                return string.Format(
                    CultureInfo.CurrentUICulture,
                    "#var {{{0}}} {{{1}}}",
                    VariableName,
                    ValueToSet.ParameterDescription);
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A deep copy of this instance.</returns>
        public override ActionViewModelBase Clone()
        {
            return new SetVariableValueActionViewModel(new SetVariableValueAction(), AllVariables, ActionDescriptor, ParameterDescriptions, AllActionDescriptions) { VariableName = VariableName, ValueToSet = ValueToSet.Clone() };
        }

        private void HandleValueToSetDescriptionChange([NotNull] object sender, [NotNull] PropertyChangedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            if (e.PropertyName == "ParameterDescription")
            {
                OnPropertyChanged("ActionDescription");
            }
        }
    }
}

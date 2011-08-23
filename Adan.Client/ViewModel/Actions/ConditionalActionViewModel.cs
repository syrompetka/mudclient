// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConditionalActionViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ConditionalActionViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ViewModel.Actions
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;

    using Common.Plugins;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Model.Actions;

    /// <summary>
    /// View model for conditional action.
    /// </summary>
    public class ConditionalActionViewModel : ActionViewModelBase
    {
        private readonly ConditionalAction _action;
        private readonly IEnumerable<ActionDescription> _allDescriptors;
        private ActionParameterViewModelBase _leftConditionParameter;
        private ActionParameterViewModelBase _rightConditionParameter;
        private ActionsViewModel _actionsToExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionalActionViewModel"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="actionDescriptor">The action descriptor.</param>
        /// <param name="allDescriptors">All descriptors.</param>
        /// <param name="parameterDescriptions">The parameter descriptions.</param>
        public ConditionalActionViewModel([NotNull]ConditionalAction action, [NotNull] ActionDescription actionDescriptor, [NotNull]IEnumerable<ActionDescription> allDescriptors, [NotNull] IEnumerable<ParameterDescription> parameterDescriptions)
            : base(action, actionDescriptor, allDescriptors)
        {
            Assert.ArgumentNotNull(action, "action");
            Assert.ArgumentNotNull(actionDescriptor, "actionDescriptor");
            Assert.ArgumentNotNull(allDescriptors, "allDescriptors");
            Assert.ArgumentNotNull(parameterDescriptions, "parameterDescriptions");

            _action = action;
            _allDescriptors = allDescriptors;
            ParameterDescriptions = parameterDescriptions;
            _leftConditionParameter = ActionParametersViewModel.CreateParameterViewModel(_action.LeftConditionParameter, parameterDescriptions);
            _leftConditionParameter.PropertyChanged += HandleParameterOrActionDescriptionChange;
            _rightConditionParameter = ActionParametersViewModel.CreateParameterViewModel(_action.RightConditionParameter, parameterDescriptions);
            _rightConditionParameter.PropertyChanged += HandleParameterOrActionDescriptionChange;

            _actionsToExecute = new ActionsViewModel(action.ActionsToExecute, allDescriptors);
            _actionsToExecute.PropertyChanged += HandleParameterOrActionDescriptionChange;
        }

        /// <summary>
        /// Gets or sets the left condition parameter.
        /// </summary>
        /// <value>
        /// The left condition parameter.
        /// </value>
        [NotNull]
        public ActionParameterViewModelBase LeftConditionParameter
        {
            get
            {
                return _leftConditionParameter;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");
                if (_leftConditionParameter != null)
                {
                    _leftConditionParameter.PropertyChanged -= HandleParameterOrActionDescriptionChange;
                }

                _leftConditionParameter = value;
                _action.LeftConditionParameter = value.Parameter;
                _leftConditionParameter.PropertyChanged += HandleParameterOrActionDescriptionChange;

                OnPropertyChanged("LeftConditionParameter");
                OnPropertyChanged("ActionDescription");
            }
        }

        /// <summary>
        /// Gets or sets the right condition parameter.
        /// </summary>
        /// <value>
        /// The right condition parameter.
        /// </value>
        [NotNull]
        public ActionParameterViewModelBase RightConditionParameter
        {
            get
            {
                return _rightConditionParameter;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                if (_leftConditionParameter != null)
                {
                    _leftConditionParameter.PropertyChanged -= HandleParameterOrActionDescriptionChange;
                }

                _rightConditionParameter = value;
                _action.RightConditionParameter = value.Parameter;
                _rightConditionParameter.PropertyChanged += HandleParameterOrActionDescriptionChange;
                OnPropertyChanged("RightConditionParameter");
                OnPropertyChanged("ActionDescription");
            }
        }

        /// <summary>
        /// Gets or sets the left condition parameter descriptor.
        /// </summary>
        /// <value>
        /// The left condition parameter descriptor.
        /// </value>
        [NotNull]
        public ParameterDescription LeftConditionParameterDescriptor
        {
            get
            {
                return LeftConditionParameter.ParameterDescriptor;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");
                LeftConditionParameter = ActionParametersViewModel.CreateNewActionParameterFromType(value);
                OnPropertyChanged("LeftConditionParameterDescriptor");
            }
        }

        /// <summary>
        /// Gets or sets the right condition parameter descriptor.
        /// </summary>
        /// <value>
        /// The right condition parameter descriptor.
        /// </value>
        [NotNull]
        public ParameterDescription RightConditionParameterDescriptor
        {
            get
            {
                return RightConditionParameter.ParameterDescriptor;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");
                RightConditionParameter = ActionParametersViewModel.CreateNewActionParameterFromType(value);
                OnPropertyChanged("RightConditionParameterDescriptor");
            }
        }

        /// <summary>
        /// Gets all parameter descriptions available.
        /// </summary>
        [NotNull]
        public IEnumerable<ParameterDescription> ParameterDescriptions
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the condition.
        /// </summary>
        /// <value>
        /// The condition.
        /// </value>
        public ActionCondition Condition
        {
            get
            {
                return _action.Condition;
            }

            set
            {
                _action.Condition = value;
                OnPropertyChanged("Condition");
                OnPropertyChanged("ActionDescription");
                OnPropertyChanged("IsBinaryOperator");
            }
        }

        /// <summary>
        /// Gets a value indicating whether action's condition requires two parameters or not.
        /// </summary>
        /// <value>
        /// <c>true</c> if action's condition requires two parameters; otherwise, <c>false</c>.
        /// </value>
        public bool IsBinaryOperator
        {
            get
            {
                return Condition.IsBinaryOperator();
            }
        }

        /// <summary>
        /// Gets or sets the actions to execute.
        /// </summary>
        /// <value>
        /// The actions to execute.
        /// </value>
        [NotNull]
        public ActionsViewModel ActionsToExecute
        {
            get
            {
                return _actionsToExecute;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                if (_actionsToExecute != null)
                {
                    _actionsToExecute.PropertyChanged -= HandleParameterOrActionDescriptionChange;
                }

                _actionsToExecute = value;
                _actionsToExecute.PropertyChanged += HandleParameterOrActionDescriptionChange;
                OnPropertyChanged("ActionsToExecute");
                OnPropertyChanged("ActionDescription");
            }
        }

        /// <summary>
        /// Gets the action description.
        /// </summary>
        public override string ActionDescription
        {
            get
            {
                if (Condition.IsBinaryOperator())
                {
                    return string.Format(
                        CultureInfo.CurrentUICulture,
                        @"#if ({0} {1} {2}){{{3}}}",
                        LeftConditionParameter.ParameterDescription,
                        Condition.ConvertToSign(),
                        RightConditionParameter.ParameterDescription,
                        ActionsToExecute.ActionsDescription);
                }

                return string.Format(
                    CultureInfo.CurrentUICulture,
                    @"#if ({0} {1}){{{2}}}",
                    LeftConditionParameter.ParameterDescription,
                    Condition.ConvertToSign(),
                    ActionsToExecute.ActionsDescription);
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A deep copy of this instance.</returns>
        public override ActionViewModelBase Clone()
        {
            var conditionalAction = new ConditionalAction();
            return new ConditionalActionViewModel(conditionalAction, ActionDescriptor, _allDescriptors, ParameterDescriptions)
                       {
                           LeftConditionParameter = LeftConditionParameter.Clone(),
                           RightConditionParameter = RightConditionParameter.Clone(),
                           Condition = Condition,
                           ActionsToExecute = ActionsToExecute.Clone(conditionalAction.ActionsToExecute)
                       };
        }

        private void HandleParameterOrActionDescriptionChange([NotNull] object sender, [NotNull] PropertyChangedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            if (e.PropertyName == "ParameterDescription" || e.PropertyName == "ActionsDescription")
            {
                OnPropertyChanged("ActionDescription");
            }
        }
    }
}

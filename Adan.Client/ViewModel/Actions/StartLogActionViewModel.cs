// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartLogActionViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the StartLogActionViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ViewModel.Actions
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using Common.Plugins;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Model.Actions;

    /// <summary>
    /// View model for start log action.
    /// </summary>
    public class StartLogActionViewModel : ActionViewModelBase
    {
        private readonly StartLogAction _action;
        private ActionParameterViewModelBase _logNameParameter;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartLogActionViewModel"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="actionDescriptor">The action descriptor.</param>
        /// <param name="parameterDescriptions">The parameter descriptions.</param>
        /// <param name="allDescriptions">All descriptions.</param>
        public StartLogActionViewModel([NotNull] StartLogAction action, [NotNull] ActionDescription actionDescriptor, [NotNull] IEnumerable<ParameterDescription> parameterDescriptions, [NotNull] IEnumerable<ActionDescription> allDescriptions)
            : base(action, actionDescriptor, allDescriptions)
        {
            Assert.ArgumentNotNull(action, "action");
            Assert.ArgumentNotNull(actionDescriptor, "actionDescriptor");
            Assert.ArgumentNotNull(parameterDescriptions, "parameterDescriptions");
            Assert.ArgumentNotNull(allDescriptions, "allDescriptions");

            _action = action;
            ParameterDescriptions = parameterDescriptions;

            _logNameParameter = ActionParametersViewModel.CreateParameterViewModel(_action.LogNameParameter, parameterDescriptions);
            _logNameParameter.PropertyChanged += HandleLogNameParameterDescriptionChange;
        }

        /// <summary>
        /// Gets or sets the log name parameter.
        /// </summary>
        /// <value>
        /// The log name parameter.
        /// </value>
        [NotNull]
        public ActionParameterViewModelBase LogNameParameter
        {
            get
            {
                return _logNameParameter;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");
                _logNameParameter.PropertyChanged -= HandleLogNameParameterDescriptionChange;

                _logNameParameter = value;
                _action.LogNameParameter = value.Parameter;
                _logNameParameter.PropertyChanged += HandleLogNameParameterDescriptionChange;
                OnPropertyChanged("LogNameParameter");
                OnPropertyChanged("ActionDescription");
            }
        }

        /// <summary>
        /// Gets or sets the log name parameter descriptor.
        /// </summary>
        /// <value>
        /// The log name parameter descriptor.
        /// </value>
        [NotNull]
        public ParameterDescription LogNameParameterDescriptor
        {
            get
            {
                return _logNameParameter.ParameterDescriptor;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                LogNameParameter = ActionParametersViewModel.CreateNewActionParameterFromType(value);
                OnPropertyChanged("LogNameParameterDescriptor");
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
                return "#startlog " + LogNameParameter.ParameterDescription;
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A deep copy of this instance.</returns>
        public override ActionViewModelBase Clone()
        {
            return new StartLogActionViewModel(new StartLogAction(), ActionDescriptor, ParameterDescriptions, AllActionDescriptions) { LogNameParameter = LogNameParameter.Clone() };
        }

        private void HandleLogNameParameterDescriptionChange([NotNull] object sender, [NotNull] PropertyChangedEventArgs e)
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

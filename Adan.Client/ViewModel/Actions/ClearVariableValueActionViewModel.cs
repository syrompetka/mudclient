// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClearVariableValueActionViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ClearVariableValueActionViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ViewModel.Actions
{
    using System.Collections.Generic;
    using System.Globalization;

    using Common.Model;
    using Common.Plugins;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Model.Actions;

    /// <summary>
    /// View model for unset variable value action.
    /// </summary>
    public class ClearVariableValueActionViewModel : ActionViewModelBase
    {
        private readonly ClearVariableValueAction _action;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClearVariableValueActionViewModel"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="actionDescriptor">The action descriptor.</param>
        /// <param name="allDescriptions">All descriptions.</param>
        public ClearVariableValueActionViewModel([NotNull] ClearVariableValueAction action, [NotNull] ActionDescription actionDescriptor, [NotNull] IEnumerable<ActionDescription> allDescriptions)
            : base(action, actionDescriptor, allDescriptions)
        {
            Assert.ArgumentNotNull(action, "action");
            Assert.ArgumentNotNull(actionDescriptor, "actionDescriptor");
            Assert.ArgumentNotNull(allDescriptions, "allDescriptions");

            _action = action;
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
        /// Gets or sets Silent mode
        /// </summary>
        public bool SilentSet
        {
            get
            {
                return _action.SilentSet;
            }
            set
            {
                _action.SilentSet = value;
                OnPropertyChanged("SilentSet");
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
                return string.Format(
                    CultureInfo.CurrentUICulture,
                    "#unvar {{{0}}}",
                    VariableName);
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A deep copy of this instance.</returns>
        public override ActionViewModelBase Clone()
        {
            return new ClearVariableValueActionViewModel
                (new ClearVariableValueAction(), ActionDescriptor, AllActionDescriptions)
                { 
                    VariableName = VariableName,
                    SilentSet = SilentSet
                };
        }
    }
}

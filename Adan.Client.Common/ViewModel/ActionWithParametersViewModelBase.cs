// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionWithParametersViewModelBase.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ActionWithParametersViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.ViewModel
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Model;
    using Plugins;

    /// <summary>
    /// Base view model for actions that have parameters.
    /// </summary>
    public abstract class ActionWithParametersViewModelBase : ActionViewModelBase
    {
        private ActionParametersViewModel _parametersModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionWithParametersViewModelBase"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="actionDescription">The action description.</param>
        /// <param name="parameterDescriptions">The parameter descriptions.</param>
        /// <param name="allDescriptions">All descriptions.</param>
        protected ActionWithParametersViewModelBase([NotNull] ActionWithParameters action, [NotNull] ActionDescription actionDescription, [NotNull] IEnumerable<ParameterDescription> parameterDescriptions, [NotNull] IEnumerable<ActionDescription> allDescriptions)
            : base(action, actionDescription, allDescriptions)
        {
            Assert.ArgumentNotNull(action, "action");
            Assert.ArgumentNotNull(actionDescription, "actionDescription");
            Assert.ArgumentNotNull(parameterDescriptions, "parameterDescriptions");
            Assert.ArgumentNotNull(allDescriptions, "allDescriptions");

            ParametersModel = new ActionParametersViewModel(action.Parameters, parameterDescriptions);
        }

        /// <summary>
        /// Gets or sets the parameters model.
        /// </summary>
        /// <value>
        /// The parameters model.
        /// </value>
        [NotNull]
        public ActionParametersViewModel ParametersModel
        {
            get
            {
                return _parametersModel;
            }

            protected set
            {
                Assert.ArgumentNotNull(value, "value");

                if (_parametersModel != null)
                {
                    _parametersModel.PropertyChanged -= HandleParametersDescriptionChanged;
                }

                _parametersModel = value;               
                _parametersModel.PropertyChanged += HandleParametersDescriptionChanged;
            }
        }

        private void HandleParametersDescriptionChanged([NotNull] object sender, [NotNull] PropertyChangedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            if (e.PropertyName == "ActionParametersDescription")
            {
                OnPropertyChanged("ActionDescription");
            }
        }
    }
}
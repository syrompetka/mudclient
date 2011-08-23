// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClearVariableValueActionDescription.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ClearVariableValueActionDescription type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Model.ActionDescriptions
{
    using System.Collections.Generic;

    using Actions;

    using Common.Model;
    using Common.Plugins;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using ViewModel.Actions;

    /// <summary>
    /// A <see cref="ActionDescription"/> implementation for <see cref="ClearVariableValueAction"/>.
    /// </summary>
    public class ClearVariableValueActionDescription : ActionDescription
    {
        private readonly IEnumerable<Variable> _allVariables;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClearVariableValueActionDescription"/> class.
        /// </summary>
        /// <param name="allDescriptions">All descriptions.</param>
        /// <param name="allVariables">All variables.</param>
        public ClearVariableValueActionDescription([NotNull] IEnumerable<ActionDescription> allDescriptions, [NotNull] IEnumerable<Variable> allVariables)
            : base("Clear variable value", allDescriptions)
        {
            Assert.ArgumentNotNull(allDescriptions, "allDescriptions");
            Assert.ArgumentNotNull(allVariables, "allVariables");
            _allVariables = allVariables;
        }

        /// <summary>
        /// Creates the <see cref="ActionBase"/> derived class instace.
        /// </summary>
        /// <returns>
        /// The instance of <see cref="ActionBase"/> derived class.
        /// </returns>
        public override ActionBase CreateAction()
        {
            return new ClearVariableValueAction();
        }

        /// <summary>
        /// Creates the action view model by specified <see cref="ActionBase"/> instance.
        /// </summary>
        /// <param name="action">The <see cref="ActionBase"/> instance to create view model for.</param>
        /// <returns>Created action view model or <c>null</c> if specified action is not supported by this description.</returns>
        public override ActionViewModelBase CreateActionViewModel(ActionBase action)
        {
            Assert.ArgumentNotNull(action, "action");

            var clearVariableAction = action as ClearVariableValueAction;
            if (clearVariableAction != null)
            {
                return new ClearVariableValueActionViewModel(clearVariableAction, _allVariables, this, AllDescriptions);
            }

            return null;
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConditionalActionDescription.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ConditionalActionDescription type.
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
    ///  A <see cref="ActionDescription"/> implementation for <see cref="ConditionalAction"/>.
    /// </summary>
    public class ConditionalActionDescription : ActionDescription
    {
        private readonly IEnumerable<ParameterDescription> _parameterDescriptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionalActionDescription"/> class.
        /// </summary>
        /// <param name="parameterDescriptions">The parameter descriptions.</param>
        /// <param name="allDescriptions">All descriptions.</param>
        public ConditionalActionDescription([NotNull] IEnumerable<ParameterDescription> parameterDescriptions, [NotNull] IEnumerable<ActionDescription> allDescriptions)
            : base("Conditional action", allDescriptions)
        {
            Assert.ArgumentNotNull(parameterDescriptions, "parameterDescriptions");
            Assert.ArgumentNotNull(allDescriptions, "allDescriptions");
            _parameterDescriptions = parameterDescriptions;
        }

        /// <summary>
        /// Creates the <see cref="ActionBase"/> derived class instace.
        /// </summary>
        /// <returns>
        /// The instance of <see cref="ActionBase"/> derived class.
        /// </returns>
        public override ActionBase CreateAction()
        {
            var condAction = new ConditionalAction();
            condAction.ActionsToExecute.Add(new SendTextAction());
            return condAction;
        }

        /// <summary>
        /// Creates the action view model by specified <see cref="ActionBase"/> instance.
        /// </summary>
        /// <param name="action">The <see cref="ActionBase"/> instance to create view model for.</param>
        /// <returns>
        /// Created action view model or <c>null</c> if specified action is not supported by this description.
        /// </returns>
        public override ActionViewModelBase CreateActionViewModel(ActionBase action)
        {
            Assert.ArgumentNotNull(action, "action");

            var conditionalAction = action as ConditionalAction;
            if (conditionalAction != null)
            {
                return new ConditionalActionViewModel(conditionalAction, this, AllDescriptions, _parameterDescriptions);
            }

            return null;
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusActionDescription.cs" company="Adamand MUD">
//   Copyright (c) Bester
// </copyright>
// <summary>
//   Defines the StatusActionDescription type.
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
    /// A <see cref="ActionDescription"/> implementation for <see cref="StatusAction"/>.
    /// </summary>
    public class StatusActionDescription : ActionDescription
    {
        private readonly IEnumerable<ParameterDescription> _parameterDescriptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusActionDescription"/> class.
        /// </summary>
        /// <param name="parameterDescriptions">The parameter descriptions.</param>
        /// <param name="allDescriptions">All descriptions.</param>
        public StatusActionDescription([NotNull]IEnumerable<ParameterDescription> parameterDescriptions, [NotNull] IEnumerable<ActionDescription> allDescriptions)
            : base("Set status bar widget", allDescriptions)
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
            return new StatusAction();
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
            var statusAction = action as StatusAction;
            if (statusAction != null)
            {
                return new StatusActionViewModel(statusAction, this, _parameterDescriptions, AllDescriptions);
            }

            return null;
        }
    }
}

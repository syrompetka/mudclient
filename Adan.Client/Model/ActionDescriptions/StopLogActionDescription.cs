// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StopLogActionDescription.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the StopLogActionDescription type.
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
    /// A <see cref="ActionDescription"/> implementation for <see cref="StopLogAction"/>.
    /// </summary>
    public class StopLogActionDescription : ActionDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StopLogActionDescription"/> class.
        /// </summary>
        /// <param name="allDescriptions">All descriptions.</param>
        public StopLogActionDescription([NotNull] IEnumerable<ActionDescription> allDescriptions)
            : base("Stop log", allDescriptions)
        {
            Assert.ArgumentNotNull(allDescriptions, "allDescriptions");
        }

        /// <summary>
        /// Creates the <see cref="ActionBase"/> derived class instace.
        /// </summary>
        /// <returns>
        /// The instance of <see cref="ActionBase"/> derived class.
        /// </returns>
        public override ActionBase CreateAction()
        {
            return new StopLogAction();
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

            var stopLogAction = action as StopLogAction;
            if (stopLogAction != null)
            {
                return new StopLogActionViewModel(stopLogAction, this, AllDescriptions);
            }

            return null;
        }
    }
}

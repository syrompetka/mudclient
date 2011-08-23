// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartLogActionDescription.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the StartLogActionDescription type.
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
    /// A <see cref="ActionDescription"/> implementation for <see cref="StartLogAction"/>.
    /// </summary>
    public class StartLogActionDescription : ActionDescription
    {
        private readonly IEnumerable<ParameterDescription> _parameterDescriptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartLogActionDescription"/> class.
        /// </summary>
        /// <param name="allDescriptions">All descriptions.</param>
        /// <param name="parameterDescriptions">The parameter descriptions.</param>
        public StartLogActionDescription([NotNull] IEnumerable<ActionDescription> allDescriptions, [NotNull] IEnumerable<ParameterDescription> parameterDescriptions)
            : base("Start log", allDescriptions)
        {
            _parameterDescriptions = parameterDescriptions;
            Assert.ArgumentNotNull(allDescriptions, "allDescriptions");
            Assert.ArgumentNotNull(parameterDescriptions, "parameterDescriptions");
        }

        /// <summary>
        /// Creates the <see cref="ActionBase"/> derived class instace.
        /// </summary>
        /// <returns>
        /// The instance of <see cref="ActionBase"/> derived class.
        /// </returns>
        public override ActionBase CreateAction()
        {
            return new StartLogAction();
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

            var startLocaAction = action as StartLogAction;
            if (startLocaAction != null)
            {
                return new StartLogActionViewModel(startLocaAction, this, _parameterDescriptions, AllDescriptions);
            }

            return null;
        }
    }
}

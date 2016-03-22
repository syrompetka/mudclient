// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputToMainWindowActionDescription.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the OutputToMainWindowActionDescription type.
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
    /// A <see cref="ActionDescription"/> implementation for <see cref="OutputToMainWindowActionDescription"/>.
    /// </summary>
    public class OutputToMainWindowActionDescription : ActionDescription
    {
        private readonly IEnumerable<ParameterDescription> _parameterDescriptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputToMainWindowActionDescription"/> class.
        /// </summary>
        /// <param name="parameterDescriptions">The parameter descriptions.</param>
        /// <param name="allDescriptions">All descriptions.</param>
        public OutputToMainWindowActionDescription([NotNull] IEnumerable<ParameterDescription> parameterDescriptions, [NotNull] IEnumerable<ActionDescription> allDescriptions)
            : base("Output to main window", allDescriptions)
        {
            Assert.ArgumentNotNull(allDescriptions, "allDescriptions");
            Assert.ArgumentNotNull(parameterDescriptions, "parameterDescriptions");

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
            return new OutputToMainWindowAction();
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
            var outputToMainWindowAction = action as OutputToMainWindowAction;
            if (outputToMainWindowAction != null)
            {
                return new OutputToMainWindowActionViewModel(outputToMainWindowAction, this, _parameterDescriptions, AllDescriptions);
            }

            return null;
        }
    }
}

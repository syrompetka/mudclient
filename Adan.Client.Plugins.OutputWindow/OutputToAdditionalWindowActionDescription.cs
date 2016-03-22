// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputToAdditionalWindowActionDescription.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the OutputToAdditionalWindowActionDescription type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.OutputWindow
{
    using System.Collections.Generic;

    using Common.Model;
    using Common.Plugins;
    using Common.ViewModel;
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// A <see cref="ActionDescription"/> implementation for <see cref="OutputToAdditionalWindowAction"/>.
    /// </summary>
    public class OutputToAdditionalWindowActionDescription : ActionDescription
    {
        private readonly IEnumerable<ParameterDescription> _parameterDescriptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputToAdditionalWindowActionDescription"/> class.
        /// </summary>
        /// <param name="parameterDescriptions">The parameter descriptions.</param>
        /// <param name="allDescriptions">All descriptions.</param>
        public OutputToAdditionalWindowActionDescription([NotNull] IEnumerable<ParameterDescription> parameterDescriptions, [NotNull] IEnumerable<ActionDescription> allDescriptions)
            : base("Output to additional window", allDescriptions)
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
            return new OutputToAdditionalWindowAction();
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
            var outputToMainWindowAction = action as OutputToAdditionalWindowAction;
            if (outputToMainWindowAction != null)
            {
                return new OutputToAdditionalWindowActionViewModel(outputToMainWindowAction, this, _parameterDescriptions, AllDescriptions);
            }

            return null;
        }
    }
}

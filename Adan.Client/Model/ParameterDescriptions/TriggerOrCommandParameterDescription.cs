// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TriggerOrCommandParameterDescription.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the TriggerOrCommandParameterDescription type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Model.ParameterDescriptions
{
    using System.Collections.Generic;

    using ActionParameters;

    using Common.Model;
    using Common.Plugins;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using ViewModel.ActionParameters;

    /// <summary>
    /// A <see cref="ParameterDescription"/> implementation for <see cref="TriggerOrCommandParameter"/>.
    /// </summary>
    public class TriggerOrCommandParameterDescription : ParameterDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerOrCommandParameterDescription"/> class.
        /// </summary>
        /// <param name="parameterDescriptions">The parameter descriptions.</param>
        public TriggerOrCommandParameterDescription([NotNull] IEnumerable<ParameterDescription> parameterDescriptions)
            : base("Trigger or command parameter", parameterDescriptions)
        {
            Assert.ArgumentNotNull(parameterDescriptions, "parameterDescriptions");
        }

        /// <summary>
        /// Creates the parameter.
        /// </summary>
        /// <returns>Created parameter.</returns>
        public override ActionParameterBase CreateParameter()
        {
            return new TriggerOrCommandParameter();
        }

        /// <summary>
        /// Creates the parameter view model.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        /// Created parameter view model.
        /// </returns>
        public override ActionParameterViewModelBase CreateParameterViewModel(ActionParameterBase parameter)
        {
            Assert.ArgumentNotNull(parameter, "parameter");

            var triggerOrCommandParameter = parameter as TriggerOrCommandParameter;
            if (triggerOrCommandParameter != null)
            {
                return new TriggerOrCommandParameterViewModel(triggerOrCommandParameter, this, ParameterDescriptions);
            }

            return null;
        }
    }
}

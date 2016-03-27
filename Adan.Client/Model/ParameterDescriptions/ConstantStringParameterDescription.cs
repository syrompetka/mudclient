// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConstantStringParameterDescription.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ConstantStringParameterDescription type.
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
    /// A <see cref="ParameterDescription"/> implementation for <see cref="ConstantStringParameter"/>.
    /// </summary>
    public class ConstantStringParameterDescription : ParameterDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantStringParameterDescription"/> class.
        /// </summary>
        /// <param name="parameterDescriptions">The parameter descriptions.</param>
        public ConstantStringParameterDescription([NotNull] IEnumerable<ParameterDescription> parameterDescriptions)
            : base("Constant string", parameterDescriptions)
        {
            Assert.ArgumentNotNull(parameterDescriptions, "parameterDescriptions");
        }

        /// <summary>
        /// Creates the parameter.
        /// </summary>
        /// <returns>Created parameter.</returns>
        public override ActionParameterBase CreateParameter()
        {
            return new ConstantStringParameter();
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

            var constantStringParameter = parameter as ConstantStringParameter;
            if (constantStringParameter != null)
            {
                return new ConstantStringParameterViewModel(constantStringParameter, this, ParameterDescriptions);
            }

            return null;
        }
    }
}

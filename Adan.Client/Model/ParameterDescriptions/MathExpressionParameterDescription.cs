// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MathExpressionParameterDescription.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the MathExpressionParameterDescription type.
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
    /// A <see cref="ParameterDescription"/> implementation for <see cref="MathExpressionParameter"/>.
    /// </summary>
    public class MathExpressionParameterDescription : ParameterDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MathExpressionParameterDescription"/> class.
        /// </summary>
        /// <param name="parameterDescriptions">The parameter descriptions.</param>
        public MathExpressionParameterDescription([NotNull] IEnumerable<ParameterDescription> parameterDescriptions)
            : base("Math expression", parameterDescriptions)
        {
            Assert.ArgumentNotNull(parameterDescriptions, "parameterDescriptions");
        }

        /// <summary>
        /// Creates the parameter.
        /// </summary>
        /// <returns>Created parameter.</returns>
        public override ActionParameterBase CreateParameter()
        {
            return new MathExpressionParameter();
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

            var mathExpressionParameter = parameter as MathExpressionParameter;
            if (mathExpressionParameter != null)
            {
                return new MathExpressionParameterViewModel(mathExpressionParameter, this, ParameterDescriptions);
            }

            return null;
        }
    }
}

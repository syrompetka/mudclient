// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VariableReferenceParameterDescription.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the VariableReferenceParameterDescription type.
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
    /// A <see cref="ParameterDescription"/> implementation for <see cref="VariableReferenceParameter"/>.
    /// </summary>
    public class VariableReferenceParameterDescription : ParameterDescription
    {
        private readonly IEnumerable<Variable> _allVariables;

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableReferenceParameterDescription"/> class.
        /// </summary>
        /// <param name="parameterDescriptions">The parameter descriptions.</param>
        /// <param name="allVariables">All variables.</param>
        public VariableReferenceParameterDescription([NotNull] IEnumerable<ParameterDescription> parameterDescriptions, [NotNull] IEnumerable<Variable> allVariables)
            : base("Variable reference", parameterDescriptions)
        {
            _allVariables = allVariables;
            Assert.ArgumentNotNull(parameterDescriptions, "parameterDescriptions");
            Assert.ArgumentNotNull(allVariables, "allVariables");
        }

        /// <summary>
        /// Creates the parameter.
        /// </summary>
        /// <returns>Created parameter.</returns>
        public override ActionParameterBase CreateParameter()
        {
            return new VariableReferenceParameter();
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

            var variableReferenceParameter = parameter as VariableReferenceParameter;
            if (variableReferenceParameter != null)
            {
                return new VariableReferenceParameterViewModel(variableReferenceParameter, _allVariables, this, ParameterDescriptions);
            }

            return null;
        }
    }
}

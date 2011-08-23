// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VariableReferenceParameterViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the VariableReferenceParameterViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ViewModel.ActionParameters
{
    using System.Collections.Generic;

    using Common.Model;
    using Common.Plugins;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Model.ActionParameters;

    /// <summary>
    /// View model for variable reference parameter.
    /// </summary>
    public class VariableReferenceParameterViewModel : ActionParameterViewModelBase
    {
        private readonly VariableReferenceParameter _parameter;

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableReferenceParameterViewModel"/> class.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="allVariables">All variables.</param>
        /// <param name="parameterDescriptor">The parameter descriptor.</param>
        /// <param name="allParameterDescriptions">All parameter descriptions.</param>
        public VariableReferenceParameterViewModel([NotNull] VariableReferenceParameter parameter, [NotNull] IEnumerable<Variable> allVariables, [NotNull] ParameterDescription parameterDescriptor, [NotNull] IEnumerable<ParameterDescription> allParameterDescriptions)
            : base(parameter, parameterDescriptor, allParameterDescriptions)
        {
            Assert.ArgumentNotNull(parameter, "parameter");
            Assert.ArgumentNotNull(allVariables, "allVariables");
            Assert.ArgumentNotNull(parameterDescriptor, "parameterDescriptor");
            Assert.ArgumentNotNull(allParameterDescriptions, "allParameterDescriptions");

            AllVariables = allVariables;

            _parameter = parameter;
        }

        /// <summary>
        /// Gets or sets the name of the variable.
        /// </summary>
        /// <value>
        /// The name of the variable.
        /// </value>
        [NotNull]
        public string VariableName
        {
            get
            {
                return _parameter.VariableName;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                _parameter.VariableName = value;
                OnPropertyChanged("VariableName");
                OnPropertyChanged("ParameterDescription");
            }
        }

        /// <summary>
        /// Gets all variables.
        /// </summary>
        [NotNull]
        public IEnumerable<Variable> AllVariables
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the parameter description.
        /// </summary>
        public override string ParameterDescription
        {
            get
            {
                return "$" + VariableName;
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A deep copy of this instance.</returns>
        public override ActionParameterViewModelBase Clone()
        {
            return new VariableReferenceParameterViewModel(new VariableReferenceParameter(), AllVariables, ParameterDescriptor, AllParameterDescriptions) { VariableName = VariableName };
        }
    }
}

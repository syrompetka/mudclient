// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConstantStringParameterViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ConstantStringParameterViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ViewModel.ActionParameters
{
    using System.Collections.Generic;

    using Common.Plugins;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Model.ActionParameters;

    /// <summary>
    /// View model for constant string parameter.
    /// </summary>
    public class ConstantStringParameterViewModel : ActionParameterViewModelBase
    {
        private readonly ConstantStringParameter _parameter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantStringParameterViewModel"/> class.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterDescriptor">The parameter descriptor.</param>
        /// <param name="allParameterDescriptions">All parameter descriptions.</param>
        public ConstantStringParameterViewModel([NotNull] ConstantStringParameter parameter, [NotNull] ParameterDescription parameterDescriptor, [NotNull] IEnumerable<ParameterDescription> allParameterDescriptions)
            : base(parameter, parameterDescriptor, allParameterDescriptions)
        {
            Assert.ArgumentNotNull(parameter, "parameter");
            Assert.ArgumentNotNull(parameterDescriptor, "parameterDescriptor");
            Assert.ArgumentNotNull(allParameterDescriptions, "allParameterDescriptions");

            _parameter = parameter;
        }

        /// <summary>
        /// Gets or sets the constant string.
        /// </summary>
        /// <value>
        /// The constant string.
        /// </value>
        [NotNull]
        public string ConstantString
        {
            get
            {
                return _parameter.ConstantString;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                _parameter.ConstantString = value;
                OnPropertyChanged("ConstantString");
                OnPropertyChanged("ParameterDescription");
            }
        }

        /// <summary>
        /// Gets the parameter description.
        /// </summary>
        public override string ParameterDescription
        {
            get
            {
                return ConstantString;
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A deep copy of this instance.</returns>
        public override ActionParameterViewModelBase Clone()
        {
            return new ConstantStringParameterViewModel(new ConstantStringParameter(), ParameterDescriptor, AllParameterDescriptions) { ConstantString = ConstantString };
        }
    }
}

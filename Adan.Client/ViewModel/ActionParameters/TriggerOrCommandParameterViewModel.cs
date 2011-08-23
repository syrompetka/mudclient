// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TriggerOrCommandParameterViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the TriggerOrCommandParameterViewModel type.
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
    /// View model for trigger or command parameter.
    /// </summary>
    public class TriggerOrCommandParameterViewModel : ActionParameterViewModelBase
    {
        private readonly TriggerOrCommandParameter _parameter;

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerOrCommandParameterViewModel"/> class.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterDescriptor">The parameter descriptor.</param>
        /// <param name="allParameterDescriptions">All parameter descriptions.</param>
        public TriggerOrCommandParameterViewModel([NotNull] TriggerOrCommandParameter parameter, [NotNull] ParameterDescription parameterDescriptor, [NotNull] IEnumerable<ParameterDescription> allParameterDescriptions)
            : base(parameter, parameterDescriptor, allParameterDescriptions)
        {
            Assert.ArgumentNotNull(parameter, "parameter");
            Assert.ArgumentNotNull(parameterDescriptor, "parameterDescriptor");
            Assert.ArgumentNotNull(allParameterDescriptions, "allParameterDescriptions");

            _parameter = parameter;
        }

        /// <summary>
        /// Gets or sets the parameter number.
        /// </summary>
        /// <value>
        /// The parameter number.
        /// </value>
        public int ParameterNumber
        {
            get
            {
                return _parameter.ParameterNumber;
            }

            set
            {
                _parameter.ParameterNumber = value;
                OnPropertyChanged("ParameterNumber");
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
                return "%" + ParameterNumber;
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A deep copy of this instance.</returns>
        public override ActionParameterViewModelBase Clone()
        {
            return new TriggerOrCommandParameterViewModel(new TriggerOrCommandParameter(), ParameterDescriptor, AllParameterDescriptions) { ParameterNumber = ParameterNumber };
        }
    }
}

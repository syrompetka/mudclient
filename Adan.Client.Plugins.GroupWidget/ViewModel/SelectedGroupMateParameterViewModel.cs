// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectedGroupMateParameterViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the SelectedGroupMateParameterViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.GroupWidget.ViewModel
{
    using System.Collections.Generic;

    using Common.Model;
    using Common.Plugins;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// A view model for <see cref="SelectedGroupMateParameter"/>.
    /// </summary>
    public class SelectedGroupMateParameterViewModel : ActionParameterViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedGroupMateParameterViewModel"/> class.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterDescriptor">The parameter descriptor.</param>
        /// <param name="allParameterDescriptions">All parameter descriptions.</param>
        public SelectedGroupMateParameterViewModel([NotNull] ActionParameterBase parameter, [NotNull] ParameterDescription parameterDescriptor, [NotNull] IEnumerable<ParameterDescription> allParameterDescriptions)
            : base(parameter, parameterDescriptor, allParameterDescriptions)
        {
            Assert.ArgumentNotNull(parameter, "parameter");
            Assert.ArgumentNotNull(parameterDescriptor, "parameterDescriptor");
            Assert.ArgumentNotNull(allParameterDescriptions, "allParameterDescriptions");
        }

        /// <summary>
        /// Gets the parameter description.
        /// </summary>
        public override string ParameterDescription
        {
            get
            {
                return "$GroupMate";
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A deep copy of this instance.</returns>
        public override ActionParameterViewModelBase Clone()
        {
            return new SelectedGroupMateParameterViewModel(new SelectedGroupMateParameter(), ParameterDescriptor, AllParameterDescriptions);
        }
    }
}

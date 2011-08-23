// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterDescription.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ParameterDescription type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Plugins
{
    using System.Collections.Generic;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Model;

    using ViewModel;

    /// <summary>
    /// A description of parameter type.
    /// </summary>
    public abstract class ParameterDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterDescription"/> class.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <param name="parameterDescriptions">The parameter descriptions.</param>
        protected ParameterDescription([NotNull] string displayName, [NotNull] IEnumerable<ParameterDescription> parameterDescriptions)
        {
            Assert.ArgumentNotNullOrWhiteSpace(displayName, "displayName");
            Assert.ArgumentNotNull(parameterDescriptions, "parameterDescriptions");
            DisplayName = displayName;
            ParameterDescriptions = parameterDescriptions;
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        [NotNull]
        public string DisplayName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets all parameter descriptions available.
        /// </summary>
        [NotNull]
        public IEnumerable<ParameterDescription> ParameterDescriptions
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates the parameter.
        /// </summary>
        /// <returns>Created parameter.</returns>
        [NotNull]
        public abstract ActionParameterBase CreateParameter();

        /// <summary>
        /// Creates the parameter view model.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>Created parameter view model.</returns>
        [CanBeNull]
        public abstract ActionParameterViewModelBase CreateParameterViewModel([NotNull] ActionParameterBase parameter);
    }
}

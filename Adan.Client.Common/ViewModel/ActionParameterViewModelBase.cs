// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionParameterViewModelBase.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ActionParamterViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.ViewModel
{
    using System.Collections.Generic;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Model;

    using Plugins;

    /// <summary>
    /// Delegate declaration for <see cref="ActionParameterViewModelBase.ParameterTypeChanged"/> event.
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    /// <param name="newParameterDescription">The new parameter description.</param>
    public delegate void ParameterChangedEventHandler([NotNull]ActionParameterViewModelBase parameter, [NotNull] ParameterDescription newParameterDescription);

    /// <summary>
    /// Base class for action parameter view model.
    /// </summary>
    public abstract class ActionParameterViewModelBase : ViewModelBase
    {
        private readonly ParameterDescription _parameterDescriptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionParameterViewModelBase"/> class.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterDescriptor">The parameter descriptor.</param>
        /// <param name="allParameterDescriptions">All parameter descriptions.</param>
        protected ActionParameterViewModelBase([NotNull] ActionParameterBase parameter, [NotNull] ParameterDescription parameterDescriptor, [NotNull] IEnumerable<ParameterDescription> allParameterDescriptions)
        {
            Assert.ArgumentNotNull(parameter, "parameter");
            Assert.ArgumentNotNull(parameterDescriptor, "parameterDescriptor");
            Assert.ArgumentNotNull(allParameterDescriptions, "allParameterDescriptions");

            Parameter = parameter;
            AllParameterDescriptions = allParameterDescriptions;
            _parameterDescriptor = parameterDescriptor;
        }

        /// <summary>
        /// Occurs when type of this paramater is changed.
        /// </summary>
        public event ParameterChangedEventHandler ParameterTypeChanged;

        /// <summary>
        /// Gets the parameter.
        /// </summary>
        [NotNull]
        public ActionParameterBase Parameter
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets all parameter descriptions.
        /// </summary>
        [NotNull]
        public IEnumerable<ParameterDescription> AllParameterDescriptions
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the type of the parameter. (Note: this property is needed for binding only)
        /// </summary>
        /// <value>
        /// The type of the parameter.
        /// </value>
        [NotNull]
        public ParameterDescription ParameterDescriptor
        {
            get
            {
                return _parameterDescriptor;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                if (ParameterTypeChanged != null)
                {
                    ParameterTypeChanged(this, value);
                }
            }
        }

        /// <summary>
        /// Gets the parameter description.
        /// </summary>
        [NotNull]
        public abstract string ParameterDescription
        {
            get;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A deep copy of this instance.</returns>
        [NotNull]
        public abstract ActionParameterViewModelBase Clone();
    }
}

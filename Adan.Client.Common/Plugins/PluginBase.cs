// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginBase.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the PluginBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CommandSerializers;

    using Conveyor;

    using ConveyorUnits;

    using CSLib.Net.Annotations;

    using MessageDeserializers;

    using Model;

    /// <summary>
    /// Base class for all plugins.
    /// </summary>
    public abstract class PluginBase
    {
        /// <summary>
        /// Gets the conveyor units that this plugin exposes.
        /// </summary>
        [NotNull]
        public virtual IEnumerable<ConveyorUnit> ConveyorUnits
        {
            get
            {
                return Enumerable.Empty<ConveyorUnit>();
            }
        }

        /// <summary>
        /// Gets the message deserializers that this plugin exposes.
        /// </summary>
        [NotNull]
        public virtual IEnumerable<MessageDeserializer> MessageDeserializers
        {
            get
            {
                return Enumerable.Empty<MessageDeserializer>();
            }
        }

        /// <summary>
        /// Gets the command serializers that this plugin exposes.
        /// </summary>
        [NotNull]
        public virtual IEnumerable<CommandSerializer> CommandSerializers
        {
            get
            {
                return Enumerable.Empty<CommandSerializer>();
            }
        }

        /// <summary>
        /// Gets the widgets of this plugin.
        /// </summary>
        [NotNull]
        public virtual IEnumerable<WidgetDescription> Widgets
        {
            get
            {
                return Enumerable.Empty<WidgetDescription>();
            }
        }

        /// <summary>
        /// Gets the custom action descriptions of this plugin.
        /// </summary>
        [NotNull]
        public virtual IEnumerable<ActionDescription> CustomActions
        {
            get
            {
                return Enumerable.Empty<ActionDescription>();
            }
        }

        /// <summary>
        /// Gets the custom action parameters descriptions of this plugin.
        /// </summary>
        [NotNull]
        public virtual IEnumerable<ParameterDescription> CustomActionParameters
        {
            get
            {
                return Enumerable.Empty<ParameterDescription>();
            }
        }

        /// <summary>
        /// Gets the plugin xaml resources to merge.
        /// </summary>
        [NotNull]
        public virtual IEnumerable<string> PluginXamlResourcesToMerge
        {
            get
            {
                return Enumerable.Empty<string>();
            }
        }

        /// <summary>
        /// Gets the custom serialization types of this plugin.
        /// </summary>
        [NotNull]
        public virtual IEnumerable<Type> CustomSerializationTypes
        {
            get
            {
                return Enumerable.Empty<Type>();
            }
        }

        /// <summary>
        /// Initializes this plugins with a specified <see cref="MessageConveyor"/> and <see cref="RootModel"/>.
        /// </summary>
        /// <param name="conveyor">The conveyor.</param>
        /// <param name="model">The model.</param>
        public abstract void Initialize([NotNull] MessageConveyor conveyor, [NotNull] RootModel model);
    }
}

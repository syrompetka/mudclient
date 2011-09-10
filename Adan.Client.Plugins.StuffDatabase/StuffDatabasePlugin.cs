// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StuffDatabasePlugin.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the StuffDatabasePlugin type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.StuffDatabase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Common.Conveyor;
    using Common.ConveyorUnits;
    using Common.MessageDeserializers;
    using Common.Model;
    using Common.Plugins;

    using CSLib.Net.Diagnostics;

    /// <summary>
    /// A <see cref="PluginBase"/> implementation to save stuff stats.
    /// </summary>
    [Export(typeof(PluginBase))]
    public sealed class StuffDatabasePlugin : PluginBase, IDisposable
    {
        private MessageDeserializer _deserializer;
        private ConveyorUnit _conveyorUnit;

        /// <summary>
        /// Gets the message deserializers that this plugin exposes.
        /// </summary>
        public override IEnumerable<MessageDeserializer> MessageDeserializers
        {
            get
            {
                return Enumerable.Repeat(_deserializer, 1);
            }
        }

        /// <summary>
        /// Gets the conveyor units that this plugin exposes.
        /// </summary>
        public override IEnumerable<ConveyorUnit> ConveyorUnits
        {
            get
            {
                return Enumerable.Repeat(_conveyorUnit, 1);
            }
        }

        /// <summary>
        /// Gets the required protocol version.
        /// </summary>
        public override int RequiredProtocolVersion
        {
            get
            {
                return 1;
            }
        }
        
        /// <summary>
        /// Initializes this plugins with a specified <see cref="MessageConveyor"/> and <see cref="RootModel"/>.
        /// </summary>
        /// <param name="conveyor">The conveyor.</param>
        /// <param name="model">The model.</param>
        public override void Initialize(MessageConveyor conveyor, RootModel model)
        {
            Assert.ArgumentNotNull(conveyor, "conveyor");
            Assert.ArgumentNotNull(model, "model");

            _deserializer = new LoreMessageDeserializer(conveyor);
            _conveyorUnit = new StuffDatabaseUnit(conveyor);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            _conveyorUnit.Dispose();
        }
    }
}

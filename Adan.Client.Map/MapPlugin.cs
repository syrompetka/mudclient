// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MapPlugin.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the MapPlugin type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Map
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Windows;

    using Common.Conveyor;
    using Common.ConveyorUnits;
    using Common.MessageDeserializers;
    using Common.Model;
    using Common.Plugins;

    using CSLib.Net.Diagnostics;
    using Adan.Client.Map.MessageDeserializers;
    using Adan.Client.Map.ConveyorUnits;

    /// <summary>
    /// A plugin to display zone map.
    /// </summary>
    [Export(typeof(PluginBase))]
    public sealed class MapPlugin : PluginBase, IDisposable
    {
        private readonly MapControl _mapControl = new MapControl();
        private ZoneManager _zoneManager;
        private CurrentRoomMessageDeserializer _messageDeserializer;
        private RouteUnit _routeUnit;

        /// <summary>
        /// Gets the widgets of this plugin.
        /// </summary>
        public override IEnumerable<WidgetDescription> Widgets
        {
            get
            {
                return Enumerable.Repeat(new WidgetDescription("Map", "Map", _mapControl, false), 1);
            }
        }

        /// <summary>
        /// Gets the message deserializers that this plugin exposes.
        /// </summary>
        public override IEnumerable<MessageDeserializer> MessageDeserializers
        {
            get
            {
                return Enumerable.Repeat(_messageDeserializer, 1);
            }
        }

        /// <summary>
        /// Gets the conveyor units that this plugin exposes.
        /// </summary>
        public override IEnumerable<ConveyorUnit> ConveyorUnits
        {
            get
            {
                return Enumerable.Repeat(_routeUnit, 1);
            }
        }

        /// <summary>
        /// Initializes this plugins with a specified <see cref="MessageConveyor"/> and <see cref="RootModel"/>.
        /// </summary>
        /// <param name="conveyor">The conveyor.</param>
        /// <param name="model">The model.</param>
        /// <param name="initializationStatusModel">The initialization status model.</param>
        /// <param name="mainWindow">The main window.</param>
        public override void Initialize(MessageConveyor conveyor, RootModel model, InitializationStatusModel initializationStatusModel, Window mainWindow)
        {
            Assert.ArgumentNotNull(conveyor, "conveyor");
            Assert.ArgumentNotNull(model, "model");
            Assert.ArgumentNotNull(initializationStatusModel, "initializationStatusModel");
            Assert.ArgumentNotNull(mainWindow, "mainWindow");

            initializationStatusModel.CurrentPluginName = "Map";
            MapDownloader.DownloadMaps(initializationStatusModel);

            var routeManger = new RouteManager(model, mainWindow);
            routeManger.LoadRoutes();
            _mapControl.RouteManager = routeManger;
            _routeUnit = new RouteUnit(conveyor, routeManger);
            _zoneManager = new ZoneManager(_mapControl, mainWindow, model, routeManger);
            _messageDeserializer = new CurrentRoomMessageDeserializer(conveyor, _zoneManager);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (_routeUnit != null)
            {
                _routeUnit.Dispose();
            }
        }
    }
}

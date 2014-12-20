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
    using Adan.Client.Common.ViewModel;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    /// <summary>
    /// A plugin to display zone map.
    /// </summary>
    [Export(typeof(PluginBase))]
    public sealed class MapPlugin : PluginBase
    {
        private readonly MapControl _mapControl;
        private ZoneManager _zoneManager;
        private CurrentRoomMessageDeserializer _messageDeserializer;
        private RouteUnit _routeUnit;
        private RouteManager _routeManager;

        /// <summary>
        /// 
        /// </summary>
        public MapPlugin()
        {
            _mapControl = new MapControl();
        }

        /// <summary>
        /// 
        /// </summary>
        public override string Name
        {
            get
            {
                return "Map";
            }
        }

        /// <summary>
        /// Gets the widgets of this plugin.
        /// </summary>
        public override IEnumerable<WidgetDescription> Widgets
        {
            get
            {

                return Enumerable.Repeat(new WidgetDescription("Map", "Map", _mapControl) 
                {
                    Left = (int)SystemParameters.PrimaryScreenWidth - 400,
                    Top = (int)SystemParameters.PrimaryScreenHeight - 400,
                    Height = 400, 
                    Width = 400,
                }, 1);
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
        /// 
        /// </summary>
        public override void Dispose()
        {
            if(_zoneManager != null)
                _zoneManager.Dispose();

            base.Dispose();
        }

        /// <summary>
        /// Initializes this plugins with a specified <see cref="MessageConveyor"/> and <see cref="RootModel"/>.
        /// </summary>
        /// <param name="initializationStatusModel">The initialization status model.</param>
        /// <param name="MainWindowEx">The main window.</param>
        public override void Initialize(InitializationStatusModel initializationStatusModel, Window MainWindowEx)
        {
            Assert.ArgumentNotNull(initializationStatusModel, "initializationStatusModel");
            Assert.ArgumentNotNull(MainWindowEx, "MainWindowEx");

            initializationStatusModel.CurrentPluginName = "Map";
            initializationStatusModel.PluginInitializationStatus = "Initializing";

            _messageDeserializer = new CurrentRoomMessageDeserializer();

            _routeManager = new RouteManager(MainWindowEx);
            _mapControl.RouteManager = _routeManager;
            _routeUnit = new RouteUnit(_routeManager);
            _zoneManager = new ZoneManager(_mapControl, MainWindowEx, _routeManager);
            initializationStatusModel.PluginInitializationStatus = "Routes loading";
            _routeManager.LoadRoutes();

            Task.Factory.StartNew(() => 
                {
                    try
                    {
                        MapDownloader.DownloadMaps();
                    }
                    catch(Exception) { }
                });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootModel"></param>
        public override void OnChangedOutputWindow(RootModel rootModel)
        {
            _routeManager.OutputWindowChanged(rootModel);
            _zoneManager.OutputWindowChanged(rootModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootModel"></param>
        public override void OnCreatedOutputWindow(RootModel rootModel)
        {
            _zoneManager.OutputWindowCreated(rootModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootModel"></param>
        public override void OnClosedOutputWindow(RootModel rootModel)
        {
            _zoneManager.OutputWindowClosed(rootModel.Uid);
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginHost.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the PluginHost type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;
    using System.Windows;

    using Common.Conveyor;
    using Common.Model;
    using Common.Plugins;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Adan.Client.Common.ViewModel;

    /// <summary>
    /// Class to host plugins.
    /// </summary>
    public sealed class PluginHost : IDisposable
    {
        private static PluginHost _instance;

        private readonly AggregateCatalog _catalog;
        private readonly CompositionContainer _container;

        private string currentOutputWindow = string.Empty;

        /// <summary>
        /// Prevents a default instance of the <see cref="PluginHost"/> class from being created.
        /// </summary>
        private PluginHost()
        {
            AllPlugins = new List<PluginBase>();
            Plugins = new List<PluginBase>();

            try
            {
                _catalog = new AggregateCatalog();
                _catalog.Catalogs.Add(new DirectoryCatalog("Plugins"));

                _container = new CompositionContainer(_catalog);
                _container.ComposeParts(this);
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// Gets the default instance of this class.
        /// </summary>
        [NotNull]
        public static PluginHost Instance
        {
            get
            {
                return _instance ?? (_instance = new PluginHost());
            }
        }

        /// <summary>
        /// Gets the all loaded plugins.
        /// </summary>
        [NotNull]
        [ImportMany(typeof(PluginBase))]
        public IList<PluginBase> AllPlugins
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the all loaded plugins.
        /// </summary>
        [NotNull]
        public IList<PluginBase> Plugins
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets all widgets.
        /// </summary>
        [NotNull]
        public IEnumerable<WidgetDescription> AllWidgets
        {
            get
            {
                return Plugins.SelectMany(plugin => plugin.Widgets);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputWindow"></param>
        public void OutputWindowCreated(OutputWindow outputWindow)
        {
            foreach (var plugin in Plugins)
            {
                try
                {
                    plugin.OnCreatedOutputWindow(outputWindow.RootModel);
                }
                catch (Exception)
                { }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputWindow"></param>
        public void OutputWindowChanged(OutputWindow outputWindow)
        {
            if (outputWindow.Uid != currentOutputWindow)
            {
                foreach (var plugin in Plugins)
                {
                    try
                    {
                        plugin.OnChangedOutputWindow(outputWindow.RootModel);
                    }
                    catch (Exception)
                    { }
                }

                currentOutputWindow = outputWindow.Uid;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputWindow"></param>
        public void OutputWindowClose(OutputWindow outputWindow)
        {
            foreach (var plugin in Plugins)
            {
                try
                {
                    plugin.OnClosedOutputWindow(outputWindow.RootModel);
                }
                catch (Exception)
                { }
            }
        }

        /// <summary>
        /// Applies the additional plugin merge dictionaries.
        /// </summary>
        public void ApplyAdditionalPluginMergeDictionaries()
        {
            foreach (var plugin in Plugins)
            {
                foreach (var resourceUrl in plugin.PluginXamlResourcesToMerge)
                {
                    Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri(resourceUrl, UriKind.Relative) });
                }
            }
        }

        /// <summary>
        /// Initializes the plugins.
        /// </summary>
        /// <param name="initializationStatusModel">The initialization status model.</param>
        /// <param name="mainWindow">The main window.</param>
        public void InitializePlugins([NotNull] InitializationStatusModel initializationStatusModel, [NotNull] Window mainWindow)
        {
            Assert.ArgumentNotNull(initializationStatusModel, "initializationStatusModel");
            Assert.ArgumentNotNull(mainWindow, "mainWindow");

            foreach (var plugin in AllPlugins)
            {
                try
                {
                    plugin.Initialize(initializationStatusModel, mainWindow);
                    Plugins.Add(plugin);

                    foreach (var actionDescription in plugin.CustomActions)
                    {
                        RootModel.AllActionDescriptions.Add(actionDescription);
                    }

                    foreach (var actionParameter in plugin.CustomActionParameters)
                    {
                        RootModel.AllParameterDescriptions.Add(actionParameter);
                    }

                    foreach (var conveyorUnit in plugin.ConveyorUnits)
                    {
                        MessageConveyor.AddConveyorUnit(conveyorUnit);
                    }

                    foreach (var commandSerializer in plugin.CommandSerializers)
                    {
                        MessageConveyor.AddCommandSerializer(commandSerializer);
                    }

                    foreach (var messageDeserializer in plugin.MessageDeserializers)
                    {
                        MessageConveyor.AddMessageDeserializer(messageDeserializer);
                    }
                }
                catch (Exception)
                { }
            }

            ApplyAdditionalPluginMergeDictionaries();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            try
            {
                foreach (var plugin in Plugins)
                    plugin.Dispose();

                if (_container != null)
                {
                    _container.Dispose();
                }

                if (_catalog != null)
                {
                    _catalog.Dispose();
                }
            }
            catch (Exception)
            { }

            GC.SuppressFinalize(this);
        }
    }
}

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

    /// <summary>
    /// Class to host plugins.
    /// </summary>
    public sealed class PluginHost : IDisposable
    {
        private readonly AggregateCatalog _catalog;
        private readonly CompositionContainer _container;
        private static PluginHost _instance;

        /// <summary>
        /// Prevents a default instance of the <see cref="PluginHost"/> class from being created.
        /// </summary>
        private PluginHost()
        {
            Plugins = new List<PluginBase>();
            _catalog = new AggregateCatalog();
            _catalog.Catalogs.Add(new DirectoryCatalog("Plugins"));

            _container = new CompositionContainer(_catalog);
            _container.ComposeParts(this);
        }

        /// <summary>
        /// Gets the default instance of this class.
        /// </summary>
        [NotNull]
        public static PluginHost Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PluginHost();
                }

                return _instance;
            }
        }

        /// <summary>
        /// Gets the all loaded plugins.
        /// </summary>
        [NotNull]
        [ImportMany(typeof(PluginBase))]
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
        /// Loads all avalable plugins.
        /// </summary>
        public void LoadPlugins()
        {
            using (var catalog = new AggregateCatalog())
            {
                catalog.Catalogs.Add(new DirectoryCatalog("Plugins"));

                using (var container = new CompositionContainer(catalog))
                {
                    container.ComposeParts(this);
                }
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
        /// <param name="conveyor">The conveyor.</param>
        /// <param name="model">The model.</param>
        public void InitializePlugins([NotNull] MessageConveyor conveyor, [NotNull] RootModel model)
        {
            Assert.ArgumentNotNull(conveyor, "conveyor");
            Assert.ArgumentNotNull(model, "model");

            foreach (var plugin in Plugins)
            {
                plugin.Initialize(conveyor, model);

                foreach (var actionDescription in plugin.CustomActions)
                {
                    model.AllActionDescriptions.Add(actionDescription);
                }

                foreach (var actionParameter in plugin.CustomActionParameters)
                {
                    model.AllParameterDescriptions.Add(actionParameter);
                }

                foreach (var conveyorUnit in plugin.ConveyorUnits)
                {
                    conveyor.AddConveyorUnit(conveyorUnit);
                }

                foreach (var commandSerializer in plugin.CommandSerializers)
                {
                    conveyor.AddCommandSerializer(commandSerializer);
                }

                foreach (var messageDeserializer in plugin.MessageDeserializers)
                {
                    conveyor.AddMessageDeserializer(messageDeserializer);
                }
            }

            ApplyAdditionalPluginMergeDictionaries();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (_container != null)
            {
                _container.Dispose();
            }

            if (_catalog != null)
            {
                _catalog.Dispose();
            }
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputToAdditionalWindowPlugin.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the OutputToAdditionalWindowPlugin type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.OutputWindow
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Windows;

    using Common.Conveyor;
    using Common.Model;
    using Common.Plugins;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Adan.Client.Plugins.OutputWindow.Models.ConveyorUnits;
    using Adan.Client.Plugins.OutputWindow.Model.Actions;
    using Adan.Client.Plugins.OutputWindow.Model;
    using Adan.Client.Common.ViewModel;

    /// <summary>
    /// A <see cref="PluginBase"/> implementation to add additional output window.
    /// </summary>
    [Export(typeof(PluginBase))]
    public sealed class OutputToAdditionalWindowPlugin : PluginBase, IDisposable
    {
        private readonly AdditionalOutputWindow _additionalOutputWindowControl = new AdditionalOutputWindow();
        private WidgetDescription _widget;

        private OutputToAdditionalWindowConveyorUnit _conveyorUnit;

        /// <summary>
        /// Gets the conveyor units that this plugin exposes.
        /// </summary>
        public override IEnumerable<Common.ConveyorUnits.ConveyorUnit> ConveyorUnits
        {
            get
            {
                return Enumerable.Repeat(_conveyorUnit, 1);
            }
        }

        /// <summary>
        /// Gets the widgets of this plugin.
        /// </summary>
        public override IEnumerable<WidgetDescription> Widgets
        {
            get
            {
                return Enumerable.Repeat(_widget, 1);
            }
        }

        /// <summary>
        /// Gets the custom action description of this plugin.
        /// </summary>
        public override IEnumerable<ActionDescription> CustomActions
        {
            get
            {
                return Enumerable.Repeat(new OutputToAdditionalWindowActionDescription(RootModel.AllParameterDescriptions, RootModel.AllActionDescriptions), 1);
            }
        }

        /// <summary>
        /// Gets the plugin xaml resources to merge.
        /// </summary>
        public override IEnumerable<string> PluginXamlResourcesToMerge
        {
            get
            {
                return Enumerable.Repeat(@"/Adan.Client.Plugins.OutputWindow;component/OutputToAdditionalWindowActionEditingTemplate.xaml", 1);
            }
        }

        /// <summary>
        /// Gets the custom serialization types of this plugin.
        /// </summary>
        public override IEnumerable<Type> CustomSerializationTypes
        {
            get
            {
                return Enumerable.Repeat(typeof(OutputToAdditionalWindowAction), 1);
            }
        }

        /// <summary>
        /// Initializes this plugins with a specified <see cref="MessageConveyor"/> and <see cref="RootModel"/>.
        /// </summary>
        /// <param name="initializationStatusModel">The initialization status model.</param>
        /// <param name="mainWindow">The main window.</param>
        public override void Initialize(InitializationStatusModel initializationStatusModel, [NotNull] Window mainWindow)
        {
            Assert.ArgumentNotNull(initializationStatusModel, "initializationStatusModel");

            initializationStatusModel.CurrentPluginName = "Additional window";
            initializationStatusModel.PluginInitializationStatus = "Initializing";

            _widget = new WidgetDescription("AdditionalOutputWindow", "Additional output", _additionalOutputWindowControl, false);
            _conveyorUnit = new OutputToAdditionalWindowConveyorUnit(_additionalOutputWindowControl);
        }
    }
}

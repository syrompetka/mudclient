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

    using Common.Conveyor;
    using Common.Model;
    using Common.Plugins;

    using CSLib.Net.Diagnostics;

    /// <summary>
    /// A <see cref="PluginBase"/> implementation to add additional output window.
    /// </summary>
    [Export(typeof(PluginBase))]
    public sealed class OutputToAdditionalWindowPlugin : PluginBase, IDisposable
    {
        private WidgetDescription _widget;
        private OutputToAdditionalWindowConveyorUnit _conveyorUnit;
        private RootModel _rootModel;

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
                return Enumerable.Repeat(new OutputToAdditionalWindowActionDescription(_rootModel.AllParameterDescriptions, _rootModel.AllActionDescriptions), 1);
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
        /// <param name="conveyor">The conveyor.</param>
        /// <param name="model">The model.</param>
        public override void Initialize(MessageConveyor conveyor, RootModel model)
        {
            Assert.ArgumentNotNull(conveyor, "conveyor");
            Assert.ArgumentNotNull(model, "model");

            var control = new AdditionalOutputWindow();
            _widget = new WidgetDescription("AdditionalOutputWindow", "Additional output", control, "ImageSource_OutputToAditionalWindowWidgetIcond");
            _conveyorUnit = new OutputToAdditionalWindowConveyorUnit(conveyor, control);
            _rootModel = model;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (_conveyorUnit != null)
            {
                _conveyorUnit.Dispose();
            }
        }
    }
}

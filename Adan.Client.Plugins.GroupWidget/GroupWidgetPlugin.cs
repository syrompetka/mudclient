// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupWidgetPlugin.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the GroupWidgetPlugin type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.GroupWidget
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

    using Properties;

    using ViewModel;

    /// <summary>
    /// <see cref="PluginBase"/> implementation to display group widget.
    /// </summary>
    [Export(typeof(PluginBase))]
    public sealed class GroupWidgetPlugin : PluginBase, IDisposable
    {
        private readonly GroupWidgetControl _groupWidgetControl = new GroupWidgetControl();
        private GroupStatusViewModel _viewModel;
        private MessageDeserializer _deserializer;
        private ConveyorUnit _conveyorUnit;
        private RootModel _rootModel;

        /// <summary>
        /// Gets the widgets of this plugin.
        /// </summary>
        public override IEnumerable<WidgetDescription> Widgets
        {
            get
            {
                return Enumerable.Repeat(new WidgetDescription("GroupWidget", Resources.Group, _groupWidgetControl), 1);
            }
        }

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
        /// Gets a value indicating whether this plugin has options dialog.
        /// </summary>
        /// <value>
        /// <c>true</c> if this plugin has options dialog; otherwise, <c>false</c>.
        /// </value>
        public override bool HasOptions
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the options menu item text.
        /// </summary>
        public override string OptionsMenuItemText
        {
            get
            {
                return Resources.GroupWidgetOptions;
            }
        }

        /// <summary>
        /// Gets the plugin xaml resources to merge.
        /// </summary>
        public override IEnumerable<string> PluginXamlResourcesToMerge
        {
            get
            {
                return Enumerable.Repeat(@"/Adan.Client.Plugins.GroupWidget;component/ParameterEditingTemplates.xaml", 1);
            }
        }

        /// <summary>
        /// Gets the custom action parameters descriptions of this plugin.
        /// </summary>
        public override IEnumerable<ParameterDescription> CustomActionParameters
        {
            get
            {
                return Enumerable.Repeat(new SelectedGroupMateParameterDescription(_rootModel.AllParameterDescriptions), 1);
            }
        }

        /// <summary>
        /// Gets the custom serialization types of this plugin.
        /// </summary>
        public override IEnumerable<Type> CustomSerializationTypes
        {
            get
            {
                return Enumerable.Repeat(typeof(SelectedGroupMateParameter), 1);
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

            _rootModel = model;
            _deserializer = new GroupStatusMessageDeserializer(conveyor);
            _viewModel = new GroupStatusViewModel(model);
            _groupWidgetControl.DataContext = _viewModel;
            _conveyorUnit = new GroupStatusUnit(conveyor, _groupWidgetControl, _viewModel);
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

        /// <summary>
        /// Shows the options dialog.
        /// </summary>
        /// <param name="parentWindow">The parent window.</param>
        public override void ShowOptionsDialog(Window parentWindow)
        {
            Assert.ArgumentNotNull(parentWindow, "parentWindow");

            var displayedAffects = Settings.Default.GroupWidgetAffects.Select(af => Constants.AllAffects.First(a => a.Name == af));
            var allAffects = Constants.AllAffects.Except(displayedAffects);
            var optionsViewModel = new GroupWidgetOptionsViewModel(Resources.GroupWidgetOptions, allAffects, displayedAffects);
            var window = new OptionsDialog { DataContext = optionsViewModel, Owner = parentWindow };
            var result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                Settings.Default.GroupWidgetAffects = optionsViewModel.DisplayedAffects.Select(af => af.Name).ToArray();
                Settings.Default.Save();
                _viewModel.ReloadDisplayedAffects();
            }
        }
    }
}

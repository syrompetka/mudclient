// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MonstersWidgetPlugin.cs" company="Adamand MUD">
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
    using Adan.Client.Plugins.GroupWidget.Model.ActionParameters;
    using Adan.Client.Plugins.GroupWidget.Model.ParameterDescriptions;
    using Adan.Client.Plugins.GroupWidget.MessageDeserializers;
    using Adan.Client.Plugins.GroupWidget.ConveyorUnits;

    /// <summary>
    /// <see cref="PluginBase"/> implementation to display monsters widget.
    /// </summary>
    [Export(typeof(PluginBase))]
    public sealed class MonstersWidgetPlugin : PluginBase, IDisposable
    {
        private readonly MonstersWidgetControl _monstersWidgetControl;
        private readonly RoomMonstersViewModel _viewModel;
        private MessageDeserializer _deserializer;
        private ConveyorUnit _conveyorUnit;
        private RootModel _rootModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonstersWidgetPlugin"/> class.
        /// </summary>
        public MonstersWidgetPlugin()
        {
            _viewModel = new RoomMonstersViewModel();
            _monstersWidgetControl = new MonstersWidgetControl { DataContext = _viewModel };
        }

        /// <summary>
        /// Gets the widgets of this plugin.
        /// </summary>
        public override IEnumerable<WidgetDescription> Widgets
        {
            get
            {
                return Enumerable.Repeat(new WidgetDescription("MonstersWidget", Resources.Monsters, _monstersWidgetControl, true), 1);
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
                return Resources.MonstersWidgetOptions;
            }
        }

        /// <summary>
        /// Gets the custom action parameters descriptions of this plugin.
        /// </summary>
        public override IEnumerable<ParameterDescription> CustomActionParameters
        {
            get
            {
                return Enumerable.Repeat(new SelectedMonsterParameterDescription(_rootModel.AllParameterDescriptions), 1);
            }
        }

        /// <summary>
        /// Gets the custom serialization types of this plugin.
        /// </summary>
        public override IEnumerable<Type> CustomSerializationTypes
        {
            get
            {
                return Enumerable.Repeat(typeof(SelectedMonsterParameter), 1);
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

            initializationStatusModel.CurrentPluginName = Resources.Monsters;
            _rootModel = model;
            _deserializer = new RoomMonstersMessageDeserializer(conveyor);
            _viewModel.UpdateRootModel(model);
            _conveyorUnit = new RoomMonstersUnit(conveyor, _monstersWidgetControl, _viewModel);
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

            var displayedAffects = Settings.Default.MonsterAffects.Select(af => Constants.AllAffects.First(a => a.Name == af));
            var allAffects = Constants.AllAffects.Except(displayedAffects);
            var optionsViewModel = new GroupWidgetOptionsViewModel(Resources.MonstersWidgetOptions, allAffects, displayedAffects);
            var window = new OptionsDialog { DataContext = optionsViewModel, Owner = parentWindow };
            var result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                Settings.Default.MonsterAffects = optionsViewModel.DisplayedAffects.Select(af => af.Name).ToArray();
                Settings.Default.Save();
                _viewModel.ReloadDisplayedAffects();
            }
        }
    }
}

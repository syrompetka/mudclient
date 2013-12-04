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
    using Adan.Client.Common.ViewModel;
    using CSLib.Net.Annotations;

    /// <summary>
    /// <see cref="PluginBase"/> implementation to display monsters widget.
    /// </summary>
    [Export(typeof(PluginBase))]
    public sealed class MonstersWidgetPlugin : PluginBase, IDisposable
    {
        private MonstersWidgetControl _monstersWidgetControl;
        private RoomMonstersViewModel _viewModel;

        private MonstersManager _monstersManager;
        private MessageDeserializer _deserializer;
        private ConveyorUnit _conveyorUnit;

        /// <summary>
        /// 
        /// </summary>
        public MonstersWidgetPlugin()
        {
            _viewModel = new RoomMonstersViewModel();
            _monstersWidgetControl = new MonstersWidgetControl() { DataContext = _viewModel };
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
                return Enumerable.Repeat(new SelectedMonsterParameterDescription(RootModel.AllParameterDescriptions), 1);
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
        /// <param name="initializationStatusModel">The initialization status model.</param>
        /// <param name="mainWindow">The main window.</param>
        public override void Initialize([NotNull] InitializationStatusModel initializationStatusModel, [NotNull] Window mainWindow)
        {
            Assert.ArgumentNotNull(initializationStatusModel, "initializationStatusModel");
            Assert.ArgumentNotNull(mainWindow, "mainWindow");

            initializationStatusModel.CurrentPluginName = Resources.Monsters;
            initializationStatusModel.PluginInitializationStatus = "Initializing";

            _deserializer = new RoomMonstersMessageDeserializer();
            _monstersManager = new MonstersManager(_monstersWidgetControl);
            _conveyorUnit = new RoomMonstersUnit(_monstersWidgetControl);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootModel"></param>
        /// <param name="uid"></param>
        public override void OnCreatedOutputWindow(RootModel rootModel, string uid)
        {
            _monstersManager.OutputWindowCreated(rootModel, uid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootModel"></param>
        /// <param name="uid"></param>
        public override void OnChangedOutputWindow(RootModel rootModel, string uid)
        {
            _monstersManager.OutputWindowChanged(uid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootModel"></param>
        /// <param name="uid"></param>
        public override void OnClosedOutputWindow(RootModel rootModel, string uid)
        {
            _monstersManager.OutputWindowClosed(uid);
        }

        /// <summary>
        /// Shows the options dialog.
        /// </summary>
        /// <param name="parentWindow">The parent window.</param>
        public override void ShowOptionsDialog([NotNull] Window parentWindow)
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

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

    using Adan.Client.Common.Conveyor;
    using Adan.Client.Common.ConveyorUnits;
    using Adan.Client.Common.MessageDeserializers;
    using Adan.Client.Common.Model;
    using Adan.Client.Common.Plugins;
    using Adan.Client.Common.ViewModel;

    using CSLib.Net.Diagnostics;

    using Adan.Client.Plugins.GroupWidget.Properties;
    using Adan.Client.Plugins.GroupWidget.ViewModel;
    using Adan.Client.Plugins.GroupWidget.MessageDeserializers;
    using Adan.Client.Plugins.GroupWidget.ConveyorUnits;
    using Adan.Client.Plugins.GroupWidget.Model.ActionParameters;
    using Adan.Client.Plugins.GroupWidget.Model.ParameterDescriptions;
    using CSLib.Net.Annotations;

    /// <summary>
    /// <see cref="PluginBase"/> implementation to display group widget.
    /// </summary>
    [Export(typeof(PluginBase))]
    public sealed class GroupWidgetPlugin : PluginBase, IDisposable
    {
        private GroupStatusViewModel _viewModel;
        private GroupWidgetControl _groupWidgetControl;

        private GroupManager _groupManager;
        private MessageDeserializer _deserializer;
        private ConveyorUnit _conveyorUnit;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupWidgetPlugin"/> class.
        /// </summary>
        public GroupWidgetPlugin()
        {
            _viewModel = new GroupStatusViewModel();
            _groupWidgetControl = new GroupWidgetControl() { DataContext = _viewModel };
        }

        /// <summary>
        /// Gets the widgets of this plugin.
        /// </summary>
        public override IEnumerable<WidgetDescription> Widgets
        {
            get
            {
                return Enumerable.Repeat(new WidgetDescription("GroupWidget", Resources.Group, _groupWidgetControl, true), 1);
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
                return Enumerable.Repeat(new SelectedGroupMateParameterDescription(RootModel.AllParameterDescriptions), 1);
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
        /// <param name="initializationStatusModel">The initialization status model.</param>
        /// <param name="mainWindow">The main window.</param>
        public override void Initialize([NotNull] InitializationStatusModel initializationStatusModel, [NotNull] Window mainWindow)
        {
            Assert.ArgumentNotNull(initializationStatusModel, "initializationStatusModel");
            Assert.ArgumentNotNull(mainWindow, "mainWindow");

            initializationStatusModel.CurrentPluginName = Resources.Group;
            initializationStatusModel.PluginInitializationStatus = "Initializing";

            _deserializer = new GroupStatusMessageDeserializer();
            _groupManager = new GroupManager(_groupWidgetControl);
            _conveyorUnit = new GroupStatusUnit(_groupWidgetControl);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootModel"></param>
        /// <param name="uid"></param>
        public override void OnChangedOutputWindow([NotNull] RootModel rootModel, [NotNull] string uid)
        {
            Assert.ArgumentNotNull(rootModel, "rootModel");
            Assert.ArgumentNotNullOrWhiteSpace(uid, "UID");

            _groupManager.OutputWindowChanged(uid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootModel"></param>
        /// <param name="uid"></param>
        public override void OnCreatedOutputWindow([NotNull] RootModel rootModel, [NotNull] string uid)
        {
            Assert.ArgumentNotNull(rootModel, "rootModel");
            Assert.ArgumentNotNullOrWhiteSpace(uid, "uid");

            _groupManager.OutputWindowCreated(rootModel, uid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootModel"></param>
        /// <param name="uid"></param>
        public override void OnClosedOutputWindow([NotNull] RootModel rootModel, [NotNull] string uid)
        {
            Assert.ArgumentNotNull(rootModel, "rootModel");
            Assert.ArgumentNotNullOrWhiteSpace(uid, "uid");

            _groupManager.OutputWindowClosed(uid);
        }

        /// <summary>
        /// Shows the options dialog.
        /// </summary>
        /// <param name="parentWindow">The parent window.</param>
        public override void ShowOptionsDialog([NotNull] Window parentWindow)
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

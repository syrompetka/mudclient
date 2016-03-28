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
    using Common.ViewModel;
    using ConveyorUnits;
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using MessageDeserializers;
    using Model.ActionParameters;
    using Model.ParameterDescriptions;
    using Properties;
    using ViewModel;

    /// <summary>
    /// <see cref="PluginBase"/> implementation to display group widget.
    /// </summary>
    [Export(typeof(PluginBase))]
    public sealed class GroupWidgetPlugin : PluginBase, IDisposable
    {
        private readonly GroupStatusViewModel _viewModel;
        private readonly GroupWidgetControl _groupWidgetControl;

        private GroupManager _groupManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupWidgetPlugin"/> class.
        /// </summary>
        public GroupWidgetPlugin()
        {
            _viewModel = new GroupStatusViewModel();
            _groupWidgetControl = new GroupWidgetControl { DataContext = _viewModel };
        }

        /// <summary>
        /// 
        /// </summary>
        public override string Name
        {
            get
            {
                return "Group";
            }
        }

        /// <summary>
        /// Gets the widgets of this plugin.
        /// </summary>
        public override IEnumerable<WidgetDescription> Widgets
        {
            get
            {
                return Enumerable.Repeat(new WidgetDescription("GroupWidget", Resources.Group, _groupWidgetControl)
                {
                    Left = 400,
                    Height = 450,
                    Width = 300
                }, 1);
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

        public override void InitializeConveyor(MessageConveyor conveyor)
        {
            conveyor.AddConveyorUnit(new GroupStatusUnit(_groupWidgetControl, conveyor));
            conveyor.AddMessageDeserializer(new GroupStatusMessageDeserializer(conveyor));
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
        /// <param name="MainWindowEx">The main window.</param>
        public override void Initialize([NotNull] InitializationStatusModel initializationStatusModel, [NotNull] Window MainWindowEx)
        {
            Assert.ArgumentNotNull(initializationStatusModel, "initializationStatusModel");
            Assert.ArgumentNotNull(MainWindowEx, "MainWindowEx");

            initializationStatusModel.CurrentPluginName = Resources.Group;
            initializationStatusModel.PluginInitializationStatus = "Initializing";

            _groupManager = new GroupManager(_groupWidgetControl);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootModel"></param>
        public override void OnChangedOutputWindow([NotNull] RootModel rootModel)
        {
            Assert.ArgumentNotNull(rootModel, "rootModel");

            _groupManager.OutputWindowChanged(rootModel.Uid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootModel"></param>
        public override void OnCreatedOutputWindow([NotNull] RootModel rootModel)
        {
            Assert.ArgumentNotNull(rootModel, "rootModel");

            _groupManager.OutputWindowCreated(rootModel, rootModel.Uid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootModel"></param>
        public override void OnClosedOutputWindow([NotNull] RootModel rootModel)
        {
            Assert.ArgumentNotNull(rootModel, "rootModel");

            _groupManager.OutputWindowClosed(rootModel.Uid);
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

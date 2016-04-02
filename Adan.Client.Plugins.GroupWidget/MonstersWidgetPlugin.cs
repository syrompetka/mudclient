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
    /// <see cref="PluginBase"/> implementation to display monsters widget.
    /// </summary>
    [Export(typeof(PluginBase))]
    public sealed class MonstersWidgetPlugin : PluginBase
    {
        private readonly MonstersWidgetControl _monstersWidgetControl;
        private readonly RoomMonstersViewModel _viewModel;

        private MonstersManager _monstersManager;

        /// <summary>
        /// 
        /// </summary>
        public MonstersWidgetPlugin()
        {
            _viewModel = new RoomMonstersViewModel();
            _monstersWidgetControl = new MonstersWidgetControl { DataContext = _viewModel };
        }

        /// <summary>
        /// 
        /// </summary>
        public override string Name
        {
            get
            {
                return "Monsters";
            }
        }

        /// <summary>
        /// Gets the widgets of this plugin.
        /// </summary>
        public override IEnumerable<WidgetDescription> Widgets
        {
            get
            {
                return Enumerable.Repeat(new WidgetDescription("MonstersWidget", Resources.Monsters, _monstersWidgetControl)
                {
                    ResizeToContent = true,
                    Left = (int)SystemParameters.PrimaryScreenWidth - 400 - 400,
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
                return Resources.MonstersWidgetOptions;
            }
        }

        public override void InitializeConveyor(MessageConveyor conveyor)
        {
            conveyor.AddConveyorUnit(new RoomMonstersUnit(_monstersWidgetControl, conveyor));
            conveyor.AddMessageDeserializer(new RoomMonstersMessageDeserializer(conveyor));
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
        /// <param name="MainWindowEx">The main window.</param>
        public override void Initialize([NotNull] InitializationStatusModel initializationStatusModel, [NotNull] Window MainWindowEx)
        {
            Assert.ArgumentNotNull(initializationStatusModel, "initializationStatusModel");
            Assert.ArgumentNotNull(MainWindowEx, "MainWindowEx");

            initializationStatusModel.CurrentPluginName = Resources.Monsters;
            initializationStatusModel.PluginInitializationStatus = "Initializing";
            _monstersManager = new MonstersManager(_monstersWidgetControl);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootModel"></param>
        public override void OnCreatedOutputWindow(RootModel rootModel)
        {
            _monstersManager.OutputWindowCreated(rootModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootModel"></param>
        public override void OnChangedOutputWindow(RootModel rootModel)
        {
            _monstersManager.OutputWindowChanged(rootModel.Uid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootModel"></param>
        public override void OnClosedOutputWindow(RootModel rootModel)
        {
            _monstersManager.OutputWindowClosed(rootModel.Uid);
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
            var optionsViewModel = new GroupWidgetOptionsViewModel(Resources.MonstersWidgetOptions, allAffects, displayedAffects, Settings.Default.MonsterDisplayAffectsCount, Settings.Default.MonsterDisplayNumber, false, false);
            var window = new OptionsDialog { DataContext = optionsViewModel, Owner = parentWindow };
            var result = window.ShowDialog();
            if (result.HasValue && result.Value)
            {
                Settings.Default.MonsterAffects = optionsViewModel.DisplayedAffects.Select(af => af.Name).ToArray();
                Settings.Default.MonsterDisplayAffectsCount = optionsViewModel.DisplayedAffectsCount;
                Settings.Default.MonsterDisplayNumber = optionsViewModel.DisplayNumber;
                Settings.Default.Save();
                _viewModel.ReloadDisplayedAffects();
            }
        }
    }
}

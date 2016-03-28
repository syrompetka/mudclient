namespace Adan.Client.Plugins.OutputWindow
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Windows;
    using Common.Conveyor;
    using Common.ConveyorUnits;
    using Common.Model;
    using Common.Plugins;
    using Common.ViewModel;
    using ConveyorUnits;
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Model;
    using Model.Actions;

    /// <summary>
    /// A <see cref="PluginBase"/> implementation to add additional output window.
    /// </summary>
    [Export(typeof(PluginBase))]
    public sealed class OutputToAdditionalWindowPlugin : PluginBase
    {
        private readonly AdditionalOutputWindow _additionalOutputWindowControl;
        private AdditionalOutputWindowManager _manager;
        private WidgetDescription _widget;
        
        /// <summary>
        /// 
        /// </summary>
        public override string Name
        {
            get 
            {
                return "OutputAdditionalWindow";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public OutputToAdditionalWindowPlugin()
        {
            _additionalOutputWindowControl = new AdditionalOutputWindow();
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
        /// <param name="MainWindowEx">The main window.</param>
        public override void Initialize(InitializationStatusModel initializationStatusModel, [NotNull] Window MainWindowEx)
        {
            Assert.ArgumentNotNull(initializationStatusModel, "initializationStatusModel");

            initializationStatusModel.CurrentPluginName = "Additional window";
            initializationStatusModel.PluginInitializationStatus = "Initializing";
            
            _manager = new AdditionalOutputWindowManager(_additionalOutputWindowControl);

            _widget = new WidgetDescription("AdditionalOutputWindow", "Additional output", _additionalOutputWindowControl)
            {
                Left = (int)SystemParameters.PrimaryScreenWidth - 400,
                Height = 300,
                Width = 400
            };
        }

        public override void InitializeConveyor(MessageConveyor conveyor)
        {
            conveyor.AddConveyorUnit(new OutputToAdditionalWindowConveyorUnit(_manager, conveyor));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootModel"></param>
        public override void OnCreatedOutputWindow(RootModel rootModel)
        {
            _manager.OutputWindowCreated(rootModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootModel"></param>
        public override void OnChangedOutputWindow(RootModel rootModel)
        {
            _manager.OutputWindowChanged(rootModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootModel"></param>
        public override void OnClosedOutputWindow(RootModel rootModel)
        {
            _manager.OutputWindowClosed(rootModel);
        }
    }
}

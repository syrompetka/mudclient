namespace Adan.Client.Plugins.Ticker
{
    using System.ComponentModel.Composition;
    using System.Windows;
    using CSLib.Net.Diagnostics;
    using Common.Conveyor;
    using Common.Model;
    using Common.Plugins;
    using Common.ViewModel;
    using ConveyorUnits;

    /// <summary>
    /// A <see cref="PluginBase"/> implementation to display ticks.
    /// </summary>
    [Export(typeof(PluginBase))]
    public sealed class TickerPlugin : PluginBase
    {
        public override string Name
        {
            get
            {
                return "Ticker";
            }
        }

        public override void InitializeConveyor(MessageConveyor conveyor)
        {
            conveyor.AddConveyorUnit(new TickerUnit(conveyor));
        }

        /// <summary>
        /// Initializes this plugins with a specified <see cref="MessageConveyor"/> and <see cref="RootModel"/>.
        /// </summary>
        /// <param name="initializationStatusModel">The initialization status model.</param>
        /// <param name="MainWindowEx">The main window.</param>
        public override void Initialize(InitializationStatusModel initializationStatusModel, Window MainWindowEx)
        {
            Assert.ArgumentNotNull(initializationStatusModel, "initializationStatusModel");

            initializationStatusModel.CurrentPluginName = "Ticker";
            initializationStatusModel.PluginInitializationStatus = "Initializing";
        }
    }
}

namespace Adan.Client.Plugins.Statistics
{
    using System.ComponentModel.Composition;
    using System.Windows;
    using Common.Conveyor;
    using Common.Plugins;
    using Common.ViewModel;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// A <see cref="PluginBase"/> implementation to calculate fight statistics.
    /// </summary>
    [Export(typeof(PluginBase))]
    public sealed class StatisticsPlugin : PluginBase
    {
        public override string Name => "Statistics";

        public override void InitializeConveyor(MessageConveyor conveyor)
        {
            conveyor.AddConveyorUnit(new StatisticsConveyourUnit(conveyor), true);
        }

        public override void Initialize(InitializationStatusModel initializationStatusModel, Window MainWindowEx)
        {
            Assert.ArgumentNotNull(initializationStatusModel, "initializationStatusModel");

            initializationStatusModel.CurrentPluginName = "Statistics";
            initializationStatusModel.PluginInitializationStatus = "Initializing";
        }
    }
}

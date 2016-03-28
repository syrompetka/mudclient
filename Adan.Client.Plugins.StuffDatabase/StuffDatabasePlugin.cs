namespace Adan.Client.Plugins.StuffDatabase
{
    using System.ComponentModel.Composition;
    using System.Windows;
    using Common.Conveyor;
    using Common.Model;
    using Common.Plugins;
    using Common.ViewModel;
    using ConveyorUnits;
    using CSLib.Net.Diagnostics;
    using MessageDeserializers;

    /// <summary>
    /// A <see cref="PluginBase"/> implementation to save stuff stats.
    /// </summary>
    [Export(typeof(PluginBase))]
    public sealed class StuffDatabasePlugin : PluginBase
    {
        public override string Name
        {
            get 
            {
                return "StuffDatabase";
            }
        }
        
        public override void InitializeConveyor(MessageConveyor conveyor)
        {
            conveyor.AddConveyorUnit(new StuffDatabaseUnit(conveyor));
            conveyor.AddMessageDeserializer(new LoreMessageDeserializer(conveyor));
        }

        /// <summary>
        /// Initializes this plugins with a specified <see cref="MessageConveyor"/> and <see cref="RootModel"/>.
        /// </summary>
        /// <param name="initializationStatusModel">The initialization status model.</param>
        /// <param name="MainWindowEx">The main window.</param>
        public override void Initialize(InitializationStatusModel initializationStatusModel, Window MainWindowEx)
        {
            Assert.ArgumentNotNull(initializationStatusModel, "initializationStatusModel");

            initializationStatusModel.CurrentPluginName = "Stuff database";
            initializationStatusModel.PluginInitializationStatus = "Initializing";
        }
    }
}

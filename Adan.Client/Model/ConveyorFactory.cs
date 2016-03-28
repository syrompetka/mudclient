using Adan.Client.CommandSerializers;
using Adan.Client.Common.Conveyor;
using Adan.Client.Common.Model;
using Adan.Client.Common.Settings;
using Adan.Client.ConveyorUnits;
using Adan.Client.MessageDeserializers;

namespace Adan.Client.Model
{
    using System.Collections.Generic;

    public sealed class ConveyorFactory
    {
        public static MessageConveyor CreateNew(string name, IList<RootModel> allRootModels)
        {
            var conveyor = MessageConveyor.CreateNew(name, SettingsHolder.Instance.GetProfile(name), allRootModels);
            InitializeConveyorUnits(conveyor);

            return conveyor;
        }

        public static MessageConveyor CreateNew(RootModel rootModel)
        {
            var conveyor = MessageConveyor.CreateNew(rootModel);
            InitializeConveyorUnits(conveyor);
            return conveyor;
        }

        private static void InitializeConveyorUnits(MessageConveyor conveyor)
        {
            conveyor.AddCommandSerializer(new TextCommandSerializer(conveyor));

            //Initialize conveyor with all deserializations
            conveyor.AddMessageDeserializer(new TextMessageDeserializer(conveyor));
            conveyor.AddMessageDeserializer(new ProtocolVersionMessageDeserializer(conveyor));

            //Initialize conveyor with message handlers. Handlers added in order processing
            conveyor.AddConveyorUnit(new CommandSeparatorUnit(conveyor));
            conveyor.AddConveyorUnit(new CommandsFromUserLineUnit(conveyor));
            conveyor.AddConveyorUnit(new VariableReplaceUnit(conveyor));
            conveyor.AddConveyorUnit(new CommandMultiplierUnit(conveyor));
            conveyor.AddConveyorUnit(new SubstitutionUnit(conveyor));
            conveyor.AddConveyorUnit(new TriggerUnit(conveyor));
            conveyor.AddConveyorUnit(new AliasUnit(conveyor));
            conveyor.AddConveyorUnit(new HotkeyUnit(conveyor));
            conveyor.AddConveyorUnit(new HighlightUnit(conveyor));
            conveyor.AddConveyorUnit(new LoggingUnit(conveyor));
            conveyor.AddConveyorUnit(new ShowMainOutputUnit(conveyor));
            conveyor.AddConveyorUnit(new SendToWindowUnit(conveyor));
            conveyor.AddConveyorUnit(new ToggleFullScreenModeUnit(conveyor));

            PluginHost.Instance.InitializeConveyor(conveyor);

            //Add remaining message handlers which should to process message last
            conveyor.AddConveyorUnit(new ProtocolVersionUnit(conveyor));
            conveyor.AddConveyorUnit(new CommandRepeaterUnit(conveyor));
            conveyor.AddConveyorUnit(new CapForLineCommandUnit(conveyor));
            conveyor.AddConveyorUnit(new ConnectionUnit(conveyor));
        }
    }
}

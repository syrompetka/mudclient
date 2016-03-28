
namespace Adan.Client.ConveyorUnits
{
    using System.Collections.Generic;
    using System.Linq;
    using Common.ConveyorUnits;
    using Common.Messages;
    using Common.Conveyor;
    using CSLib.Net.Diagnostics;
    using Messages;

    /// <summary>
    /// A <see cref="ConveyorUnit"/> to handle protocol version messages.
    /// </summary>
    public class ProtocolVersionUnit : ConveyorUnit
    {
        public ProtocolVersionUnit(MessageConveyor conveyor) : base(conveyor)
        {
        }

        /// <summary>
        /// Gets a set of message types that this unit can handle.
        /// </summary>
        public override IEnumerable<int> HandledMessageTypes
        {
            get
            {
                return Enumerable.Repeat(BuiltInMessageTypes.ProtocolVersionMessage, 1);
            }
        }

        /// <summary>
        /// Gets a set of command types that this unit can handle.
        /// </summary>
        public override IEnumerable<int> HandledCommandTypes
        {
            get
            {
                return Enumerable.Empty<int>();
            }
        }
        
        public override void HandleMessage(Message message)
        {
            Assert.ArgumentNotNull(message, "message");

            var protocolVersionMessage = message as ProtocolVersionMessage;
            if (protocolVersionMessage != null)
            {
                Conveyor.RootModel.ServerVersion = protocolVersionMessage.Version;

                message.Handled = true;
            }
        }
    }
}

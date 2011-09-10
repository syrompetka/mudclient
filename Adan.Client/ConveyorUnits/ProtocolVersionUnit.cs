// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtocolVersionUnit.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ProtocolVersionUnit type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ConveyorUnits
{
    using System.Collections.Generic;
    using System.Linq;

    using Commands;

    using Common.Conveyor;
    using Common.ConveyorUnits;
    using Common.Messages;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Messages;

    using Properties;

    /// <summary>
    /// A <see cref="ConveyorUnit"/> to handle protocol version messages.
    /// </summary>
    public class ProtocolVersionUnit : ConveyorUnit
    {
        private readonly int _clientProtocolVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolVersionUnit"/> class.
        /// </summary>
        /// <param name="messageConveyor">The message conveyor.</param>
        /// <param name="clientProtocolVersion">The client protocol version.</param>
        public ProtocolVersionUnit([NotNull] MessageConveyor messageConveyor, int clientProtocolVersion)
            : base(messageConveyor)
        {
            _clientProtocolVersion = clientProtocolVersion;
            Assert.ArgumentNotNull(messageConveyor, "messageConveyor");
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

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        public override void HandleMessage(Message message)
        {
            Assert.ArgumentNotNull(message, "message");
            var protocolVersionMessage = message as ProtocolVersionMessage;
            if (protocolVersionMessage == null || protocolVersionMessage.Version == _clientProtocolVersion)
            {
                return;
            }

            message.Handled = true;
            PushMessageToConveyor(new ErrorMessage(Resources.ProtocolVersionMismatch));
            PushCommandToConveyor(new DisconnectCommand());
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandRepeaterUnit.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the CommandRepeaterUnit type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ConveyorUnits
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using Common.Commands;
    using Common.Conveyor;
    using Common.ConveyorUnits;
    using Common.Messages;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Messages;

    /// <summary>
    /// A <see cref="ConveyorUnit"/> implementation that "repeats" text commands to output window.
    /// </summary>
    public class CommandRepeaterUnit : ConveyorUnit
    {
        private bool _displayInput = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandRepeaterUnit"/> class.
        /// </summary>
        /// <param name="messageConveyor">The message conveyor.</param>
        public CommandRepeaterUnit([NotNull] MessageConveyor messageConveyor)
            : base(messageConveyor)
        {
            Assert.ArgumentNotNull(messageConveyor, "messageConveyor");
        }

        /// <summary>
        /// Gets a set of message types that this unit can handle.
        /// </summary>
        public override IEnumerable<int> HandledMessageTypes
        {
            get
            {
                return new[] { BuiltInMessageTypes.EchoModeMessage };
            }
        }

        /// <summary>
        /// Gets a set of command types that this unit can handle.
        /// </summary>
        public override IEnumerable<int> HandledCommandTypes
        {
            get
            {
                return new[] { BuiltInCommandTypes.TextCommand };
            }
        }

        /// <summary>
        /// Handles the command.
        /// </summary>
        /// <param name="command">The command to handle.</param>
        public override void HandleCommand(Command command)
        {
            Assert.ArgumentNotNull(command, "command");

            var textCommand = command as TextCommand;
            if (textCommand == null)
            {
                return;
            }

            if (_displayInput)
            {
                PushMessageToConveyor(new CommandRepeatMessage(textCommand.CommandText));
            }
            else
            {
                PushMessageToConveyor(new CommandRepeatMessage(Regex.Replace(textCommand.CommandText, ".", "*")));
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        public override void HandleMessage(Message message)
        {
            Assert.ArgumentNotNull(message, "message");

            var echoMessage = message as ChangeEchoModeMessage;
            if (echoMessage != null)
            {
                _displayInput = echoMessage.DisplayTypedCharacters;
            }
        }
    }
}

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
    using Common.Commands;
    using Common.Conveyor;
    using Common.ConveyorUnits;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Messages;

    /// <summary>
    /// A <see cref="ConveyorUnit"/> implementation that "repeats" text commands to output window.
    /// </summary>
    public class CommandRepeaterUnit : ConveyorUnit
    {
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

            PushMessageToConveyor(new CommandRepeatMessage(textCommand.CommandText));
        }
    }
}

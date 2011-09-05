// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandSeparatorUnit.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the CommandSeparatorUnit type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ConveyorUnits
{
    using System.Collections.Generic;
    using System.Linq;

    using Common.Commands;
    using Common.Conveyor;
    using Common.ConveyorUnits;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// A <see cref="ConveyorUnit"/> implementation that splits commands separated by ';'.
    /// </summary>
    public class CommandSeparatorUnit : ConveyorUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandSeparatorUnit"/> class.
        /// </summary>
        /// <param name="messageConveyor">The message conveyor.</param>
        public CommandSeparatorUnit([NotNull] MessageConveyor messageConveyor)
            : base(messageConveyor)
        {
            Assert.ArgumentNotNull(messageConveyor, "messageConveyor");
        }

        #region Overrides of ConveyorUnit

        /// <summary>
        /// Gets a set of message types that this unit can handle.
        /// </summary>
        public override IEnumerable<int> HandledMessageTypes
        {
            get
            {
                return Enumerable.Empty<int>();
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

            if (!textCommand.CommandText.Contains(";"))
            {
                return;
            }

            foreach (var subCommand in textCommand.CommandText.Split(';'))
            {
                PushCommandToConveyor(new TextCommand(subCommand));
            }

            textCommand.Handled = true;
        }

        #endregion
    }
}

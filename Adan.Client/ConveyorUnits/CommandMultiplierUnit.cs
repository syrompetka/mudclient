// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandMultiplierUnit.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the CommandMultiplierUnit type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ConveyorUnits
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Common.Commands;
    using Common.Conveyor;
    using Common.ConveyorUnits;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// A <see cref="ConveyorUnit"/> implementation that "multiplies" commands string with "#number"
    /// </summary>
    public class CommandMultiplierUnit : ConveyorUnit
    {
        private readonly Regex _regex = new Regex(@"^\#([0-9]+)(.*)");

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandMultiplierUnit"/> class.
        /// </summary>
        /// <param name="messageConveyor">The message conveyor.</param>
        public CommandMultiplierUnit([NotNull] MessageConveyor messageConveyor)
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

            foreach (Match match in _regex.Matches(textCommand.CommandText.Trim()))
            {
                command.Handled = true;
                int count;
                if (int.TryParse(match.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out count))
                {
                    for (int i = 0; i < count; i++)
                    {
                        PushCommandToConveyor(new TextCommand(match.Groups[2].Value));
                    }
                }
            }
        }
    }
}

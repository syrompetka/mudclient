using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adan.Client.Common.Commands;
using Adan.Client.Common.Conveyor;
using Adan.Client.Common.ConveyorUnits;
using Adan.Client.Common.Model;
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;

namespace Adan.Client.ConveyorUnits
{
    /// <summary>
    /// A <see cref="CapForLineCommandUnit"/> implementation that will stop command line.
    /// </summary>
    public class CapForLineCommandUnit : ConveyorUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CapForLineCommandUnit"/> class.
        /// </summary>
        /// <param name="messageConveyor">The message conveyor.</param>
        public CapForLineCommandUnit([NotNull] MessageConveyor messageConveyor)
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

            if (textCommand.CommandText.StartsWith(RootModel.CommandChar.ToString()))
                command.Handled = true;
        }
    }
}
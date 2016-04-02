using System;
using System.Collections.Generic;
using System.Linq;
using Adan.Client.Commands;
using Adan.Client.Common.Commands;
using Adan.Client.Common.Conveyor;
using Adan.Client.Common.ConveyorUnits;
using Adan.Client.Messages;

namespace Adan.Client.ConveyorUnits
{
    /// <summary>
    /// A <see cref="ConveyorUnit"/> implementation to toggle full screen mode of the main window.
    /// </summary>
    public sealed class ToggleFullScreenModeUnit : ConveyorUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleFullScreenModeUnit"/> class.
        /// </summary>
        public ToggleFullScreenModeUnit(MessageConveyor conveyor) : base(conveyor)
        {
        }

        /// <summary>
        /// Gets a set of message types that this unit can handle.
        /// </summary>
        public override IEnumerable<int> HandledMessageTypes
        {
            get { return Enumerable.Empty<int>(); }
        }

        /// <summary>
        /// Gets a set of command types that this unit can handle.
        /// </summary>
        public override IEnumerable<int> HandledCommandTypes
        {
            get { return new[] { BuiltInCommandTypes.ToggleFullScreenMode, BuiltInCommandTypes.TextCommand }; }
        }

        public override void HandleCommand(Command command, bool isImport = false)
        {
            var toggleCommand = command as ToggleFullScreenModeCommand;
            if (toggleCommand != null)
            {
                PushMessageToConveyor(new ToggleFullScreenModeMessage());
                command.Handled = true;
                return;
            }

            var textCommand = command as TextCommand;
            if (textCommand != null)
            {
                if (string.Equals(textCommand.CommandText.Trim(), "#togglefullscreen", StringComparison.OrdinalIgnoreCase))
                {
                    PushMessageToConveyor(new ToggleFullScreenModeMessage());
                    command.Handled = true;
                }
            }

        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HotkeyUnit.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the HotkeyUnit type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ConveyorUnits
{
    using System.Linq;

    using Commands;

    using Common.Commands;
    using Common.Conveyor;
    using Common.ConveyorUnits;
    using Common.Messages;
    using Common.Model;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Model;
    using Model.Actions;

    /// <summary>
    /// A <see cref="ConveyorUnit"/> that handles hotkeys.
    /// </summary>
    public class HotkeyUnit : ConveyorUnit
    {
        private readonly RootModel _rootModel;
        private readonly ActionExecutionContext _context = new ActionExecutionContext { CurrentMessage = new OutputToMainWindowMessage(string.Empty) };

        /// <summary>
        /// Initializes a new instance of the <see cref="HotkeyUnit"/> class.
        /// </summary>
        /// <param name="messageConveyor">The message conveyor.</param>
        /// <param name="rootModel">The root model.</param>
        public HotkeyUnit([NotNull] MessageConveyor messageConveyor, [NotNull]RootModel rootModel)
            : base(messageConveyor)
        {
            Assert.ArgumentNotNull(messageConveyor, "messageConveyor");
            Assert.ArgumentNotNull(rootModel, "rootModel");
            _rootModel = rootModel;
        }

        #region Overrides of ConveyorUnit

        /// <summary>
        /// Handles the command.
        /// </summary>
        /// <param name="command">The command to handle.</param>
        public override void HandleCommand(Command command)
        {
            Assert.ArgumentNotNull(command, "command");
            var hotKeyCommand = command as HotkeyCommand;
            if (hotKeyCommand == null)
            {
                return;
            }

            hotKeyCommand.Handled = true;
            hotKeyCommand.HotkeyProcessed = false;
            foreach (var group in _rootModel.Groups.Where(g => g.IsEnabled))
            {
                foreach (var hotkey in group.Hotkeys)
                {
                    if (hotkey.Key != hotKeyCommand.Key || hotkey.ModifierKeys != hotKeyCommand.ModifierKeys)
                    {
                        continue;
                    }

                    foreach (var action in hotkey.Actions)
                    {
                        action.Execute(_rootModel, _context);
                    }

                    hotKeyCommand.HotkeyProcessed = true;
                    return;
                }
            }
        }

        #endregion
    }
}

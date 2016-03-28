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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common.Commands;
    using Common.Conveyor;
    using Common.ConveyorUnits;
    using Common.Model;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// A <see cref="ConveyorUnit"/> that handles hotkeys.
    /// </summary>
    public class HotkeyUnit : ConveyorUnit
    {
        //private readonly RootModel _rootModel;
        //private readonly ActionExecutionContext _context = new ActionExecutionContext { CurrentMessage = new OutputToMainWindowMessage(string.Empty) };
        private readonly ActionExecutionContext _context = ActionExecutionContext.Empty;

        public HotkeyUnit(MessageConveyor conveyor) : base(conveyor)
        {
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
                return new[] { BuiltInCommandTypes.HotkeyCommand };
            }
        }
        
        public override void HandleCommand(Command command, bool isImport = false)
        {
            Assert.ArgumentNotNull(command, "command");

            var hotKeyCommand = command as HotkeyCommand;

            if (hotKeyCommand == null)
            {
                return;
            }

            foreach (var group in Conveyor.RootModel.Groups.Where(g => g.IsEnabled))
            {
                foreach (var hotkey in group.Hotkeys)
                {
                    if (hotkey.Key != hotKeyCommand.Key || hotkey.ModifierKeys != hotKeyCommand.ModifierKeys)
                    {
                        continue;
                    }

                    foreach (var action in hotkey.Actions)
                    {
                        action.Execute(Conveyor.RootModel, _context);
                    }

                    hotKeyCommand.Handled = true;
                    return;
                }
            }
        }

        #endregion
    }
}

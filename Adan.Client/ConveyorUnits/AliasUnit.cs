// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AliasUnit.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the AliasUnit type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ConveyorUnits
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Common.Commands;
    using Common.Conveyor;
    using Common.ConveyorUnits;
    using Common.Messages;
    using Common.Model;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// A <see cref="ConveyorUnit"/> that processes aliases.
    /// </summary>
    public class AliasUnit : ConveyorUnit
    {
        private readonly RootModel _rootModel;
        private readonly ActionExecutionContext _context = new ActionExecutionContext { CurrentMessage = new OutputToMainWindowMessage(string.Empty) };

        // For optimization.
        private readonly char[] _paramsSeparatorArray = new[] { ' ' };

        /// <summary>
        /// Initializes a new instance of the <see cref="AliasUnit"/> class.
        /// </summary>
        /// <param name="messageConveyor">The message conveyor.</param>
        /// <param name="rootModel">The root model.</param>
        public AliasUnit([NotNull] MessageConveyor messageConveyor, [NotNull]RootModel rootModel)
            : base(messageConveyor)
        {
            Assert.ArgumentNotNull(messageConveyor, "messageConveyor");
            Assert.ArgumentNotNull(rootModel, "rootModel");
            _rootModel = rootModel;
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

            var commandText = Regex.Replace(textCommand.CommandText.Trim(), @"\s+", " ");
            foreach (var group in _rootModel.Groups.Where(g => g.IsEnabled))
            {
                foreach (var alias in group.Aliases)
                {
                    if (!commandText.StartsWith(alias.Command + " ", StringComparison.CurrentCulture) && !commandText.Equals(alias.Command, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var parts = commandText.Split(_paramsSeparatorArray, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2)
                    {
                        _context.Parameters[0] = string.Join(" ", parts.Skip(1));
                    }
                    else
                    {
                        _context.Parameters[0] = string.Empty;
                    }

                    for (int i = 1; i < 10; i++)
                    {
                        if (i < parts.Length)
                        {
                            _context.Parameters[i] = parts[i];
                        }
                        else
                        {
                            _context.Parameters[i] = string.Empty;
                        }
                    }

                    foreach (var action in alias.Actions)
                    {
                        action.Execute(_rootModel, _context);
                    }

                    textCommand.Handled = true;
                    return;
                }
            }
        }

        #endregion
    }
}

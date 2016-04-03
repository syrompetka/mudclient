namespace Adan.Client.ConveyorUnits
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Common.Conveyor;
    using Common.Commands;
    using Common.ConveyorUnits;
    using Common.Model;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// A <see cref="ConveyorUnit"/> that processes aliases.
    /// </summary>
    public class AliasUnit : ConveyorUnit
    {
        private readonly ActionExecutionContext _context = ActionExecutionContext.Empty;

        private readonly Regex _whiteSpaceRegex = new Regex(@" {2,}", RegexOptions.Compiled);
        // For optimization.
        private readonly char[] _paramsSeparatorArray = new[] { ' ' };

        /// <summary>
        /// Initializes a new instance of the <see cref="AliasUnit"/> class.
        /// </summary>
        public AliasUnit(MessageConveyor conveyor)
            : base(conveyor)
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
                return new[] { BuiltInCommandTypes.TextCommand };
            }
        }

        public override void HandleCommand(Command command, bool isImport = false)
        {
            Assert.ArgumentNotNull(command, "command");

            var textCommand = command as TextCommand;
            if (textCommand == null)
            {
                return;
            }

            var commandText = _whiteSpaceRegex.Replace(textCommand.CommandText.Trim(), " ");
            foreach (var group in Conveyor.RootModel.Groups.Where(g => g.IsEnabled))
            {
                foreach (var alias in group.Aliases)
                {
                    if (!commandText.StartsWith(alias.Command + " ", StringComparison.OrdinalIgnoreCase) && !commandText.Equals(alias.Command, StringComparison.OrdinalIgnoreCase))
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
                    foreach (var actionBase in alias.Actions)
                    {
                        actionBase.Execute(Conveyor.RootModel, _context);
                    }

                    Conveyor.RootModel.PushCommandToConveyor(FlushOutputQueueCommand.Instance);
                    textCommand.Handled = true;
                    return;
                }
            }
        }

        #endregion
    }
}

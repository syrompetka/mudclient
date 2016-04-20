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
    using Model.Actions;
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

                    int ind = commandText.IndexOf(' ');
                    if (ind != -1)
                        _context.Parameters[0] = commandText.Substring(ind + 1);
                    else
                        _context.Parameters[0] = String.Empty;

                    var parts = _context.Parameters[0].Split(_paramsSeparatorArray, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 1; i < 10; ++i)
                    {
                        if (i - 1 < parts.Length)
                            _context.Parameters[i] = parts[i - 1];
                        else
                            _context.Parameters[i] = string.Empty;
                    }

                    //If we have only 1 parameter then %1 = %0 like in jmc
                    var allCommandText = String.Join(";", alias.Actions.OfType<SendTextAction>().Select(a => a.CommandText).ToArray());
                    if (allCommandText.Contains("%1") && !allCommandText.Contains("%0") && !allCommandText.Contains("%2"))
                    {
                        _context.Parameters[1] = _context.Parameters[0];
                    }
                    
                    var aliasContainsParams = false;
                    int lastSendTextAction = -1;
                    for (var i = 0; i < alias.Actions.Count; ++i)
                    {
                        var act = alias.Actions[i] as SendTextAction;
                        if (act != null)
                        {
                            lastSendTextAction = i;
                            if (act.CommandText.Contains("%0") || act.CommandText.Contains("%1"))
                                aliasContainsParams = true;
                        }
                    }

                    for (var i = 0; i < alias.Actions.Count; i++)
                    {
                        if (i == lastSendTextAction && !aliasContainsParams)
                        {
                            var sendTextAction = alias.Actions[i] as SendTextAction;
                            if (sendTextAction == null)
                            {
                                continue;
                            }

                            if (sendTextAction.Parameters.Any())
                            {
                                sendTextAction.Execute(Conveyor.RootModel, _context);
                            }
                            else
                            {
                                new SendTextAction
                                {
                                    CommandText = sendTextAction.CommandText + " " + _context.Parameters[0]
                                }.Execute(Conveyor.RootModel, _context);
                            }
                        }
                        else
                        {
                            alias.Actions[i].Execute(Conveyor.RootModel, _context);
                        }
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

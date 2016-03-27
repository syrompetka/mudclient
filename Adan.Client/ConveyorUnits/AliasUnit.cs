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
    using Model.Actions;
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


        /// <summary>
        /// Initializes a new instance of the <see cref="AliasUnit"/> class.
        /// </summary>
        public AliasUnit()
            : base()
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="rootModel"></param>
        /// <param name="isImport"></param>
        public override void HandleCommand(Command command, RootModel rootModel, bool isImport = false)
        {
            Assert.ArgumentNotNull(command, "command");

            var textCommand = command as TextCommand;
            if (textCommand == null)
            {
                return;
            }

            var commandText = Regex.Replace(textCommand.CommandText.Trim(), @"\s+", " ");
            foreach (var group in rootModel.Groups.Where(g => g.IsEnabled))
            {
                foreach (var alias in group.Aliases)
                {
                    if (!commandText.StartsWith(alias.Command + " ", StringComparison.CurrentCulture) && !commandText.Equals(alias.Command, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    int ind = commandText.IndexOf(' ');

                    if (ind != -1)
                        _context.Parameters[0] = commandText.Substring(ind + 1);
                    else
                        _context.Parameters[0] = String.Empty;

                    _context.Parameters[1] = _context.Parameters[0];

                    //var aliasContainsParams = alias.Actions.OfType<SendTextAction>().Any(a => a.CommandText.Contains("%0") || a.CommandText.Contains("%1"));
                    var aliasContainsParams = false;
                    int lastSendTextAction = -1;
                    for(var i = 0; i < alias.Actions.Count; ++i)
                    {
                        var act = alias.Actions[i] as SendTextAction;
                        if(act != null)
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
                            (new SendTextAction() { CommandText = sendTextAction.CommandText + " " + _context.Parameters[0] }).Execute(rootModel, _context);
                        }
                        else
                            alias.Actions[i].Execute(rootModel, _context);
                    }

                    textCommand.Handled = true;
                    return;
                }
            }
        }

        #endregion
    }
}

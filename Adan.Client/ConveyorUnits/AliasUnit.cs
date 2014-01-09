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
    using Adan.Client.Model.Actions;
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

                    foreach (var action in alias.Actions)
                    {
                        //Проверка старых, неправильно работающих алиасов
                        var oldAction = action as SendTextAction;
                        if(oldAction != null)
                            (new SendTextOneParameterAction() { CommandText = oldAction.CommandText, Parameters = oldAction.Parameters }).Execute(rootModel, _context);
                        else
                            action.Execute(rootModel, _context);
                    }

                    textCommand.Handled = true;
                    return;
                }
            }
        }

        #endregion
    }
}

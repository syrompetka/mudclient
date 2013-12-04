﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandSeparatorUnit.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the CommandSeparatorUnit type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ConveyorUnits
{
    using System.Collections.Generic;
    using System.Linq;

    using Adan.Client.Common.Commands;
    using Adan.Client.Common.Conveyor;
    using Adan.Client.Common.ConveyorUnits;
    using Adan.Client.Common.Model;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// A <see cref="ConveyorUnit"/> implementation that splits commands separated by CharDelimiter.
    /// </summary>
    public class CommandSeparatorUnit : ConveyorUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandSeparatorUnit"/> class.
        /// </summary>
        public CommandSeparatorUnit()
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

            if (textCommand.IsSeparated)
                return;

            string commandText = textCommand.CommandText;

            int nest = 0;
            int i = 0;
            int startIndex = 0;
            while (i < commandText.Length)
            {
                while (i < commandText.Length && !(commandText[i] == RootModel.CommandDelimiter && nest == 0))
                {
                    if (commandText[i] == '{')
                    {
                        nest++;
                    }
                    else if (commandText[i] == '}')
                    {
                        nest--;
                    }

                    i++;
                }

                if (i < commandText.Length)
                {
                    PushCommandToConveyor(new TextCommand(commandText.Substring(startIndex, i - startIndex)) { IsSeparated = true }, rootModel);
                    i++;
                    startIndex = i;
                    textCommand.Handled = true;
                }
            }

            if(startIndex != 0)
                PushCommandToConveyor(new TextCommand(commandText.Substring(startIndex, i - startIndex)) { IsSeparated = true }, rootModel);
        }

        #endregion
    }
}

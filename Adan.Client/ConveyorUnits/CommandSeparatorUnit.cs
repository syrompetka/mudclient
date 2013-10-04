// --------------------------------------------------------------------------------------------------------------------
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
    /// A <see cref="ConveyorUnit"/> implementation that splits commands separated by ';'.
    /// </summary>
    public class CommandSeparatorUnit : ConveyorUnit
    {
        private RootModel _rootModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandSeparatorUnit"/> class.
        /// </summary>
        /// <param name="messageConveyor">The message conveyor.</param>
        /// <param name="rootModel">The Root Model.</param>
        public CommandSeparatorUnit([NotNull] MessageConveyor messageConveyor, [NotNull] RootModel rootModel)
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

            if (textCommand.IsSeparated)
                return;

            string commandText = textCommand.CommandText;

            int nest = 0;
            int i = 0;
            int startIndex = 0;
            while (i < commandText.Length)
            {
                while (i < commandText.Length && !(commandText[i] == RootModel.CharDelimiter && nest == 0))
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
                    base.PushCommandToConveyor(new TextCommand(commandText.Substring(startIndex, i - startIndex)) { IsSeparated = true });
                    i++;
                    startIndex = i;
                    textCommand.Handled = true;
                }
            }

            if(startIndex != 0)
                base.PushCommandToConveyor(new TextCommand(commandText.Substring(startIndex, i - startIndex)) { IsSeparated = true });
        }

        #endregion
    }
}

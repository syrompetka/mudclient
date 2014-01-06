// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandRepeaterUnit.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the CommandRepeaterUnit type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ConveyorUnits
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Adan.Client.Common.Model;
    using Common.Commands;
    using Common.Conveyor;
    using Common.ConveyorUnits;
    using Common.Messages;
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Messages;

    /// <summary>
    /// A <see cref="ConveyorUnit"/> implementation that "repeats" text commands to output window.
    /// </summary>
    public class CommandRepeaterUnit : ConveyorUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandRepeaterUnit"/> class.
        /// </summary>
        public CommandRepeaterUnit()
            : base()
        {
        }

        /// <summary>
        /// Gets a set of message types that this unit can handle.
        /// </summary>
        public override IEnumerable<int> HandledMessageTypes
        {
            get
            {
                return new[] { BuiltInMessageTypes.EchoModeMessage };
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

            PushMessageToConveyor(new CommandRepeatMessage(textCommand.CommandText), rootModel);

            //if (_displayInput)
            //{
            //    PushMessageToConveyor(new CommandRepeatMessage(textCommand.CommandText), rootModel);
            //}
            //else
            //{
            //    PushMessageToConveyor(new CommandRepeatMessage(Regex.Replace(textCommand.CommandText, ".", "*")), rootModel);
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="rootModel"></param>
        public override void HandleMessage(Message message, RootModel rootModel)
        {
            Assert.ArgumentNotNull(message, "message");

            var echoMessage = message as ChangeEchoModeMessage;
            if (echoMessage != null)
            {
                message.Handled = true;
                //_displayInput = echoMessage.DisplayTypedCharacters;
            }
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandRepeatMessage.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the CommandRepeatMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Messages
{
    using Common.Messages;
    using Common.Themes;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// A message that "displays" user entered text command.
    /// </summary>
    public class CommandRepeatMessage : OutputToMainWindowMessage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        public CommandRepeatMessage([NotNull] string commandText)
            : base(commandText, TextColor.RepeatCommandTextColor)
        {
            Assert.ArgumentNotNull(commandText, "commandText");

            SkipTriggers = true;
        }

        /// <summary>
        /// Gets the type of this message.
        /// </summary>
        /// <value>
        /// The type of this message.
        /// </value>
        public override int MessageType
        {
            get
            {
                return BuiltInMessageTypes.SystemMessage;
            }
        }
    }
}

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
        /// Initializes a new instance of the <see cref="CommandRepeatMessage"/> class.
        /// </summary>
        /// <param name="commandText">The command text to display.</param>
        public CommandRepeatMessage([NotNull] string commandText)
            : base(commandText, TextColor.RepeatCommandTextColor)
        {
            Assert.ArgumentNotNull(commandText, "commandText");
            SkipProcessing = true;
            SkipTriggers = true;
        }
    }
}

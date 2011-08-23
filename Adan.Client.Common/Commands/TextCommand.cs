// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextCommand.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the TextCommand type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Commands
{
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Text command to be sent to server.
    /// </summary>
    public class TextCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextCommand"/> class.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        public TextCommand([NotNull] string commandText)
        {
            Validate.ArgumentNotNull(commandText, "commandText");
            CommandText = commandText;
        }

        /// <summary>
        /// Gets the text of this command.
        /// </summary>
        [NotNull]
        public string CommandText
        {
            get;
            private set;
        }
    }
}

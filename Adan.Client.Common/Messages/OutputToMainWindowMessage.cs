// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputToMainWindowMessage.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the OutputToMainWindowMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Messages
{
    using System.Collections.Generic;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Themes;

    /// <summary>
    /// Message that is output to main window.
    /// </summary>
    public class OutputToMainWindowMessage : TextMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutputToMainWindowMessage"/> class.
        /// </summary>
        /// <param name="messageBlocks">The message blocks.</param>
        public OutputToMainWindowMessage([NotNull] IEnumerable<TextMessageBlock> messageBlocks)
            : base(messageBlocks)
        {
            Assert.ArgumentNotNull(messageBlocks, "messageBlocks");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputToMainWindowMessage"/> class.
        /// </summary>
        /// <param name="text">The text to display.</param>
        /// <param name="foregroundColor">Color of the foreground.</param>
        public OutputToMainWindowMessage([NotNull] string text, TextColor foregroundColor)
            : base(text, foregroundColor)
        {
            Assert.ArgumentNotNull(text, "text");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputToMainWindowMessage"/> class.
        /// </summary>
        /// <param name="text">The text to display.</param>
        public OutputToMainWindowMessage([NotNull] string text)
            : base(text)
        {
            Assert.ArgumentNotNull(text, "text");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputToMainWindowMessage"/> class.
        /// </summary>
        /// <param name="text">The text to display.</param>
        /// <param name="foregroundColor">Color of the foreground.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        public OutputToMainWindowMessage([NotNull] string text, TextColor foregroundColor, TextColor backgroundColor)
            : base(text, foregroundColor, backgroundColor)
        {
            Assert.ArgumentNotNull(text, "text");
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
                return BuiltInMessageTypes.TextMessage;
            }
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputToAdditionalWindowMessage.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the OutputToAdditionalWindowMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.OutputWindow
{
    using Common.Messages;
    using Common.Themes;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Message that is output to additional window.
    /// </summary>
    public class OutputToAdditionalWindowMessage : TextMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutputToAdditionalWindowMessage"/> class.
        /// </summary>
        /// <param name="originalMessage">The original message.</param>
        public OutputToAdditionalWindowMessage([NotNull] TextMessage originalMessage)
            : base(originalMessage)
        {
            Assert.ArgumentNotNull(originalMessage, "originalMessage");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputToAdditionalWindowMessage"/> class.
        /// </summary>
        /// <param name="text">The text to display.</param>
        /// <param name="foregroundColor">Color of the foreground.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        public OutputToAdditionalWindowMessage([NotNull] string text, TextColor foregroundColor, TextColor backgroundColor)
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

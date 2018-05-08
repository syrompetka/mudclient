// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputToAdditionalWindowMessage.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the OutputToAdditionalWindowMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.OutputWindow.Messages
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

            this.SkipSubstitution = true;
            this.SkipTriggers = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public OutputToAdditionalWindowMessage(string text)
            : base(text)
        {
            this.SkipSubstitution = true;
            this.SkipTriggers = true;
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

            this.SkipSubstitution = true;
            this.SkipTriggers = true;
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override TextMessage Clone()
        {
            return new OutputToAdditionalWindowMessage(this);
        }
    }
}

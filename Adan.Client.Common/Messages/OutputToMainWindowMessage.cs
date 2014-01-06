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
        /// 
        /// </summary>
        /// <param name="originalMessage"></param>
        public OutputToMainWindowMessage(OutputToMainWindowMessage originalMessage)
            : base(originalMessage)
        {
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
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="isColored"></param>
        public OutputToMainWindowMessage([NotNull] string text, bool isColored)
            : base(text, isColored)
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override TextMessage NewInstance()
        {
            return new OutputToMainWindowMessage(this);
        }
    }
}

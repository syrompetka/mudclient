// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InfoMessage.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Information message that should be displayed to end user.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Messages
{
    using System.Collections.Generic;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Themes;

    /// <summary>
    /// Information message that should be displayed to end user.
    /// </summary>
    public class InfoMessage : OutputToMainWindowMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InfoMessage"/> class.
        /// </summary>
        /// <param name="text">The text to display.</param>
        public InfoMessage([NotNull] string text)
            : base(text, TextColor.BrightWhite)
        {
            Assert.ArgumentNotNull(text, "text");

            base.AppendText("\x1B[0m");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoMessage"/> class.
        /// </summary>
        /// <param name="text">The text to display.</param>
        /// <param name="textColor">Color of the text.</param>
        public InfoMessage([NotNull] string text, TextColor textColor)
            : base(text, textColor)
        {
            Assert.ArgumentNotNull(text, "text");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalMessage"></param>
        public InfoMessage(InfoMessage originalMessage)
            : base(originalMessage)
        {

        }

        ///// <summary>
        ///// Initializes a new instance of the <see cref="InfoMessage"/> class.
        ///// </summary>
        ///// <param name="messageBlocks">The message blocks.</param>
        //public InfoMessage([NotNull]IEnumerable<TextMessageBlock> messageBlocks)
        //    : base(messageBlocks)
        //{
        //    Assert.ArgumentNotNull(messageBlocks, "messageBlocks");
        //}

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

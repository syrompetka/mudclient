// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextMessage.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the TextMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Messages
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Themes;

    /// <summary>
    /// Plain text message.
    /// </summary>
    public abstract class TextMessage : Message
    {
        private bool _isInnerTextComputed;
        private string _innerText;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextMessage"/> class.
        /// </summary>
        /// <param name="originalMessage">The original message.</param>
        protected TextMessage([NotNull] TextMessage originalMessage)
        {
            Assert.ArgumentNotNull(originalMessage, "originalMessage");

            // need to deep copy original message to prevent double substitution for example.
            var blocks = originalMessage.MessageBlocks.Select(textMessageBlock => new TextMessageBlock(textMessageBlock.Text, textMessageBlock.Foreground, textMessageBlock.Background)).ToList();

            MessageBlocks = blocks;
            _isInnerTextComputed = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextMessage"/> class.
        /// </summary>
        /// <param name="messageBlocks">The message blocks.</param>
        protected TextMessage([NotNull] IEnumerable<TextMessageBlock> messageBlocks)
        {
            Assert.ArgumentNotNull(messageBlocks, "messageBlocks");

            MessageBlocks = messageBlocks;
            _isInnerTextComputed = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextMessage"/> class.
        /// </summary>
        /// <param name="text">The text to display.</param>
        /// <param name="foregroundColor">Color of the foreground.</param>
        protected TextMessage([NotNull] string text, TextColor foregroundColor)
        {
            Assert.ArgumentNotNull(text, "text");
            MessageBlocks = new List<TextMessageBlock> { new TextMessageBlock(text, foregroundColor) };
            _isInnerTextComputed = true;
            _innerText = text;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextMessage"/> class.
        /// </summary>
        /// <param name="text">The text to display.</param>
        protected TextMessage([NotNull] string text)
        {
            Assert.ArgumentNotNull(text, "text");
            MessageBlocks = new List<TextMessageBlock> { new TextMessageBlock(text) };
            _isInnerTextComputed = true;
            _innerText = text;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextMessage"/> class.
        /// </summary>
        /// <param name="text">The text to display.</param>
        /// <param name="foregroundColor">Color of the foreground.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        protected TextMessage([NotNull] string text, TextColor foregroundColor, TextColor backgroundColor)
        {
            Assert.ArgumentNotNull(text, "text");
            MessageBlocks = new List<TextMessageBlock> { new TextMessageBlock(text, foregroundColor, backgroundColor) };
            _isInnerTextComputed = true;
            _innerText = text;
        }

        /// <summary>
        /// Gets the message blocks.
        /// </summary>
        [NotNull]
        public IEnumerable<TextMessageBlock> MessageBlocks
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the inner text of this message.
        /// </summary>
        [NotNull]
        public string InnerText
        {
            get
            {
                if (!_isInnerTextComputed)
                {
                    var builder = new StringBuilder();
                    foreach (var messageBlock in MessageBlocks)
                    {
                        builder.Append(messageBlock.Text);
                    }

                    _innerText = builder.ToString();
                    _isInnerTextComputed = true;
                }

                return _innerText;
            }
        }

        /// <summary>
        /// Updates the inner text.
        /// </summary>
        public void UpdateInnerText()
        {
            _isInnerTextComputed = false;
            _innerText = string.Empty;
        }

        /// <summary>
        /// Updates the message blocks.
        /// </summary>
        /// <param name="messageBlocks">The message blocks.</param>
        public void UpdateMessageBlocks([NotNull] IEnumerable<TextMessageBlock> messageBlocks)
        {
            Assert.ArgumentNotNull(messageBlocks, "messageBlocks");

            MessageBlocks = messageBlocks;
        }
    }
}

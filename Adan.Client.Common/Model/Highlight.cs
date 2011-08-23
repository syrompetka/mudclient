// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Highlight.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the Highlight type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Model
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Text.RegularExpressions;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Messages;
    using Themes;
    using Utils;

    /// <summary>
    /// A highlight of all incoming strings that match certain pattern with a specific color and background.
    /// </summary>
    [DataContract]
    public class Highlight
    {
        private string _textToHighlight;
        private Regex _textToHighlightRegex;

        /// <summary>
        /// Initializes a new instance of the <see cref="Highlight"/> class.
        /// </summary>
        public Highlight()
        {
            _textToHighlight = string.Empty;
            TextColor = TextColor.None;
            BackgroundColor = TextColor.None;
        }

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>
        /// The color of the background.
        /// </value>
        [DataMember]
        public TextColor BackgroundColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        /// <value>
        /// The color of the text.
        /// </value>
        [DataMember]
        public TextColor TextColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the text to highlight.
        /// </summary>
        /// <value>
        /// The text to highlight.
        /// </value>
        [NotNull]
        [DataMember]
        public string TextToHighlight
        {
            get
            {
                return _textToHighlight;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                _textToHighlight = value;
                _textToHighlightRegex = null;
            }
        }

        [NotNull]
        private Regex TextToHighlightRegex
        {
            get
            {
                if (_textToHighlightRegex == null)
                {
                    _textToHighlightRegex = new Regex(WildcardStringHelper.ConvertToValidRegex(TextToHighlight));
                }

                return _textToHighlightRegex;
            }
        }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="textMessage">The text message.</param>
        public void ProcessMessage([NotNull] TextMessage textMessage)
        {
            Assert.ArgumentNotNull(textMessage, "textMessage");

            var messageBlocks = textMessage.MessageBlocks;
            bool matchSuccess = false;
            foreach (Match match in TextToHighlightRegex.Matches(textMessage.InnerText))
            {
                matchSuccess = true;
                var newBlocks = new List<TextMessageBlock>();
                var matchIndex = match.Index;
                var matchLength = match.Length;
                bool matchTextAdded = false;
                foreach (var block in messageBlocks)
                {
                    if (matchLength <= 0)
                    {
                        newBlocks.Add(block);
                    }
                    else if (matchIndex < block.Text.Length)
                    {
                        var charsToRemove = Math.Min(block.Text.Length - matchIndex, matchLength);
                        newBlocks.Add(new TextMessageBlock(block.Text.Remove(matchIndex), block.Foreground, block.Background));
                        if (!matchTextAdded)
                        {
                            newBlocks.Add(new TextMessageBlock(match.Value, TextColor, BackgroundColor));
                            matchTextAdded = true;
                        }

                        if (matchLength + matchIndex < block.Text.Length)
                        {
                            newBlocks.Add(new TextMessageBlock(block.Text.Substring(matchLength + matchIndex, block.Text.Length - matchLength - matchIndex), block.Foreground, block.Background));
                        }

                        matchLength -= charsToRemove;
                        matchIndex = 0;
                    }
                    else
                    {
                        newBlocks.Add(block);
                        matchIndex -= block.Text.Length;
                    }

                    if (matchIndex < 0)
                    {
                        matchIndex = 0;
                    }
                }

                messageBlocks = newBlocks;
            }

            if (matchSuccess)
            {
                textMessage.UpdateMessageBlocks(messageBlocks);
            }
        }
    }
}

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
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Messages;
    using Themes;
    using Utils.PatternMatching;

    /// <summary>
    /// A highlight of all incoming strings that match certain pattern with a specific color and background.
    /// </summary>
    [Serializable]
    public class Highlight
    {
        [NonSerialized]
        private readonly IList<string> _matchingResults = new List<string>(Enumerable.Repeat<string>(null, 11));
        private string _textToHighlight;
        [NonSerialized]
        private PatternToken _rootPatternToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="Highlight"/> class.
        /// </summary>
        public Highlight()
        {
            _textToHighlight = string.Empty;
            ForegroundColor = TextColor.None;
            BackgroundColor = TextColor.None;
        }

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>
        /// The color of the background.
        /// </value>
        [XmlAttribute]
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
        [XmlAttribute]
        public TextColor ForegroundColor
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
        [XmlAttribute]
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
            }
        }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="textMessage">The text message.</param>
        /// <param name="rootModel">The root model.</param>
        public void ProcessMessage([NotNull] TextMessage textMessage, [NotNull] RootModel rootModel)
        {
            Assert.ArgumentNotNull(textMessage, "textMessage");
            Assert.ArgumentNotNull(rootModel, "rootModel");

            //var messageBlocks = textMessage.MessageBlocks;
            //bool matchSuccess = false;
            int position = 0;
            ClearMatchingResults();
            string text = textMessage.InnerText;
            var res = GetRootPatternToken(rootModel).Match(text, position, _matchingResults);

            while (res.IsSuccess)
            {
                textMessage.HighlightInnerText(ForegroundColor, BackgroundColor, res.StartPosition, res.EndPosition - res.StartPosition);
                position = res.EndPosition;
                ClearMatchingResults();
                res = GetRootPatternToken(rootModel).Match(text, position, _matchingResults);

                //matchSuccess = true;
                //var newBlocks = new List<TextMessageBlock>();
                //var matchIndex = res.StartPosition;
                //var matchLength = res.EndPosition - res.StartPosition;
                //position = res.EndPosition;
                //bool matchTextAdded = false;
                //foreach (var block in messageBlocks)
                //{
                //    if (matchLength <= 0)
                //    {
                //        newBlocks.Add(block);
                //    }
                //    else if (matchIndex < block.Text.Length)
                //    {
                //        var charsToRemove = Math.Min(block.Text.Length - matchIndex, matchLength);
                //        newBlocks.Add(
                //            new TextMessageBlock(block.Text.Remove(matchIndex), block.Foreground, block.Background));
                //        if (!matchTextAdded)
                //        {
                //            newBlocks.Add(new TextMessageBlock(textMessage.InnerText.Substring(res.StartPosition, res.EndPosition - res.StartPosition), TextColor, BackgroundColor));
                //            matchTextAdded = true;
                //        }

                //        if (matchLength + matchIndex < block.Text.Length)
                //        {
                //            newBlocks.Add(
                //                new TextMessageBlock(
                //                    block.Text.Substring(
                //                        matchLength + matchIndex, block.Text.Length - matchLength - matchIndex),
                //                    block.Foreground,
                //                    block.Background));
                //        }

                //        matchLength -= charsToRemove;
                //        matchIndex = 0;
                //    }
                //    else
                //    {
                //        newBlocks.Add(block);
                //        matchIndex -= block.Text.Length;
                //    }

                //    if (matchIndex < 0)
                //    {
                //        matchIndex = 0;
                //    }
                //}

                //messageBlocks = newBlocks;
                //ClearMatchingResults();
                //res = GetRootPatternToken(rootModel).Match(textMessage.InnerText, position, _matchingResults);
            }

            //if (matchSuccess)
            //{
            //    textMessage.UpdateMessageBlocks(messageBlocks);
            //}
        }

        private void ClearMatchingResults()
        {
            for (int i = 0; i < _matchingResults.Count; i++)
            {
                _matchingResults[i] = null;
            }
        }

        [NotNull]
        private PatternToken GetRootPatternToken([NotNull] RootModel rootModel)
        {
            Assert.ArgumentNotNull(rootModel, "rootModel");

            if (_rootPatternToken == null)
            {
                _rootPatternToken = WildcardParser.ParseWildcardString(_textToHighlight, rootModel);
            }

            return _rootPatternToken;
        }
    }
}

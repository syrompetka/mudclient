// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Highlight.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the Highlight type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using Adan.Client.Common.Messages;
using Adan.Client.Common.Themes;
using Adan.Client.Common.Utils.PatternMatching;
using CSLib.Net.Annotations;
using CSLib.Net.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Adan.Client.Common.Model
{

    /// <summary>
    /// A highlight of all incoming strings that match certain pattern with a specific color and background.
    /// </summary>
    [Serializable]
    public class Highlight : IUndo
    {
        [NonSerialized]
        private readonly IList<string> _matchingResults = new List<string>(Enumerable.Repeat<string>(null, 11));
        private string _textToHighlight;

        [NonSerialized]
        private PatternToken _rootPatternToken;

        private Regex _wildRegex = new Regex(@"%[0-9]", RegexOptions.Compiled);

        /// <summary>
        /// Initializes a new instance of the <see cref="Highlight"/> class.
        /// </summary>
        public Highlight()
        {
            _textToHighlight = string.Empty;
            ForegroundColor = TextColor.None;
            BackgroundColor = TextColor.None;

            Group = null;
            Operation = UndoOperation.None;
        }

        /// <summary>
        /// Is Regular Expression
        /// </summary>
        [NotNull]
        [XmlAttribute]
        public bool IsRegExp
        {
            get;
            set;
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

                if (value.Length > 2 && value[0] == '/' && value[value.Length - 1] == '/')
                {
                    _textToHighlight = value.Substring(1, value.Length - 2);
                    IsRegExp = true;
                }
                else
                {
                    _textToHighlight = value;
                    IsRegExp = false;
                }

                _rootPatternToken = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public UndoOperation Operation
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public Group Group
        {
            get;
            set;
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

            var messageBlocks = textMessage.MessageBlocks;
            //int position = 0;
            //ClearMatchingResults();
            string text = textMessage.InnerText;

            if (string.IsNullOrEmpty(textMessage.InnerText))
                return;

            if (IsRegExp)
            {
                var varReplace = rootModel.ReplaceVariables(TextToHighlight);
                if (!varReplace.IsAllVariables)
                    return;

                Regex rExp = new Regex(varReplace.Value);

                Match match = rExp.Match(textMessage.InnerText);

                while (match.Success)
                {
                    textMessage.HighlightText(match.Index, match.Length, ForegroundColor, BackgroundColor);
                    match = rExp.Match(textMessage.InnerText, match.Index + match.Length);
                }
            }
            else
            {
                var varReplace = rootModel.ReplaceVariables(TextToHighlight);
                if (!varReplace.IsAllVariables)
                    return;

                Regex rExp = new Regex(_wildRegex.Replace(Regex.Escape(varReplace.Value), ".*"));

                var match = rExp.Match(textMessage.InnerText);
                while (match.Success)
                {
                    textMessage.HighlightText(match.Index, match.Length, ForegroundColor, BackgroundColor);
                    match = rExp.Match(textMessage.InnerText, match.Index + match.Length);
                }
            }

            //var res = GetRootPatternToken(rootModel).Match(text, position, _matchingResults);

            //while (res.IsSuccess)
            //{
            //    textMessage.HighlightText(res.StartPosition, res.EndPosition - res.StartPosition, ForegroundColor, BackgroundColor);
            //    position = res.EndPosition;
            //    ClearMatchingResults();
            //    res = GetRootPatternToken(rootModel).Match(text, position, _matchingResults);
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string UndoInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("#Хайлайт {").Append(ForegroundColor);
            sb.Append(",").Append(BackgroundColor).Append("} {").Append(TextToHighlight).Append("} ");
            switch (Operation)
            {
                case UndoOperation.Add:
                    sb.Append("восстановлен");
                    break;
                case UndoOperation.Remove:
                    sb.Append("удален");
                    break;
            }

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Undo(RootModel rootModel)
        {
            if (Group != null && Operation != UndoOperation.None)
            {
                switch (Operation)
                {
                    case UndoOperation.Add:
                        Group.Highlights.Add(this);
                        break;
                    case UndoOperation.Remove:
                        Group.Highlights.Remove(this);
                        break;
                }
            }
        }

        private void ClearMatchingResults()
        {
            for (int i = 0; i < _matchingResults.Count; i++)
            {
                _matchingResults[i] = string.Empty;
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

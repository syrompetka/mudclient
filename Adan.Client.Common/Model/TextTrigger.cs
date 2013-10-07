// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextTrigger.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the TextTrigger type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;
    using System.Text;
    using System.Text.RegularExpressions;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Messages;

    using Utils.PatternMatching;

    /// <summary>
    /// Trigger that handles text messages from server.
    /// </summary>
    [Serializable]
    public class TextTrigger : TriggerBase
    {
        [NonSerialized]
        private readonly IList<string> _matchingResults = new List<string>(Enumerable.Repeat<string>(null, 11));

        [NonSerialized]
        private PatternToken _rootPatternToken;

        [NonSerialized]
        private ActionExecutionContext _context;
        private string _matchingPattern;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextTrigger"/> class.
        /// </summary>
        public TextTrigger()
        {
            MatchingPattern = string.Empty;
            _context = new ActionExecutionContext();
        }

        /// <summary>
        /// Get Is Regular Expression
        /// </summary>
        [NotNull]
        [XmlIgnore]
        public bool IsRegExp
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the matching pattern.
        /// </summary>
        /// <value>
        /// The matching pattern.
        /// </value>
        [NotNull]
        [XmlAttribute]
        public string MatchingPattern
        {
            get
            {
                return _matchingPattern;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                _matchingPattern = value;
                _rootPatternToken = null;

                if (_matchingPattern.StartsWith("/") && _matchingPattern.EndsWith("/"))
                    IsRegExp = true;
                else
                    IsRegExp = false;
            }
        }

        [NotNull]
        private ActionExecutionContext Context
        {
            get
            {
                return _context ?? (_context = new ActionExecutionContext());
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="rootModel">The RootModel.</param>
        public override void HandleMessage(Message message, RootModel rootModel)
        {
            Assert.ArgumentNotNull(message, "message");
            Assert.ArgumentNotNull(rootModel, "rootModel");

            var textMessage = message as TextMessage;
            if (textMessage == null || string.IsNullOrEmpty(MatchingPattern))
            {
                return;
            }

            if (IsRegExp)
            {
                Regex rExp = new Regex(MatchingPattern.Substring(1, MatchingPattern.Length - 2));
                Match m = rExp.Match(textMessage.InnerText);
                if (!m.Success)
                    return;

                for(int i = 0; i < 10; i++)
                {
                    if (i + 1< m.Groups.Count)
                        Context.Parameters[i] = m.Groups[i + 1].ToString();
                    else
                        Context.Parameters[i] = String.Empty;
                }
            }
            else
            {
                ClearMatchingResults();
                var res = GetRootPatternToken(rootModel).Match(textMessage.InnerText, 0, _matchingResults);
                if (!res.IsSuccess)
                {
                    return;
                }

                for (int i = 0; i < 10; i++)
                {
                    Context.Parameters[i] = _matchingResults[i];
                }
            }

            Context.CurrentMessage = message;

            foreach (var action in Actions)
            {
                action.Execute(rootModel, Context);
            }

            if (StopProcessingTriggersAfterThis)
            {
                textMessage.SkipTriggers = true;
            }

            if (DoNotDisplayOriginalMessage)
            {
                textMessage.Handled = true;
            }
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
                _rootPatternToken = WildcardParser.ParseWildcardString(MatchingPattern, rootModel);
            }

            return _rootPatternToken;
        }
    }
}

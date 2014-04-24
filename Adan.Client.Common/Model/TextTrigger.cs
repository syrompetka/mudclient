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
    using Adan.Client.Common.Settings;

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
        [NonSerialized]
        private string _matchingPattern;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextTrigger"/> class.
        /// </summary>
        public TextTrigger()
            : base()
        {
            MatchingPattern = string.Empty;
            _context = new ActionExecutionContext();
            IsRegExp = false;
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

                if (!string.IsNullOrEmpty(value) && value[0] == '/' && value[value.Length - 1] == '/')
                {
                    _matchingPattern = value.Substring(1, value.Length - 2);
                    IsRegExp = true;
                }
                else
                    _matchingPattern = value;
                
                _rootPatternToken = null;
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
                Regex rExp = new Regex(MatchingPattern);
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string GetPatternString()
        {
            return MatchingPattern;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string UndoInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("#Триггер: ").Append("#action {").Append(GetPatternString()).Append("} ");
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
        public override void Undo()
        {
            if (Group != null && Operation != UndoOperation.None)
            {
                switch (Operation)
                {
                    case UndoOperation.Add:
                        Group.Triggers.Add(this);
                        break;
                    case UndoOperation.Remove:
                        Group.Triggers.Remove(this);
                        break;
                }
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

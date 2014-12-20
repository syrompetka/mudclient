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
    using System.Diagnostics;

    /// <summary>
    /// Trigger that handles text messages from server.
    /// </summary>
    [Serializable]
    public class TextTrigger : TriggerBase
    {
        [NonSerialized]
        private readonly IList<string> _matchingResults = new List<string>(Enumerable.Repeat<string>(string.Empty, 10));

        [NonSerialized]
        private PatternToken _rootPatternToken;

        [NonSerialized]
        private ActionExecutionContext _context;

        [NonSerialized]
        private string _matchingPattern;

        private Regex _wildRegex = new Regex(@"%[0-9]", RegexOptions.Compiled);

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

                if (value.Length > 2 && value[0] == '/' && value[value.Length - 1] == '/')
                {
                    _matchingPattern = value.Substring(1, value.Length - 2);
                    IsRegExp = true;
                }
                else
                {
                    _matchingPattern = value;
                    IsRegExp = false;
                }
                
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
            if (textMessage == null || string.IsNullOrEmpty(MatchingPattern) || string.IsNullOrEmpty(textMessage.InnerText))
            {
                return;
            }

            ClearMatchingResults();

            if (IsRegExp)
            {
                var varReplace = rootModel.ReplaceVariables(MatchingPattern);
                if (!varReplace.IsAllVariables)
                    return;

                Regex rExp = new Regex(varReplace.Value);
                Match match = rExp.Match(textMessage.InnerText);

                if (!match.Success)
                    return;

                for (int i = 0; i < 10; i++)
                {
                    if (i + 1 < match.Groups.Count)
                        Context.Parameters[i] = match.Groups[i + 1].ToString();
                }
            }
            else
            {
                //var res = GetRootPatternToken(rootModel).Match(textMessage.InnerText, 0, _matchingResults);
                //if (!res.IsSuccess)
                //{
                //    return;
                //}

                var varReplace = rootModel.ReplaceVariables(MatchingPattern);
                if (!varReplace.IsAllVariables)
                    return;

                Regex rExp = new Regex(_wildRegex.Replace(Regex.Escape(varReplace.Value),
                    m =>
                    {
                        return string.Format("(?<{0}>.*)", int.Parse(m.Value[1].ToString()) + 1);
                    }));

                Match match = rExp.Match(textMessage.InnerText);
                if (!match.Success)
                    return;

                for (int i = 0; i <= 10; i++)
                {
                    Context.Parameters[i] = match.Groups[(i + 1).ToString()].Value;
                }

                //for (int i = 0; i < 10; i++)
                //{
                //    if (i < _matchingResults.Count)
                //        Context.Parameters[i] = _matchingResults[i];
                //}
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
        /// <param name="rootModel"></param>
        public override void Undo(RootModel rootModel)
        {
            if (Group != null && Operation != UndoOperation.None)
            {
                switch (Operation)
                {
                    case UndoOperation.Add:
                        Group.Triggers.Add(this);
                        break;
                    case UndoOperation.Remove:
                        TriggerBase th = this;
                        Group.Triggers.TryTake(out th);
                        break;
                }

                rootModel.RecalculatedEnabledTriggersPriorities();
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
                _rootPatternToken = WildcardParser.ParseWildcardString(MatchingPattern, rootModel);
            }

            return _rootPatternToken;
        }
    }
}

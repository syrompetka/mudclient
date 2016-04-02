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
        private readonly IList<string> _matchingResults = new List<string>(Enumerable.Repeat(string.Empty, 10));

        [NonSerialized]
        private PatternToken _rootPatternToken;

        [NonSerialized]
        private ActionExecutionContext _context;

        [NonSerialized]
        private string _matchingPattern;

        [NonSerialized]
        private Regex _compiledRegex = null;

        private readonly Regex _wildRegex = new Regex(@"%[0-9]", RegexOptions.Compiled);

        /// <summary>
        /// Initializes a new instance of the <see cref="TextTrigger"/> class.
        /// </summary>
        public TextTrigger()
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
                }

                if (IsRegExp && (value.IndexOf("$") == -1 || value.IndexOf("$") == value.Length - 1))
                    _compiledRegex = new Regex(_matchingPattern, RegexOptions.Compiled | RegexOptions.CultureInvariant);
                else
                    _compiledRegex = null;
                
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

        public bool MatchMessage(TextMessage textMessage, RootModel rootModel)
        {
            ClearMatchingResults();

            if (IsRegExp)
            {
                Match match;
                if (_compiledRegex != null)
                    match = _compiledRegex.Match(textMessage.InnerText);
                else
                {
                    var varReplace = rootModel.ReplaceVariables(MatchingPattern);
                    if (!varReplace.IsAllVariables)
                        return false;

                    Regex rExp = new Regex(varReplace.Value);
                    match = rExp.Match(textMessage.InnerText);
                }

                if (!match.Success)
                    return false;

                for (int i = 0; i < 10; i++)
                {
                    if (i + 1 < match.Groups.Count)
                        _matchingResults[i] = match.Groups[i + 1].ToString();
                }

                return true;
            }
            else
            {
                var res = GetRootPatternToken(rootModel).Match(textMessage.InnerText, 0, _matchingResults);
                return res.IsSuccess;
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

            if (!MatchMessage(textMessage, rootModel))
                return;

            for (int i = 0; i < 10; i++)
            {
                if (i < _matchingResults.Count)
                    Context.Parameters[i] = _matchingResults[i];
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

        public override string GetPatternString()
        {
            return MatchingPattern;
        }

        public override string UndoInfo()
        {
            var sb = new StringBuilder();
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
                        Group.Triggers.Remove(this);
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

        /// <summary>
        /// Represent trigger as string (similar to JMC)
        /// </summary>
        /// <returns>String representation of the trigger</returns>
        public override string ToString()
        {
            var patternString = this.GetPatternString();
            if (this.IsRegExp)
                patternString = "/" + patternString + "/";
            if (this.Group == null)
                return string.Format("#action {{{0}}} {{{1}}} {{{2}}}", patternString, this.Actions[0].ToString(), this.Priority);
            else
                return string.Format("#action {{{0}}} {{{1}}} {{{2}}} {{{3}}}", patternString, this.Actions[0].ToString(), this.Priority, this.Group.Name);
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Substitution.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the Substitution type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Model
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml.Serialization;
    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Messages;
    using Utils.PatternMatching;

    /// <summary>
    /// A replacement of all incoming strings that match certain pattern to specific one.
    /// </summary>
    [Serializable]
    public class Substitution : IUndo
    {
        [NonSerialized]
        private readonly IList<string> _matchingResults = new List<string>(Enumerable.Repeat<string>(null, 11));
        [NonSerialized]
        private PatternToken _rootPatternToken;
        [NonSerialized]
        private PatternToken _rootSubstituteWithPatternToken;
        [NonSerialized]
        private string _pattern = string.Empty;
        [NonSerialized]
        private string _substituteWith = string.Empty;
        [NonSerialized]
        private Regex _compiledRegex = null;

        private Regex _wildRegex = new Regex(@"%[0-9]", RegexOptions.Compiled);

        /// <summary>
        /// Initializes a new instance of the <see cref="Substitution"/> class.
        /// </summary>
        public Substitution()
        {
            _pattern = string.Empty;
            _substituteWith = string.Empty;

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
        /// Gets or sets the pattern.
        /// </summary>
        /// <value>
        /// The pattern.
        /// </value>
        [NotNull]
        [XmlAttribute]
        public string Pattern
        {
            get
            {
                return _pattern;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                if (value.Length > 2 && value[0] == '/' && value[value.Length - 1] == '/')
                {
                    _pattern = value.Substring(1, value.Length - 2);
                    IsRegExp = true;
                }
                else
                {
                    _pattern = value;
                }

                if (IsRegExp && (value.IndexOf("$") == -1 || value.IndexOf("$") == value.Length - 1))
                    _compiledRegex = new Regex(_pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant);
                else
                    _compiledRegex = null;

                _rootPatternToken = null;
            }
        }

        /// <summary>
        /// Gets or sets the substitute with.
        /// </summary>
        /// <value>
        /// The substitute with.
        /// </value>
        [NotNull]
        [XmlAttribute]
        public string SubstituteWith
        {
            get
            {
                return _substituteWith;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                _substituteWith = value;
                _rootSubstituteWithPatternToken = null;
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
        /// 
        /// </summary>
        /// <param name="textMessage"></param>
        /// <param name="rootModel"></param>
        public void HandleMessage([NotNull] TextMessage textMessage, [NotNull]RootModel rootModel)
        {
            Assert.ArgumentNotNull(textMessage, "message");
            Assert.ArgumentNotNull(rootModel, "rootModel");

            ClearMatchingResults();
            string text = textMessage.InnerText;

            if (string.IsNullOrEmpty(textMessage.InnerText))
                return;

            if (IsRegExp)
            {
                Regex rExp;
                if (_compiledRegex != null)
                {
                    rExp = _compiledRegex;
                }
                else
                {
                    var varReplace = rootModel.ReplaceVariables(_pattern);
                    if (!varReplace.IsAllVariables)
                        return;

                    rExp = new Regex(varReplace.Value);
                }

                StringBuilder sb = new StringBuilder(text.Length);
                int offset = 0;
                Match match = rExp.Match(textMessage.InnerText);
                if (match.Success)
                {
                    do
                    {
                        if (offset < match.Index)
                            sb.Append(text, offset, match.Index - offset);

                        offset = match.Index + match.Length;

                        sb.Append(_wildRegex.Replace(SubstituteWith,
                            m =>
                            {
                                return match.Groups[int.Parse(m.Value.Substring(1))].Value;
                            }));

                        match = rExp.Match(textMessage.InnerText, match.Index + match.Length);
                    } while (match.Success);

                    if (offset < textMessage.InnerText.Length)
                        sb.Append(text, offset, textMessage.InnerText.Length - offset);

                    //TODO: We have cleared information about color :( Need to do something with this
                    textMessage.Clear();
                    textMessage.AppendText(sb.ToString());
                }
            }
            else
            {
                int position = 0;
                ClearMatchingResults();

                var res = GetRootPatternToken(rootModel).Match(text, position, _matchingResults);

                while (res.IsSuccess)
                {
                    //Actually method TextMessage.Substitute() too heavy if we are substituting more than one substitution in one string
                    //TODO: Best way build string right here
                    textMessage.Substitute(res.StartPosition, res.EndPosition - res.StartPosition, GetSubstituteWithPatternToken(rootModel).GetValue(_matchingResults));
                    
                    position = res.StartPosition + GetSubstituteWithPatternToken(rootModel).GetValue(_matchingResults).Length;
                    ClearMatchingResults();

                    if (position > text.Length)
                        break;

                    res = GetRootPatternToken(rootModel).Match(text, position, _matchingResults);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string UndoInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("#Замена {");
            if (IsRegExp)
                sb.Append("/");
            sb.Append(Pattern);
            if (IsRegExp)
                sb.Append("/");
            sb.Append("} ");
            switch (Operation)
            {
                case UndoOperation.Add:
                    sb.Append("восстановлена");
                    break;
                case UndoOperation.Remove:
                    sb.Append("удалена");
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
                        Group.Substitutions.Add(this);
                        break;
                    case UndoOperation.Remove:                        
                        Group.Substitutions.Remove(this);
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
        private PatternToken GetSubstituteWithPatternToken([NotNull] RootModel rootModel)
        {
            Assert.ArgumentNotNull(rootModel, "rootModel");

            if (_rootSubstituteWithPatternToken == null)
            {
                _rootSubstituteWithPatternToken = WildcardParser.ParseWildcardString(SubstituteWith, rootModel);
            }

            return _rootSubstituteWithPatternToken;
        }

        [NotNull]
        private PatternToken GetRootPatternToken([NotNull] RootModel rootModel)
        {
            Assert.ArgumentNotNull(rootModel, "rootModel");

            if (_rootPatternToken == null)
            {
                _rootPatternToken = WildcardParser.ParseWildcardString(Pattern, rootModel);
            }

            return _rootPatternToken;
        }
    }
}
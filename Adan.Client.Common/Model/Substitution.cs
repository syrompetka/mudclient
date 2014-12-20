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
                    IsRegExp = false;
                }

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

            //ClearMatchingResults();
            string text = textMessage.InnerText;

            if (string.IsNullOrEmpty(textMessage.InnerText))
                return;

            if (IsRegExp)
            {
                var varReplace = rootModel.ReplaceVariables(_pattern);
                if (!varReplace.IsAllVariables)
                    return;

                Regex rExp = new Regex(varReplace.Value);
                //var matches = rExp.Matches(text);

                //if (matches.Count == 1)
                    //return;

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

                        //StringBuilder value = new StringBuilder(SubstituteWith);
                        //for (int k = 1; k < match.Groups.Count; ++k)
                        //    value.Replace("%" + (k - 1), match.Groups[k].Value);

                        //sb.Append(value);

                        match = rExp.Match(textMessage.InnerText, match.Index + match.Length);
                    } while (match.Success);

                    if (offset < textMessage.InnerText.Length)
                        sb.Append(text, offset, textMessage.InnerText.Length - offset);

                    //for (int i = 0; i < matches.Count; ++i)
                    //{
                    //    if (offset < matches[i].Index)
                    //        sb.Append(text, offset, matches[i].Index - offset);

                    //    offset = matches[i].Index + matches[i].Length;

                    //    //StringBuilder value = new StringBuilder(SubstituteWith);
                    //    //for (int k = 1; k < matches[i].Groups.Count; ++k)
                    //    //    value.Replace("%" + (k - 1), matches[i].Groups[k].Value);
                    //    sb.Append(_wildRegex.Replace(SubstituteWith,
                    //        m =>
                    //        {
                    //            return matches[i].Groups[m.Value[1]].Value;
                    //        }));

                    //    //sb.Append(value);
                    //}

                    textMessage.Clear();
                    textMessage.AppendText(sb.ToString());
                }
            }
            else
            {
                var varReplace = rootModel.ReplaceVariables(_pattern);
                if (!varReplace.IsAllVariables)
                    return;

                Regex rExp = new Regex(_wildRegex.Replace(Regex.Escape(varReplace.Value),
                    m =>
                    {
                        return string.Format("(?<{0}>.*)", int.Parse(m.Value[1].ToString()) + 1);
                    }));
                //var matches = rExp.Matches(text);

                //if (matches.Count == 0)
                //    return;

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
                                return match.Groups[(int.Parse(m.Value[1].ToString()) + 1).ToString()].Value;
                            }));

                        //StringBuilder value = new StringBuilder(SubstituteWith);
                        //for (int k = 1; k < match.Groups.Count; ++k)
                        //    value.Replace("%" + (k - 1), match.Groups[k].Value);

                        //sb.Append(value);

                        match = rExp.Match(textMessage.InnerText, match.Index + match.Length);
                    } while (match.Success);
                    //for (int i = 0; i < matches.Count; ++i)
                    //{
                    //    if (offset < matches[i].Index)
                    //        sb.Append(text, offset, matches[i].Index - offset);

                    //    offset = matches[i].Index + matches[i].Length;

                    //    sb.Append(_wildRegex.Replace(SubstituteWith,
                    //        m =>
                    //        {
                    //            return matches[i].Groups[m.Value[1]].Value;
                    //        }));

                    //    //StringBuilder value = new StringBuilder(SubstituteWith);
                    //    //for (int k = 1; k < matches[i].Groups.Count; ++k)
                    //    //    value.Replace("%" + (k - 1), matches[i].Groups[k].Value);

                    //    //sb.Append(value);
                    //}

                    if (offset < textMessage.InnerText.Length)
                        sb.Append(text, offset, textMessage.InnerText.Length - offset);

                    textMessage.Clear();
                    textMessage.AppendText(sb.ToString());

                    //ClearMatchingResults();
                    //int position = 0;
                    //var res = GetRootPatternToken(rootModel).Match(text, position, _matchingResults);
                    //while (res.IsSuccess)
                    //{
                    //    textMessage.Substitution(res.StartPosition, res.EndPosition - res.StartPosition, SubstituteWith);

                    //    //TODO: Что это за говно?
                    //    position = res.StartPosition + GetSubstituteWithPatternToken(rootModel).GetValue(_matchingResults).Length;
                    //    ClearMatchingResults();

                    //    if (position > text.Length)
                    //        break;

                    //    res = GetRootPatternToken(rootModel).Match(text, position, _matchingResults);
                    //}
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
                        Substitution th = this;
                        Group.Substitutions.TryTake(out th);
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
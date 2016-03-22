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

            string text = textMessage.InnerText;
            if (string.IsNullOrEmpty(textMessage.InnerText))
                return;

            //TODO: Вместо встроенного метода Substitute в TextMessage выгоднее реализовать вручную замену, чтобы каждый раз не пересчитывать уже обработанные начальные блоки.
            if (IsRegExp)
            {
                var varReplace = rootModel.ReplaceVariables(_pattern);
                if (!varReplace.IsAllVariables)
                    return;

                Regex rExp = new Regex(varReplace.Value);
                int offset = 0;
                Match match = rExp.Match(textMessage.InnerText);
                if (match.Success)
                {
                    do
                    {                        
                        offset = match.Index + match.Length;
                        textMessage.Substitute(match.Index, match.Length, _wildRegex.Replace(SubstituteWith,
                            m =>
                            {
                                return match.Groups[int.Parse(m.Value.Substring(1))].Value;
                            }));
                        match = rExp.Match(textMessage.InnerText, match.Index + match.Length);
                    } while (match.Success);
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
                
                int offset = 0;
                Match match = rExp.Match(textMessage.InnerText);
                if (match.Success)
                {
                    do
                    {                        
                        offset = match.Index + match.Length;
                        textMessage.Substitute(match.Index, match.Length, _wildRegex.Replace(SubstituteWith,
                            m =>
                            {
                                return match.Groups[int.Parse(m.Value.Substring(1))].Value;
                            }));
                        match = rExp.Match(textMessage.InnerText, match.Index + match.Length);
                    } while (match.Success);
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
    }
}
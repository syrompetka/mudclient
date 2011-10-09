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
    using System.Linq;
    using System.Xml.Serialization;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Messages;

    using Utils.PatternMatching;

    /// <summary>
    /// A replacement of all incoming strings that match certain pattern to specific one.
    /// </summary>
    [Serializable]
    public class Substitution
    {
        [NonSerialized]
        private readonly IList<string> _matchingResults = new List<string>(Enumerable.Repeat<string>(null, 11));

        [NonSerialized]
        private PatternToken _rootPatternToken;
        [NonSerialized]
        private PatternToken _rootSubstituteWithPatternToken;

        private string _pattern = string.Empty;
        private string _substituteWith = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Substitution"/> class.
        /// </summary>
        public Substitution()
        {
            _pattern = string.Empty;
            _substituteWith = string.Empty;
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

                _pattern = value;
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
                _rootSubstituteWithPatternToken = WildcardParser.ParseWildcardString(value);
            }
        }

        [NotNull]
        private PatternToken RootPatternToken
        {
            get
            {
                if (_rootPatternToken == null)
                {
                    _rootPatternToken = WildcardParser.ParseWildcardString(Pattern);
                }

                return _rootPatternToken;
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void HandleMessage([NotNull] TextMessage message)
        {
            Assert.ArgumentNotNull(message, "message");

            foreach (var block in message.MessageBlocks)
            {
                ClearMatchingResults();
                int position = 0;
                var res = RootPatternToken.Match(block.Text, position, _matchingResults);
                while (res.IsSuccess)
                {
                    var replaceResult = block.Text.Substring(0, res.StartPosition)
                                 + _rootSubstituteWithPatternToken.GetValue(_matchingResults)
                                 + block.Text.Substring(res.EndPosition);
                    if (replaceResult != block.Text)
                    {
                        block.ChangeInnerText(replaceResult);
                        message.UpdateInnerText();
                    }

                    position = res.StartPosition + _rootSubstituteWithPatternToken.GetValue(_matchingResults).Length;
                    ClearMatchingResults();
                    res = RootPatternToken.Match(block.Text, position, _matchingResults);
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
    }
}

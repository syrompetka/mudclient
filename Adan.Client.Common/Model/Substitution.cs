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
                _rootSubstituteWithPatternToken = null;
            }
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
            var res = GetRootPatternToken(rootModel).Match(text, 0, _matchingResults);
            if (res.IsSuccess)
            {
                int position = 0;
                int coloredPosition = 0;
                int coloredStartPosition = 0;
                int lastPosition = res.StartPosition - position;
                string coloredText = textMessage.ColoredText;
                StringBuilder sb = new StringBuilder(coloredText.Length);
                do
                {
                    int count = 0;
                    if (lastPosition == 0)
                    {
                        while (coloredText[coloredStartPosition] == '\x1B')
                        {
                            coloredStartPosition += 2;
                            var tt = coloredText[coloredStartPosition];

                            while (coloredStartPosition < coloredText.Length && coloredText[coloredStartPosition] != 'm')
                                coloredStartPosition++;

                            if (coloredStartPosition == coloredText.Length)
                            {
                                coloredStartPosition = 0;
                            }
                            else
                            {
                                coloredStartPosition++;
                            }
                        }
                    }
                    else
                    {
                        while (count < lastPosition)
                        {
                            if (coloredText[coloredStartPosition] == '\x1B')
                            {
                                coloredStartPosition += 2;
                                var tt = coloredText[coloredStartPosition];

                                while (coloredStartPosition < coloredText.Length && coloredText[coloredStartPosition] != 'm')
                                    coloredStartPosition++;

                                if (coloredStartPosition == coloredText.Length)
                                    break;

                                coloredStartPosition++;
                            }
                            else
                            {
                                count++;
                                coloredStartPosition++;
                            }
                        }
                    }

                    sb.Append(coloredText, coloredPosition, coloredStartPosition - coloredPosition);
                    sb.Append(GetSubstituteWithPatternToken(rootModel).GetValue(_matchingResults));

                    lastPosition = res.EndPosition - position;
                    count = 0;
                    while (count < lastPosition)
                    {
                        if (coloredText[coloredPosition] == '\x1B')
                        {
                            coloredPosition += 2;
                            var tt = coloredText[coloredPosition];

                            while (coloredPosition < coloredText.Length && coloredText[coloredPosition] != 'm')
                                coloredPosition++;

                            if (coloredPosition == coloredText.Length)
                                break;

                            coloredPosition++;
                        }
                        else
                        {
                            count++;
                            coloredPosition++;
                        }
                    }

                    position = res.StartPosition + GetSubstituteWithPatternToken(rootModel).GetValue(_matchingResults).Length;
                    ClearMatchingResults();
                    if (position >= text.Length)
                    {
                        break;
                    }

                    res = GetRootPatternToken(rootModel).Match(text, position, _matchingResults);
                } while (res.IsSuccess);

                if (coloredPosition < coloredText.Length)
                    sb.Append(coloredText, coloredPosition, coloredText.Length - coloredPosition);

                textMessage.Clear();
                textMessage.AddText(sb.ToString());
                //rootModel.PushMessageToConveyor(textMessage.NewInstance());
                //textMessage.Handled = true;
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
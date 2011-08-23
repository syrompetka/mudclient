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
    using System.Runtime.Serialization;
    using System.Text.RegularExpressions;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Messages;
    using Utils;

    /// <summary>
    /// A replacement of all incoming strings that match certain pattern to specific one.
    /// </summary>
    [DataContract]
    public class Substitution
    {
        private Regex _regex;

        private string _pattern = string.Empty;
        private string _substituteWith = string.Empty;
        private string _substituteWithRegexPattern = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Substitution"/> class.
        /// </summary>
        public Substitution()
        {
            _pattern = string.Empty;
            _substituteWith = string.Empty;
            _substituteWithRegexPattern = string.Empty;
        }

        /// <summary>
        /// Gets or sets the pattern.
        /// </summary>
        /// <value>
        /// The pattern.
        /// </value>
        [NotNull]
        [DataMember]
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
                _regex = null;
            }
        }

        /// <summary>
        /// Gets or sets the substitute with.
        /// </summary>
        /// <value>
        /// The substitute with.
        /// </value>
        [NotNull]
        [DataMember]
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
                _substituteWithRegexPattern = WildcardStringHelper.ConvertToValidRegexSubstitutePattern(value);
            }
        }

        [NotNull]
        private Regex Regex
        {
            get
            {
                return _regex ?? (_regex = new Regex(WildcardStringHelper.ConvertToValidRegex(Pattern)));
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
                var replaceResult = Regex.Replace(block.Text, _substituteWithRegexPattern);
                if (replaceResult == block.Text)
                {
                    continue;
                }

                block.ChangeInnerText(replaceResult);
                message.UpdateInnerText();
            }
        }
    }
}

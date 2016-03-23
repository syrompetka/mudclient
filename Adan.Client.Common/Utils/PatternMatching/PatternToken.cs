// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PatternToken.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the PatternToken type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Utils.PatternMatching
{
    using System.Collections.Generic;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// A single token of pattern to match.
    /// </summary>
    public abstract class PatternToken
    {
        /// <summary>
        /// Gets the next token.
        /// </summary>
        [CanBeNull]
        protected PatternToken NextToken
        {
            get;
            private set;
        }

        /// <summary>
        /// Matches the specified string.
        /// </summary>
        /// <param name="target">The string to match.</param>
        /// <param name="position">The position at the <paramref name="target"/> to start with.</param>
        /// <param name="matchingResults">The mathing results.</param>
        /// <returns>
        ///  <c>true</c> if matching was sucessfull; otherwise - <c>false</c>.
        /// </returns>
        public abstract MatchingResult Match([NotNull] string target, int position, [NotNull] IList<string> matchingResults);

        /// <summary>
        /// Gets the value of this pattern with applied matching results.
        /// </summary>
        /// <param name="matchingResults">The matching results.</param>
        /// <returns>A string value of this patthern.</returns>
        [NotNull]
        public abstract string GetValue([NotNull] IList<string> matchingResults);

        /// <summary>
        /// Sets the next token.
        /// </summary>
        /// <param name="patternToken">The pattern token to set as next token.</param>
        public void SetNextToken([NotNull] PatternToken patternToken)
        {
            Assert.ArgumentNotNull(patternToken, "patternToken");

            NextToken = patternToken;
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConstantStringToken.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ConstantStringToken type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Utils.PatternMatching
{
    using System;
    using System.Collections.Generic;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// A constant string token.
    /// </summary>
    public class ConstantStringToken : PatternToken
    {
        private readonly PatternToken _previousToken;
        private readonly bool _anchored;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantStringToken"/> class.
        /// </summary>
        /// <param name="previousToken">The previous token.</param>
        /// <param name="anchored">if set to <c>true</c> [anchored].</param>
        /// <param name="searchValue">The constant string.</param>
        public ConstantStringToken([CanBeNull] PatternToken previousToken, bool anchored, [NotNull] string searchValue)
        {
            Assert.ArgumentNotNull(searchValue, "searchValue");

            _previousToken = previousToken;
            _anchored = anchored;
            SearchValue = searchValue;
        }

        /// <summary>
        /// Gets or sets values to search.
        /// </summary>
        /// <value>
        /// The search value.
        /// </value>
        protected string SearchValue
        {
            get;
            set;
        }
        
        /// <summary>
        /// Matches the specified string.
        /// </summary>
        /// <param name="target">The string to match.</param>
        /// <param name="position">The position at the <paramref name="target"/> to start with.</param>
        /// <param name="matchingResults">The mathing results.</param>
        /// <returns>
        ///   <c>true</c> if matching was sucessfull; otherwise - <c>false</c>.
        /// </returns>
        public override MatchingResult Match(string target, int position, IList<string> matchingResults)
        {
            Assert.ArgumentNotNull(target, "target");
            Assert.ArgumentNotNull(matchingResults, "matchingResults");

            var foundPosition = target.IndexOf(SearchValue, position, StringComparison.Ordinal);
            if (foundPosition < 0)
            {
                return new MatchingResult { IsSuccess = false };
            }

            if (_anchored && foundPosition != position)
            {
                return new MatchingResult { IsSuccess = false };
            }

            var previousWildcardToken = _previousToken as WildcardToken;
            if (previousWildcardToken != null && !previousWildcardToken.TryToSetValue(target.Substring(position, foundPosition - position), matchingResults))
            {
                return new MatchingResult { IsSuccess = false };
            }

            if (NextToken != null)
            {
                var res = NextToken.Match(target, foundPosition + SearchValue.Length, matchingResults);
                return res.IsSuccess
                           ? new MatchingResult { IsSuccess = true, StartPosition = foundPosition, EndPosition = res.EndPosition }
                           : new MatchingResult { IsSuccess = false };
            }

            return new MatchingResult { IsSuccess = true, StartPosition = foundPosition, EndPosition = foundPosition + SearchValue.Length };
        }

        /// <summary>
        /// Gets the value of this pattern with applied matching results.
        /// </summary>
        /// <param name="matchingResults">The matching results.</param>
        /// <returns>
        /// A string value of this patthern.
        /// </returns>
        public override string GetValue(IList<string> matchingResults)
        {
            Assert.ArgumentNotNull(matchingResults, "matchingResults");

            if (NextToken != null)
            {
                return SearchValue + NextToken.GetValue(matchingResults);
            }

            return SearchValue;
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WildcardToken.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the WildCardTokent type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Utils.PatternMatching
{
    using System;
    using System.Collections.Generic;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Represents wildcard matching token ('%i').
    /// </summary>
    public class WildcardToken : PatternToken
    {
        private readonly int _wildcardNumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="WildcardToken"/> class.
        /// </summary>
        /// <param name="wildcardNumber">The wild card number.</param>
        public WildcardToken(int wildcardNumber)
        {
            _wildcardNumber = wildcardNumber;
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

            if (NextToken != null)
            {
                var res = NextToken.Match(target, position, matchingResults);
                return res.IsSuccess
                           ? new MatchingResult { IsSuccess = true, StartPosition = position, EndPosition = res.EndPosition }
                           : new MatchingResult { IsSuccess = false };
            }

            if (position < target.Length)
            {
                var value = target.Substring(position);
                return TryToSetValue(value, matchingResults)
                           ? new MatchingResult { IsSuccess = true, StartPosition = position, EndPosition = target.Length - 1 }
                           : new MatchingResult { IsSuccess = false };
            }

            return new MatchingResult { IsSuccess = true, StartPosition = position, EndPosition = target.Length - 1 };
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

            var val = matchingResults[_wildcardNumber] ?? string.Empty;
            if (NextToken != null)
            {
                return val + NextToken.GetValue(matchingResults);
            }

            return val;
        }

        /// <summary>
        /// Tries to set value.
        /// </summary>
        /// <param name="valueToSet">The value to set.</param>
        /// <param name="matchingResults">The matching results.</param>
        /// <returns>
        ///   <c>true</c> if value was set; otherwise - <c>false</c>.
        /// </returns>
        public bool TryToSetValue([NotNull] string valueToSet, [NotNull]IList<string> matchingResults)
        {
            Assert.ArgumentNotNull(valueToSet, "valueToSet");
            Assert.ArgumentNotNull(matchingResults, "matchingResults");

            if (!String.IsNullOrEmpty(matchingResults[_wildcardNumber]))
            {
                return string.Equals(valueToSet, matchingResults[_wildcardNumber], StringComparison.Ordinal);
            }

            matchingResults[_wildcardNumber] = valueToSet;
            return true;
        }
    }
}

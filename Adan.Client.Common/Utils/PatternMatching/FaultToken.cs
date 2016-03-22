// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FaultToken.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the FaultToken type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Utils.PatternMatching
{
    using System;
    using System.Collections.Generic;

    using CSLib.Net.Diagnostics;

    /// <summary>
    /// A token which always faults.
    /// </summary>
    public class FaultToken : PatternToken
    {
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

            return new MatchingResult { IsSuccess = false };
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

            return string.Empty;
        }
    }
}

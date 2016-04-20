// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VariableReferenceToken.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the VariableReferenceToken type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Utils.PatternMatching
{
    using System.Collections.Generic;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Model;

    /// <summary>
    /// A <see cref="PatternToken"/> that references some variable by name.
    /// </summary>
    public class VariableReferenceToken : ConstantStringToken
    {
        private readonly RootModel _rootModel;
        private readonly string _variableName;

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableReferenceToken"/> class.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="previousToken">The previous token.</param>
        /// <param name="anchored">if set to <c>true</c> [anchored].</param>
        /// <param name="variableName">Name of the variable.</param>
        public VariableReferenceToken([NotNull] RootModel rootModel, [CanBeNull] PatternToken previousToken, bool anchored, [NotNull] string variableName)
            : base(previousToken, anchored, string.Empty)
        {
            Assert.ArgumentNotNull(rootModel, "rootModel");
            Assert.ArgumentNotNull(variableName, "variableName");

            _rootModel = rootModel;
            _variableName = variableName;
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

            SearchValue = _rootModel.GetVariableValue(_variableName);
            return base.Match(target, position, matchingResults);
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

            SearchValue = _rootModel.GetVariableValue(_variableName);
            return base.GetValue(matchingResults);
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchCaseCollection.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   A collection of switch cases.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Utils
{
    using System.Collections.ObjectModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// A collection of switch cases.
    /// </summary>
    public sealed class SwitchCaseCollection : Collection<SwitchCase>
    {
        /// <summary>
        /// Adds a new case to the collection.
        /// </summary>
        /// <param name="when">The value to compare against the input.</param>
        /// <param name="then">The output value to use if the case matches.</param>
        public void Add([NotNull] object when, [NotNull] object then)
        {
            Assert.ArgumentNotNull(when, "when");
            Assert.ArgumentNotNull(then, "then");

            Add(new SwitchCase { When = when, Then = then });
        }
    }
}   

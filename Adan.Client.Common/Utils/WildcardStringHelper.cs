// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WildcardStringHelper.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the WildcardStringHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Utils
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Class to hold helper methods for working with wildcards.
    /// </summary>
    public static class WildcardStringHelper
    {
        /// <summary>
        /// Converts valid to valid regex pattern string.
        /// </summary>
        /// <param name="wildcardToConvert">The wildcard to convert.</param>
        /// <returns>
        /// Converted regex pattern string.
        /// </returns>
        [NotNull]
        public static string ConvertToValidRegex([NotNull] string wildcardToConvert)
        {
            Assert.ArgumentNotNull(wildcardToConvert, "wildcardToConvert");

            string res = Regex.Escape(wildcardToConvert);
            if (res.StartsWith(@"\^", StringComparison.Ordinal))
            {
                res = res.Remove(0, 1);
            }

            for (int i = 1; i < 10; i++)
            {
                var groupPos = res.IndexOf("%" + i, StringComparison.Ordinal);
                if (groupPos >= 0)
                {
                    res = res.Remove(groupPos, 2).Insert(groupPos, string.Format(CultureInfo.InvariantCulture, "(?<{0}>.*)", i));
                    res = res.Replace("%" + i, string.Format(CultureInfo.InvariantCulture, @"\k<{0}>", i));
                }
            }

            return res;
        }

        /// <summary>
        /// Converts a string to valid regex substitute pattern.
        /// </summary>
        /// <param name="substitutePattern">The substitute pattern.</param>
        /// <returns>
        /// A valid regex substitute pattern.
        /// </returns>
        [NotNull]
        public static string ConvertToValidRegexSubstitutePattern([NotNull] string substitutePattern)
        {
            Assert.ArgumentNotNull(substitutePattern, "substitutePattern");
            var res = substitutePattern;
            for (int i = 1; i < 10; i++)
            {
                res = res.Replace("%" + i, "${" + i + "}");
            }

            return res;
        }
    }
}

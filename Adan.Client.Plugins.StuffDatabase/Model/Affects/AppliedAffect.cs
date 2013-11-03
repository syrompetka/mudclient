// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppliedAffect.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Base class for all affect applied to a character wearing some object.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.StuffDatabase.Model.Affects
{
    using System;
    using System.Globalization;

    using Common.Messages;

    using CSLib.Net.Annotations;

    using Properties;

    /// <summary>
    /// Base class for all affect applied to a character wearing some object.
    /// </summary>
    [Serializable]
    public abstract class AppliedAffect
    {
        /// <summary>
        /// Gets the info message representing this affect.
        /// </summary>
        /// <returns><see cref="InfoMessage"/> instance.</returns>
        [NotNull]
        public abstract InfoMessage ConvertToInfoMessage();

        /// <summary>
        /// Gets the ASCII time.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns>String representation of specified duration.</returns>
        [NotNull]
        protected static string GetAsciiTime(int duration)
        {
            int d = (duration * 60) / 1440;
            int h = ((duration * 60) - (d * 1440)) / 60;
            return d > 0
                       ? string.Format(CultureInfo.CurrentUICulture, Resources.LongTimeFormat, d, h)
                       : string.Format(CultureInfo.CurrentUICulture, Resources.ShortTimeFormat, h);
        }
    }
}
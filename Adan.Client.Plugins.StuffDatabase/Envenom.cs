// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Envenom.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   "Envenom" skill enhancement.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.StuffDatabase
{
    using System;
    using System.Globalization;
    using System.Xml.Serialization;

    using Common.Messages;

    using CSLib.Net.Annotations;

    using Properties;

    /// <summary>
    /// "Envenom" skill enhancement.
    /// </summary>
    public class Envenom : AppliedAffect
    {
        /// <summary>
        /// Gets or sets the duration of applied affect.
        /// </summary>
        /// <value>
        /// The duration of applied affect.
        /// </value>
        [XmlAttribute]
        public int Duration
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the info message representing this affect.
        /// </summary>
        /// <returns><see cref="InfoMessage"/> instance.</returns>
        public override InfoMessage ConvertToInfoMessage()
        {
            return new InfoMessage(string.Format(CultureInfo.CurrentUICulture, " " + Resources.AffectEnvenom, GetAsciiTime(Duration)));
        }
    }
}
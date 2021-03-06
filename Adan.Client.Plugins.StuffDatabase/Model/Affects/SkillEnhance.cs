﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SkillEnhance.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   An enhancement of some skill.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.StuffDatabase.Model.Affects
{
    using System;
    using System.Globalization;
    using System.Xml.Serialization;

    using Common.Messages;

    using CSLib.Net.Annotations;

    using Properties;
    using Adan.Client.Common.Themes;

    /// <summary>
    /// An enhancement of some skill.
    /// </summary>
    [Serializable]
    public class SkillEnhance : AppliedAffect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SkillEnhance"/> class.
        /// </summary>
        public SkillEnhance()
        {
            SkillName = string.Empty;
        }

        /// <summary>
        /// Gets or sets the name of the skill.
        /// </summary>
        /// <value>
        /// The name of the skill.
        /// </value>
        [NotNull]
        [XmlAttribute]
        public string SkillName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the enhance value.
        /// </summary>
        /// <value>
        /// The enhance value.
        /// </value>
        [XmlAttribute]
        public int EnhanceValue
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
            var infoMessage = new InfoMessage(string.Format(CultureInfo.CurrentUICulture, " " + Resources.SkillEnhance, EnhanceValue, SkillName));

            if (NecessarySetItemsCount > 0)
            {
                infoMessage.AppendText(string.Format(" (Необходимо {0} предмет{1} из набора)", NecessarySetItemsCount, NecessarySetItemsCount < 5 ? "а" : "ов"), TextColor.BrightYellow);
            }

            return infoMessage;
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SkillResist.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   A resistance to some skill.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.StuffDatabase.Model.Affects
{
    using System.Globalization;
    using System.Xml.Serialization;

    using Common.Messages;

    using CSLib.Net.Annotations;

    using Properties;
    using Adan.Client.Common.Themes;

    /// <summary>
    /// A resistance to some skill.
    /// </summary>
    public class SkillResist : AppliedAffect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SkillResist"/> class.
        /// </summary>
        public SkillResist()
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
        /// Gets or sets the resist value.
        /// </summary>
        /// <value>
        /// The resist value.
        /// </value>
        [XmlAttribute]
        public int ResistValue
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
            var infoMessage = new InfoMessage(string.Format(CultureInfo.CurrentUICulture, " " + Resources.SkillResist, SkillName, ResistValue));
            
            if (NecessarySetItemsCount > 0)
            {
                infoMessage.AppendText(string.Format(" (Необходимо {0} предметов из набора)", NecessarySetItemsCount), TextColor.BrightYellow);
            }

            return infoMessage;
        }
    }
}
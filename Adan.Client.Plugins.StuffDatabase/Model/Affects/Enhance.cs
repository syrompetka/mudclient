// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Enhance.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   A change of some characters parameter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.StuffDatabase.Model.Affects
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;
    using Common.Messages;
    using Common.Themes;
    using CSLib.Net.Annotations;

    /// <summary>
    /// A change of some characters parameter.
    /// </summary>
    public class Enhance : AppliedAffect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Enhance"/> class.
        /// </summary>
        public Enhance()
        {
            ModifiedParameter = string.Empty;
            SourceSkill = string.Empty;
        }

        /// <summary>
        /// Gets or sets the name of a skill caused this enhancment.
        /// </summary>
        /// <value>
        /// The name of a skill or empty string if enhancement caused not by a skill.
        /// </value>
        [NotNull]
        [XmlAttribute]
        public string SourceSkill
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of  modified parameter.
        /// </summary>
        /// <value>
        /// The name of modified parameter.
        /// </value>
        [NotNull]
        [XmlAttribute(AttributeName = "Type")]
        public string ModifiedParameter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value of this enhancement.
        /// </summary>
        /// <value>
        /// The value of this enhancement.
        /// </value>
        [XmlAttribute]
        public int Value
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the duration of this enhancement.
        /// </summary>
        /// <value>
        /// The duration of this enhancement.
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
            var infoMessage = new InfoMessage(" " + ModifiedParameter + ": ");

            infoMessage.AppendText(Value.ToString("+#;-#;", CultureInfo.CurrentCulture), Value > 0 ? TextColor.BrightGreen : TextColor.BrightRed);

            if (!string.IsNullOrEmpty(SourceSkill))
            {
                infoMessage.AppendText(string.Format(CultureInfo.CurrentUICulture, " ({0})", SourceSkill), TextColor.White);
            }

            if (Duration > 0)
            {
                infoMessage.AppendText(string.Format(CultureInfo.CurrentUICulture, " [{0}]", GetAsciiTime(Duration)), TextColor.White);
            }

            if (NecessarySetItemsCount > 0)
            {
                infoMessage.AppendText(string.Format(" (Необходимо {0} предметов из набора)", NecessarySetItemsCount), TextColor.BrightYellow);
            }

            return infoMessage;
        }
    }
}
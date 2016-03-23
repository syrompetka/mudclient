// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WandOrStaffSpell.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   A description of spell casted when some wand or staff is used.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.StuffDatabase
{
    using System;
    using System.Xml.Serialization;

    using CSLib.Net.Annotations;

    /// <summary>
    /// A description of spell casted when some wand or staff is used.
    /// </summary>
    [Serializable]
    public class WandOrStaffSpell
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WandOrStaffSpell"/> class.
        /// </summary>
        public WandOrStaffSpell()
        {
            SpellName = string.Empty;
        }

        /// <summary>
        /// Gets or sets the name of the spell.
        /// </summary>
        /// <value>
        /// The name of the spell.
        /// </value>
        [NotNull]
        [XmlAttribute(AttributeName = "Name")]
        public string SpellName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the total charges of the wand of staff.
        /// </summary>
        /// <value>
        /// The total charges.
        /// </value>
        [XmlAttribute]
        public int TotalCharges
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of charges left.
        /// </summary>
        /// <value>
        /// The number of charges left.
        /// </value>
        [XmlAttribute]
        public int ChargesLeft
        {
            get;
            set;
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScrollOrPotionSpell.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   A description of scroll or potion spell casted when some object is used.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.StuffDatabase
{
    using System;
    using System.Xml.Serialization;

    using CSLib.Net.Annotations;

    /// <summary>
    /// A description of scroll or potion spell casted when some object is used.
    /// </summary>
    [Serializable]
    public class ScrollOrPotionSpell
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollOrPotionSpell"/> class.
        /// </summary>
        public ScrollOrPotionSpell()
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
    }
}
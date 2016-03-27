// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpellBook.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Details of some spell book.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.StuffDatabase
{
    using System;
    using System.Xml.Serialization;

    using CSLib.Net.Annotations;

    /// <summary>
    /// Details of some spell book.
    /// </summary>
    [Serializable]
    public class SpellBook
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpellBook"/> class.
        /// </summary>
        public SpellBook()
        {
            SpellName = string.Empty;
            Profession = string.Empty;
        }

        /// <summary>
        /// Gets or sets the name of the spell.
        /// </summary>
        /// <value>
        /// The name of the spell.
        /// </value>
        [NotNull]
        [XmlAttribute]
        public string SpellName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the profession which can read this spell book.
        /// </summary>
        /// <value>
        /// The profession name.
        /// </value>
        [NotNull]
        [XmlAttribute]
        public string Profession
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of times this spell can be casted.
        /// </summary>
        /// <value>
        /// Positive integer if number of casts is limited; otherwise zero.
        /// </value>
        [XmlAttribute]
        public int CastCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the minimum level of a character to learn spell of the book.
        /// </summary>
        /// <value>
        /// Positive integer representing minimum level requried to read the book.
        /// </value>
        [XmlAttribute]
        public int LearnLevel
        {
            get;
            set;
        }
    }
}
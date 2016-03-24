// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CraftBook.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Details of some craft book.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.StuffDatabase
{
    using System;
    using System.Xml.Serialization;

    using CSLib.Net.Annotations;

    /// <summary>
    /// Details of some craft book.
    /// </summary>
    [Serializable]
    public class CraftBook
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CraftBook"/> class.
        /// </summary>
        public CraftBook()
        {
            Name = string.Empty;
            Description = string.Empty;
        }

        /// <summary>
        /// Gets or sets the name of recipe written in the book.
        /// </summary>
        /// <value>
        /// The name of recipe.
        /// </value>
        [NotNull]
        [XmlAttribute]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the description of recipe written in the book.
        /// </summary>
        /// <value>
        /// The description of recipe.
        /// </value>
        [NotNull]
        [XmlAttribute]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the minimum skill level required to lear the recipe of the book.
        /// </summary>
        /// <value>
        /// Positive integer representing required skill level or zero(0) if there are no level restrictions.
        /// </value>
        [XmlAttribute]
        public int MinSkillLevel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the min level required to use the book or recipe.
        /// </summary>
        /// <value>
        /// The min level required.
        /// </value>
        [XmlAttribute]
        public int MinLevel
        {
            get;
            set;
        }
    }
}
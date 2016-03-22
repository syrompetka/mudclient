// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArmorStats.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   A description of armor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.StuffDatabase
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// A description of armor.
    /// </summary>
    [Serializable]
    public class ArmorStats
    {
        /// <summary>
        /// Gets or sets the armor.
        /// </summary>
        /// <value>
        /// The armor.
        /// </value>
        [XmlAttribute]
        public int Armor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the armor class.
        /// </summary>
        /// <value>
        /// The armor class.
        /// </value>
        [XmlAttribute]
        public int ArmorClass
        {
            get;
            set;
        }
    }
}
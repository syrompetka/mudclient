// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeaponStats.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   A description of some weapon.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.StuffDatabase
{
    using System;
    using System.Xml.Serialization;

    using CSLib.Net.Annotations;

    /// <summary>
    /// A description of some weapon.
    /// </summary>
    [Serializable]
    public class WeaponStats
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WeaponStats"/> class.
        /// </summary>
        public WeaponStats()
        {
            RequiredSkill = string.Empty;
        }

        /// <summary>
        /// Gets or sets the required skill.
        /// </summary>
        /// <value>
        /// The required skill.
        /// </value>
        [NotNull]
        [XmlAttribute]
        public string RequiredSkill
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the average damage of the weapon.
        /// </summary>
        /// <value>
        /// The average damage.
        /// </value>
        [XmlAttribute]
        public float AverageDamage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the numer of dice sides.
        /// </summary>
        /// <value>
        /// The numer of dice sides.
        /// </value>
        [XmlAttribute]
        public int DiceSides
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the dice throw count.
        /// </summary>
        /// <value>
        /// The dice throw count.
        /// </value>
        [XmlAttribute]
        public int DiceCount
        {
            get;
            set;
        }
    }
}
﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MagicArrows.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   "Magic arrows" skill enhancement.
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
    /// "Magic arrows" skill enhancement.
    /// </summary>
    [Serializable]
    public class MagicArrows : AppliedAffect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MagicArrows"/> class.
        /// </summary>
        public MagicArrows()
        {
            MagicType = string.Empty;
        }

        /// <summary>
        /// Gets or sets the type of the magic.
        /// </summary>
        /// <value>
        /// The type of the magic.
        /// </value>
        [NotNull]
        [XmlAttribute]
        public string MagicType
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
            return Duration > 0 
                ? new InfoMessage(string.Format(CultureInfo.CurrentUICulture, " " + Resources.AffectMagicArrows, GetMagicType(), DiceSides, DiceCount, GetAsciiTime(Duration))) 
                : new InfoMessage(string.Format(CultureInfo.CurrentUICulture, " " + Resources.AffectMagicArrowsUnlimited, GetMagicType(), DiceSides, DiceCount));
        }

        [NotNull]
        private string GetMagicType()
        {
            switch (MagicType)
            {
                case "FIRE":
                    return Resources.AffectMagicArrowsFire;
                case "WATER":
                    return Resources.AffectMagicArrowsWater;
                case "AIR":
                    return Resources.AffectMagicArrowsAir;
                case "EARTH":
                    return Resources.AffectMagicArrowsEarth;
            }

            return "Unknown magic";
        }
    }
}
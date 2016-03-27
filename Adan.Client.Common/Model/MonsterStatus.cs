// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MonsterStatus.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the MonsterStatus type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Model
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// A class to describe status of single monster in players room.
    /// </summary>
    [Serializable]
    public class MonsterStatus : CharacterStatus
    {
        /// <summary>
        /// Gets or sets a value indicating whether this monster is player character.
        /// </summary>
        /// <value>
        /// <c>true</c> if this monster is player character; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool IsPlayerCharacter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this monster is boss.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this monster is boss; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool IsBoss
        {
            get;
            set;
        }
    }
}
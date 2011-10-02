// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CharacterStatus.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the CharacterStatus type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.GroupWidget
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    using CSLib.Net.Annotations;

    /// <summary>
    /// A class to describe status of single character in players group.
    /// </summary>
    [Serializable]
    public class CharacterStatus
    {
        private readonly List<Affect> _affects;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterStatus"/> class.
        /// </summary>
        public CharacterStatus()
        {
            Name = string.Empty;
            _affects = new List<Affect>();
        }

        /// <summary>
        /// Gets or sets the name of character.
        /// </summary>
        /// <value>
        /// The name of character.
        /// </value>
        [NotNull]
        [XmlAttribute]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets characters position.
        /// </summary>
        /// <value>
        /// <see cref="GroupWidget.Position"/> enumeration representing character position.
        /// </value>
        [XmlAttribute]
        public Position Position
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this character is in same room with current player.
        /// </summary>
        /// <value>
        /// <c>true</c> if this character is in same room; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool InSameRoom
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this charrackter is attacked by somebody else.
        /// </summary>
        /// <value>
        /// <c>true</c> if this charackter is attacked; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool IsAttacked
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the hits percent of this character.
        /// </summary>
        /// <value>
        /// The hits percent.
        /// </value>
        [XmlAttribute]
        public float HitsPercent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the moves percent of this character.
        /// </summary>
        /// <value>
        /// The moves percent.
        /// </value>
        [NotNull]
        [XmlAttribute]
        public float MovesPercent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the affects of this character.
        /// </summary>
        [NotNull]
        public List<Affect> Affects
        {
            get
            {
                return _affects;
            }
        }
    }
}
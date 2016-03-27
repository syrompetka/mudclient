// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Room.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the Room type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Map.Model
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    using CSLib.Net.Annotations;

    /// <summary>
    /// Represents a room.
    /// </summary>
    [Serializable]
    public class Room
    {
        private readonly List<RoomExit> _exits = new List<RoomExit>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Room"/> class.
        /// </summary>
        public Room()
        {
            Name = string.Empty;
            Description = string.Empty;
        }

        /// <summary>
        /// Gets or sets the unique identifier of this room.
        /// </summary>
        /// <value>
        /// The unique identifier of this room.
        /// </value>
        [XmlAttribute]
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of this room.
        /// </summary>
        /// <value>
        /// The name of this room.
        /// </value>
        [NotNull]
        [XmlAttribute]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the description of this room.
        /// </summary>
        /// <value>
        /// The description of this room.
        /// </value>
        [NotNull]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a collection of exits available in this room.
        /// </summary>
        [NotNull]
        public List<RoomExit> Exits
        {
            get
            {
                return _exits;
            }
        }

        /// <summary>
        /// Gets or sets the X location of this room.
        /// </summary>
        /// <value>
        /// The X location of this room.
        /// </value>
        [XmlAttribute]
        public int XLocation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Y location of this room.
        /// </summary>
        /// <value>
        /// The X location of this room.
        /// </value>
        [XmlAttribute]
        public int YLocation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Z location of this room.
        /// </summary>
        /// <value>
        /// The Z location of this room.
        /// </value>
        [XmlAttribute]
        public int ZLocation
        {
            get;
            set;
        }
    }
}

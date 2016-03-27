// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdditionalRoomParameters.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the AdditionalRoomParameters type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Map.Model
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    using Common.Model;

    using CSLib.Net.Annotations;

    /// <summary>
    /// Class to store user defined parameters of a room.
    /// </summary>
    [Serializable]
    public class AdditionalRoomParameters
    {
        private readonly List<ActionBase> _actionsToExecuteOnRoomEntry;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdditionalRoomParameters"/> class.
        /// </summary>
        public AdditionalRoomParameters()
        {
            _actionsToExecuteOnRoomEntry = new List<ActionBase>();
            RoomAlias = string.Empty;
        }

        /// <summary>
        /// Gets or sets identifier of a room these parameters belong to.
        /// </summary>
        /// <value>
        /// The room identifier.
        /// </value>
        [XmlAttribute]
        public int RoomId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the room alias.
        /// </summary>
        /// <value>
        /// The room alias.
        /// </value>
        [XmlAttribute]
        [NotNull]
        public string RoomAlias
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the comments for this room.
        /// </summary>
        /// <value>
        /// The comments for this room.
        /// </value>
        [NotNull]
        public string Comments
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this room has been visited by player previously.
        /// </summary>
        /// <value>
        /// <c>true</c> if this room has been visited; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool HasBeenVisited
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color to use to display this room on the map.
        /// </summary>
        /// <value>
        /// The color of this room.
        /// </value>
        [XmlAttribute]
        public RoomColor Color
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the icon to display over this room.
        /// </summary>
        /// <value>
        /// The icon of this room.
        /// </value>
        [XmlAttribute]
        public RoomIcon Icon
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the actions to execute on room entry.
        /// </summary>
        [NotNull]
        public List<ActionBase> ActionsToExecuteOnRoomEntry
        {
            get
            {
                return _actionsToExecuteOnRoomEntry;
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A deep copy of this instance.</returns>
        [NotNull]
        public AdditionalRoomParameters Clone()
        {
            return new AdditionalRoomParameters
                       {
                           Color = Color,
                           Comments = Comments,
                           HasBeenVisited = HasBeenVisited,
                           Icon = Icon,
                           RoomAlias = RoomAlias,
                           RoomId = RoomId,
                       };
        }
    }
}

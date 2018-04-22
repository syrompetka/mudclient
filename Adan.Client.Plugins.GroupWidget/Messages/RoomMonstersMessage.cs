// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoomMonstersMessage.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the RoomMonstersMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Plugins.GroupWidget.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    using Common.Messages;
    using Common.Model;

    using CSLib.Net.Annotations;

    /// <summary>
    /// A message containing status of monsters in players room.
    /// </summary>
    [Serializable]
    public class RoomMonstersMessage : Message
    {
        private readonly List<MonsterStatus> _monsters;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomMonstersMessage"/> class.
        /// </summary>
        public RoomMonstersMessage()
        {
            _monsters = new List<MonsterStatus>();
        }

        /// <summary>
        /// Gets the type of this message.
        /// </summary>
        /// <value>
        /// The type of this message.
        /// </value>
        public override int MessageType
        {
            get
            {
                return Constants.RoomMonstersMessage;
            }
        }

        [XmlAttribute]
        public bool IsRound
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the player group mates statuses.
        /// </summary>
        [NotNull]
        [XmlArrayItem(typeof(MonsterStatus), ElementName = "Monster")]
        public List<MonsterStatus> Monsters
        {
            get
            {
                return _monsters;
            }
        }
    }
}

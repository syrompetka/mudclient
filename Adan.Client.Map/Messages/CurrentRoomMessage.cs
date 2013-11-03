// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CurrentRoomMessage.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the CurrentRoomMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Map.Messages
{
    using System;
    using System.Xml.Serialization;

    using Common.Messages;

    /// <summary>
    /// Message that contains current room information.
    /// </summary>
    [Serializable]
    public class CurrentRoomMessage : Message
    {
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
                return Constants.CurrentRoomMessageType;
            }
        }

        /// <summary>
        /// Gets or sets the current room identifier.
        /// </summary>
        /// <value>
        /// The current room identifier.
        /// </value>
        [XmlAttribute]
        public int RoomId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current zone identifier.
        /// </summary>
        /// <value>
        /// The current zone identifier.
        /// </value>
        [XmlAttribute]
        public int ZoneId
        {
            get;
            set;
        }
    }
}

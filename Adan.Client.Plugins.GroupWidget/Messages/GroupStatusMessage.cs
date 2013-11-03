// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupStatusMessage.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the GroupStatusMessage type.
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
    /// A message containing status of players group.
    /// </summary>
    [Serializable]
    public class GroupStatusMessage : Message
    {
        private readonly List<CharacterStatus> _groupMates;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupStatusMessage"/> class.
        /// </summary>
        public GroupStatusMessage()
        {
            _groupMates = new List<CharacterStatus>();
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
                return Constants.GroupStatusMessageType;
            }
        }

        /// <summary>
        /// Gets the player group mates statuses.
        /// </summary>
        [NotNull]
        [XmlArrayItem(typeof(CharacterStatus), ElementName = "GroupMate")]
        [XmlArrayItem(typeof(PetStatus), ElementName = "Pet")]
        public List<CharacterStatus> GroupMates
        {
            get
            {
                return _groupMates;
            }
        }
    }
}

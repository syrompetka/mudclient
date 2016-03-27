// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtocolVersionMessage.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ProtocolVersionMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Messages
{
    using System;
    using System.Xml.Serialization;

    using Common.Messages;

    /// <summary>
    /// A message containing protocol version.
    /// </summary>
    [Serializable]
    public class ProtocolVersionMessage : Message
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
                return BuiltInMessageTypes.ProtocolVersionMessage;
            }
        }

        /// <summary>
        /// Gets or sets the version of protocol.
        /// </summary>
        /// <value>
        /// The version of protocol.
        /// </value>
        [XmlAttribute]
        public int Version
        {
            get;
            set;
        }
    }
}

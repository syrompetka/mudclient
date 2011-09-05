// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectedMessage.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the ConnectedMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Messages
{
    /// <summary>
    /// Message that appears when client connects to server.
    /// </summary>
    public class ConnectedMessage : Message
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
                return BuiltInMessageTypes.ConnectionMessages;
            }
        }
    }
}

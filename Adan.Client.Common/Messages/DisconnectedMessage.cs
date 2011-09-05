// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisconnectedMessage.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the DisconnectedMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Messages
{
    /// <summary>
    /// Message that appears when client losts connection to server.
    /// </summary>
    public class DisconnectedMessage : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisconnectedMessage"/> class.
        /// </summary>
        /// <param name="totalBytesReceived">The total bytes received.</param>
        /// <param name="bytesDecompressed">The bytes decompressed.</param>
        public DisconnectedMessage(int totalBytesReceived, int bytesDecompressed)
        {
            TotalBytesReceived = totalBytesReceived;
            BytesDecompressed = bytesDecompressed;
        }

        /// <summary>
        /// Gets the total bytes received.
        /// </summary>
        public int TotalBytesReceived
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the bytes decompressed.
        /// </summary>
        public int BytesDecompressed
        {
            get;
            private set;
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
                return BuiltInMessageTypes.ConnectionMessages;
            }
        }
    }
}
